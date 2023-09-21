using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ModScale { SUM, MULT }


[CreateAssetMenu(menuName = "Skills/Skills/New Passive Skill")]
public class BasePassiveSkill : SkillBase
{
    public enum Type { NONE, STATS, RESISTANCES, AFFINITIES, SPECIAL }

    [SerializeField] Type type;

    #region Number Mods
    #region Stats
    [SerializeField] Stats stat;
    #endregion
    #region Affinities
    [SerializeField] List<Affinity> affinities;
    #endregion
    [SerializeField] ModScale modScale;
    [SerializeField] float amount;
    #endregion
    #region Special
    [SerializeField] AilmentSO effect;
    #endregion

    #region Getters
    public List<Affinity> Affinities => affinities;
    public AilmentSO Effect => effect;
    public bool IsReaction => effect != null && effect.IsReaction;
    #endregion

    internal float Get(Stats stat, ModScale scale)
    {
        if (type == Type.STATS && modScale == scale)
            if (this.stat == stat) return amount;
        return scale == ModScale.SUM ? 0 : 1;
    }

    internal float GetResistance(Affinity affinity)
    {
        if (type == Type.RESISTANCES)
            for (int i = 0; i < affinities.Count; i++)
                if (affinities[i] == affinity) return amount;
        return 1;
    }

    public List<Affinity> GetAffinities()
    {
        List<Affinity> affinities = new();
        if (type == Type.AFFINITIES)
            affinities.AddRange(this.affinities);
        return affinities.Distinct().ToList();
    }
}
