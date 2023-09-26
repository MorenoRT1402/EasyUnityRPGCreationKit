using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AilmentType
{
    NO_EFFECT,
    DAMAGE_PER_TIME, AFFINITY_MOD,
    STATS_MOD,
    CANT_MOVE,
    CONFUSION,
    ACTION_INVALID,
    OUT_OF_BATTLE,
    COUNTER
}

public class Ailment
{
    private float timer, timerTemp, timeToFinish;

    private AilmentSO so;
    private bool activate;
    private List<Ailment> secundaryAilments;

    public Ailment(AilmentSO sO)
    {
        so = sO;
        //        Debug.Log($"{so.GetInstanceID()} == {sO.GetInstanceID()} ? {so.GetInstanceID()==sO.GetInstanceID()}");
    }

    public Ailment(Ailment ailment)
    {
        so = ailment.SO;
    }


    #region Getters
    public AilmentSO SO => so;
    internal string Name => so.Name;
    internal float Accurate => so.Accurate;
    public bool Activate { get; set; }

    #endregion

    /*

    public void UpdateAilment(FighterStateMachine character)
    {
        if (so != null)
        {
            if (character.currentState != FighterStateMachine.TurnState.DEAD)
            {
                applyEffect(character);

                timer += Time.deltaTime;
                timerTemp += Time.deltaTime;

                //            Debug.Log($"{character.hero.Stats.Name} {so.Name} : {timeToFinish} != 0 && {timer} >= {timeToFinish} ? {timeToFinish != 0 && timer >= timeToFinish}");

                if (timeToFinish != 0 && timer >= timeToFinish) //time out
                {
                    EndOfAilment(character);
                }

                if (secundaryAilments != null && secundaryAilments.Count > 0) //for ailments with more than 1 effect
                    for (int sA = 0; sA < secundaryAilments.Count; sA++)
                        secundaryAilments[sA].UpdateAilment(character);
            }
            else if (so.AfterDeath) { EndOfAilment(character); }
        }
    }
    */
        public void UpdateAilment(PersonajeHandler character)
    {
        if (so != null)
        {
            if (!character.IsDead())
            {
                ApplyEffect(character);

                timer += Time.deltaTime;
                timerTemp += Time.deltaTime;

                //            Debug.Log($"{character.hero.Stats.Name} {so.Name} : {timeToFinish} != 0 && {timer} >= {timeToFinish} ? {timeToFinish != 0 && timer >= timeToFinish}");

                if (timeToFinish != 0 && timer >= timeToFinish) //time out
                {
                    EndOfAilment(character);
                }

                if (secundaryAilments != null && secundaryAilments.Count > 0) //for ailments with more than 1 effect
                    for (int sA = 0; sA < secundaryAilments.Count; sA++)
                        secundaryAilments[sA].UpdateAilment(character);
            }
            else if (so.AfterDeath) { EndOfAilment(character); }
        }
    }
        public void EndOfAilment(PersonajeHandler character)
    {
        RevertEffects(character);
        activate = false;
        character.Stats.Ailments.Remove(this);
    }

/*
        internal void EndOfAilment(StatsHandler stats)
    {
        revertEffects(stats);
        activate = false;
        stats.Ailments.Remove(this);
    }

    public void EndOfAilment(FighterStateMachine character)
    {
        revertEffects(character);
        activate = false;
        character.Hero.Stats.Ailments.Remove(this);
    }

    internal void EndOfAilment(StatsHandler stats)
    {
        revertEffects(stats);
        activate = false;
        stats.Ailments.Remove(this);
    }
    */

/*
    private void revertEffects(FighterStateMachine character)
    {
        so.revertEffect(character);
    }
    */
        private void ApplyEffect(PersonajeHandler character)
    {
        so.ApplyEffect(character, ref activate, ref timerTemp);
    }

        private void RevertEffects(PersonajeHandler handler)
    {
        so.revertEffect(handler);
    }

/*
    private void applyEffect(FighterStateMachine character)
    {
        so.applyEffect(character, ref activate, ref timerTemp);
    }
    */

    internal void Initialize()
    {
        timeToFinish = so.GetRandomTimeToFinish();

        timer = 0;
        activate = false;

        secundaryAilments = GetSecundaryAilments();

        foreach (Ailment ailment in secundaryAilments)
            ailment.Initialize();

    }

    private List<Ailment> GetSecundaryAilments()
    {
        List<Ailment> secundaryAilments = new List<Ailment>();
        List<AilmentSO> secundaryAilmentsSO = so.SecundaryAilments;

        foreach (AilmentSO sO in secundaryAilmentsSO)
            secundaryAilments.Add(new Ailment(sO));

        return secundaryAilments;
    }

    internal void TakeDamageCheck(PersonajeHandler character)
    {
        //        Debug.Log($"{character==null} {this==null} {so==null}");
        if (so != null && so.TakeDamageCheck())
            EndOfAilment(character);
    }

    internal List<AilmentSO> GetReactionStates()
    {
        return so.GetReactionStates();
    }
}
