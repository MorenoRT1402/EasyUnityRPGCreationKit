using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            { Stats.MAX_ATTACK, attack },
            { Stats.MAX_STRENGTH, strength },
            { Stats.MAX_DEX, dexterity },
            { Stats.MAX_MAGIC_ATTACK, magicAttack },
            { Stats.MAX_MIND, mind },
            { Stats.MAX_DEFENSE, defense },
            { Stats.MAX_MAGIC_DEFENSE, magicDefense },
            { Stats.MAX_SPEED, speed },
            { Stats.MAX_CRIT_PROB, critProb },
            { Stats.MAX_PRECISION, precision },
            { Stats.MAX_EVASION, evasion }
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
