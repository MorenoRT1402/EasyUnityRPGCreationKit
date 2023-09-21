using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Create Data/Growth/Stats")]
public class StatsGrowthSO : ScriptableObject
{

    [Header("Growth Formulas")]
    public string hp;
    public string mp, stamina;
    public string attack, strength;
    public string magicAttack, mind;
    public string defense, magicDefense;
    public string dexterity, speed;
    public string critProb, precision, evasion;

    public Dictionary<Stats, string> growthFormulas;

    private void OnEnable()
    {
        InitDict();
    }

    public string Get(Stats stat)
    {
        InitDict();
        return growthFormulas[stat];
    }

    private void InitDict()
    {
        growthFormulas = new Dictionary<Stats, string>()
        {

        { Stats.HP_MAX, hp},
        { Stats.MP_MAX, mp},
        { Stats.STAMINA_MAX, stamina},
        { Stats.MAX_ATTACK, attack},
        { Stats.MAX_STRENGTH, strength},
        { Stats.MAX_DEX, dexterity},
        { Stats.MAX_MAGIC_ATTACK, magicAttack},
        { Stats.MAX_MIND, mind},
        { Stats.MAX_DEFENSE, defense},
        { Stats.MAX_MAGIC_DEFENSE, magicDefense},
        { Stats.MAX_SPEED, speed},
        { Stats.MAX_PRECISION, precision},
        { Stats.MAX_EVASION, evasion},
        { Stats.MAX_CRIT_PROB, critProb}

        };
    }
}
