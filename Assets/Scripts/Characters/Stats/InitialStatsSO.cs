using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stats
{
    LEVEL, JOB_LEVEL,
    HP, HP_MAX, MP, MP_MAX, STAMINA, STAMINA_MAX,
    STRENGTH, DEX, MIND,
    ATTACK, MAGIC_ATTACK,
    DEFENSE, MAGIC_DEFENSE,
    SPEED, PRECISION, CRIT_PROB, EVASION,
}

[CreateAssetMenu(menuName = "Characters/Create Data/Stats")]
public class InitialStatsSO : ScriptableObject
{
    public float life, mana, stamina, attack, strength, dexterity, magicAttack;
    public float mind, defense, magicDefense, speed;

    public float critProb, precision, evasion;

    public Dictionary<Stats, float> statsDict;

    private void OnEnable() {
        Initialize();
    }

    private void Initialize()
    {
        InitDict();
    }

    private void InitDict()
    {
        statsDict = new(){
            { Stats.HP_MAX, life },
            { Stats.MP_MAX, mana },
            { Stats.STAMINA_MAX, stamina },
            { Stats.ATTACK, attack },
            { Stats.STRENGTH, strength },
            { Stats.DEX, dexterity },
            { Stats.MAGIC_ATTACK, magicAttack },
            { Stats.MIND, mind },
            { Stats.DEFENSE, defense },
            { Stats.MAGIC_DEFENSE, magicDefense },
            { Stats.SPEED, speed },
            { Stats.CRIT_PROB, critProb },
            { Stats.PRECISION, precision },
            { Stats.EVASION, evasion }
        };
    }

    internal float Get(Stats stat)
    {
        if(statsDict == null) InitDict();
        return statsDict[stat];
    }

    internal Dictionary<Stats, float> GetStats()
    {
        if (statsDict == null) InitDict();
        return statsDict;
    }
}
