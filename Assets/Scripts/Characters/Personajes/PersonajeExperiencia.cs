using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeExperiencia
{

    private float c1 = 1.1f;
    private float c2 = 0.7f;

    [Header("Stats")]
    private PersonajeStats stats;
    private Job job;
    private PersonajeHandler handler;


    public void SetData(PersonajeHandler personajeHandler)
    {
        c1 = GeneralManager.Instance.C1;
        c2 = GeneralManager.Instance.C2;

        handler = personajeHandler;
        stats = personajeHandler.Stats.Character;
        job = personajeHandler.Stats.Job;
    }


    public void AddExp(int expObtenida)
    {
        if (stats.Level >= stats.MaxLevel) expObtenida = (int)(calculateReqExp(stats.Level) - stats.Exp); //To Max Level (estetic)
        if (expObtenida != 0)
        {
            stats.AddExp(expObtenida);
            while (stats.Exp >= calculateReqExp(stats.Level))
            {
                UpdateLevel();
            }
        }
        //        BattleManagerUI.Instance.ActualizarBarraExperiencia();
    }

    internal void AddJP(int jp)
    {
        if (jp > 0)
        {
            job.AddExp(jp);
            while (job.Exp >= CalculateJobReqExp(job.Level))
            {
                UpdateJobLevel();
            }
        }
    }

    private void UpdateJobLevel()
    {
        if (job.Level < job.MaxLevel)
        {
            handler.CharacterJobLevelUp();
        }
    }

    private void UpdateLevel()
    {
        if (stats.Level < stats.MaxLevel)
        {
            handler.CharacterLevelUp();
        }
    }

    /*
    private void setExpMaxNv(float expObtenida)
    {
        stats.Experiencia += expObtenida;
        stats.ExpRequeridaSiguienteNivel = stats.Experiencia;
        return;
    }
    */


    public float GetParcialActualExp() //Get Exp obtained in this level
    {
        int nivelActual = stats.Level;
        float RequiredExpPreviousLevel = calculateReqExp(nivelActual - 1);
        return Mathf.Max(stats.Exp - RequiredExpPreviousLevel, 0);
    }

    private float calculateReqExp(int nivel) //Get Exp necesary to Level Up
    {
        if (stats.InitialLevel != 0 && nivel == 0) return 0;

        int originalLevel = stats.Level;
        handler.Stats.Character.SetLevel(nivel);

        //        float valorIncremental = stats.ExpNextLvlMult; // Valor incremental por nivel
        int expFormula = (int)(GeneralManager.Instance.UseRecomendedExpFormula ? ExpFormula(nivel) : NCalcManager.Instance.CalculateObjectToFloat(handler.gameObject, stats.LevelUpFormula)); //If not works change newHandler for handler

        handler.Stats.Character.SetLevel(originalLevel);

        return expFormula;
    }

    internal float GetParcialExpNextLevel() //Get Exp left to Level Up
    {
        return calculateReqExp(stats.Level) - stats.Exp;
    }

    public float GetRelativeExpReq()
    { //Get Exp necesary to Level Up in this level
        int actualLevel = stats.Level;
        //        Debug.Log($"{calculateReqExp(actualLevel)} - {calculateReqExp(actualLevel - 1)} = {calculateReqExp(actualLevel) - calculateReqExp(actualLevel - 1)}");
        return calculateReqExp(actualLevel) - calculateReqExp(actualLevel - 1);
    }

    private float CalculateJobReqExp(int level)
    {
        if (job.InitialLevel != 0 && level == 0) return 0;

        int originalLevel = job.Level;
        handler.Stats.Job.SetLevel(level);

        int expFormula = (int)NCalcManager.Instance.CalculateObjectToFloat(handler.gameObject, job.LevelUpFormula);

        handler.Stats.Job.SetLevel(originalLevel);

        return expFormula;
    }
    internal float GetParcialActualJobExp()
    {
        int actualLevel = job.Level;
        float RequiredExpPreviousLevel = CalculateJobReqExp(actualLevel - 1);
        return Mathf.Max(job.Exp - RequiredExpPreviousLevel, 0);
    }

    internal float GetRelativeJobExpReq()
    {
        int actualLevel = job.Level;
        return CalculateJobReqExp(actualLevel) - CalculateJobReqExp(actualLevel - 1);
    }

    private int ExpFormula(int nivel) //For recommended formula
    {
        float tiempoPorNivel = MathF.Pow(nivel, c1);
        float dificultadEnemigos = MathF.Pow(nivel, c2);
        return (int)(tiempoPorNivel * dificultadEnemigos);
    }
}

