using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats
{
    public InitialStatsSO so;
    public float life, mana, stamina, attack, strength, dexterity, magicAttack;
    public float mind, defense, magicDefense, speed;

    public float critProb, precision, evasion;

    public Dictionary<Stats, float> statsDict;

    public BaseStats(InitialStatsSO model)
    {
        so = model;
        Initialize();
    }

    public BaseStats(InitialStatsSO model, Dictionary<Stats, float> stats) : this(model)
    {
        InitDict(stats);
    }

    private void Initialize()
    {
        InitDict(so.GetStats());
    }

    private void InitDict(Dictionary<Stats, float> other)
    {
        statsDict = new();
        foreach (KeyValuePair<Stats, float> pair in other)
            statsDict[pair.Key] = pair.Value;
    }

        internal Dictionary<Stats, float> GetStats()
    {
        if (statsDict == null) InitDict(so.GetStats());
        return statsDict;
    }

    internal float Get(Stats stat)
    {
        if (statsDict == null) InitDict(so.GetStats());
        return statsDict[stat];
    }

    internal void Add(Stats stat, float amount)
    {
        statsDict[stat] += amount;
    }

    public override string ToString()
    {
        return $"Vida {life} \n Mana {mana} \n Stamina {stamina} \n Fuerza {strength} \n Destreza {dexterity} \n Mente {mind} \n Defensa {defense} \n Defensa M. {magicDefense} \n Velocidad {speed} \n Precision {precision} \n CritProb {critProb}%";
    }
}
