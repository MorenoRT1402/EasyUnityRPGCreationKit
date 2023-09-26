using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new Job")]
[Serializable]
public class JobSO : ScriptableObject
{
    [Header("Initial Stats")]
    public string InitialName;
    public int InitialLevel, InitialMaxLevel;
    public string LevelUpFormula;
    public int InitialExp, InitialExpNextLevel;
    public float InitialExpReqMult;

    public InitialStatsSO initialStats;

    public BaseActiveSkill InitialBasicAttack;
    public List<SkillBase> InitialSkills;
    public SpecialActiveSkill InitialGuardSkill, InitialWaitSkill, InitialFleeSkill;

    [Header("Equip")]
    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;

    [Header("Growth Per Level")]
    [SerializeField] private StatsGrowthSO statsGrowth;

    [Space(20)]

    [SerializeField] private SkillsGrowthSO skillsLearn;

    #region Getters
    public SkillsGrowthSO SkillsLearn => skillsLearn;
    public Dictionary<Stats, string> GrowthFormulas => statsGrowth.growthFormulas;

    #endregion

        public float Get(Stats stat)
    {
        return initialStats.Get(stat);
    }
    public string GetGrowth(Stats stat)
    {
        return statsGrowth.Get(stat);
    }

}
