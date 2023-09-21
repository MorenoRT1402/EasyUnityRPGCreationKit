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
    public float InitialLife, InitialMana, InitialStamina, InitialStrength, InitialDex, InitialMind, InitialDefense, InitialMagicDefense, InitialSpeed;

    public float InitialCritProb, InitialPrecision, InitialEvasion;

    public int InitialExp, InitialExpNextLevel;
    public float InitialExpReqMult;

    public BaseActiveSkill InitialBasicAttack;
    public List<SkillBase> InitialSkills;
    public SpecialActiveSkill InitialGuardSkill, InitialWaitSkill, InitialFleeSkill;

    [Header("Equip")]
    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;

    public Dictionary<Stats, string> growthFormulas;

    [Header("Growth Per Level")]
    [SerializeField] private string lifeGrowth = "10";
    [SerializeField] private string manaGrowth = "5";
    [SerializeField] private string staminaGrowth = "2";
    [SerializeField] private string strengthGrowth = "1";
    [SerializeField] private string defenseGrowth = "rndm";
    [SerializeField] private string dexterityGrowth = "1";
    [SerializeField] private string mindGrowth = "1";
    [SerializeField] private string magicDefenseGrowth = "rndm";
    [SerializeField] private string speedGrowth = "1";
    [SerializeField] private string critProbGrowth = "0.0";
    [SerializeField] private string precisionGrowth = "0.0";
    [SerializeField] private string evasionGrowth = "0.0";

    [Space(20)]

    [SerializeField] private SkillsGrowthSO skillsLearn;

    #region Getters
    public SkillsGrowthSO SkillsLearn => skillsLearn;
    #endregion


    private void OnEnable()
    {
        initDict();
    }

    public string Get(Stats stat)
    {
        initDict();
        return growthFormulas[stat];
    }

    private void initDict()
    {
        growthFormulas = new Dictionary<Stats, string>()
        {

        { Stats.HP_MAX, lifeGrowth},
        { Stats.MP_MAX, manaGrowth},
        { Stats.STAMINA_MAX, staminaGrowth},
        { Stats.STRENGTH, strengthGrowth},
        { Stats.DEX, dexterityGrowth},
        { Stats.MIND, mindGrowth},
        { Stats.DEFENSE, defenseGrowth},
        { Stats.MAGIC_DEFENSE, magicDefenseGrowth},
        { Stats.SPEED, speedGrowth},
        { Stats.PRECISION, precisionGrowth},
        { Stats.EVASION, evasionGrowth},
        { Stats.CRIT_PROB, critProbGrowth}

        };
    }
}
