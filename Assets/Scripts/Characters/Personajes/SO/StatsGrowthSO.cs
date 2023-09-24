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
        { Stats.ATTACK, attack},
        { Stats.STRENGTH, strength},
        { Stats.DEX, dexterity},
        { Stats.MAGIC_ATTACK, magicAttack},
        { Stats.MIND, mind},
        { Stats.DEFENSE, defense},
        { Stats.MAGIC_DEFENSE, magicDefense},
        { Stats.SPEED, speed},
        { Stats.PRECISION, precision},
        { Stats.EVASION, evasion},
        { Stats.CRIT_PROB, critProb}

        };
    }
}
