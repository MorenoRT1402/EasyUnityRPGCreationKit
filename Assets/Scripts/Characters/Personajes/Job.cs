using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job
{

    [SerializeField] private JobSO so;

    [Header("Stats")]

    private string name;
    private int level, maxLevel;
    private float life, mana, stamina, attack, strength, dexterity, magicAttack, mind, defense, magicDefense, speed;

    private float critProb, precision, evasion;

    private int experience, expNextLevel;
    private float expNextLvlMult;

    private BaseActiveSkill basicAttack;
    private List<SkillBase> skills;
    private SpecialActiveSkill guardSkill, waitSkill, fleeSkill;

    [Header("Equip")]
    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;


    #region Getters
    public JobSO SO => so;
    public string Name => name;
    public int Level => level;
    public int MaxLevel => maxLevel;
    public int InitialLevel => so.InitialLevel;
    public float Life => life;
    public float Mana => mana;
    public float Stamina => stamina;
    public float Attack => attack;
    public float Strength => strength;
    public float Defense => defense;
    public float Dexterity => dexterity;
    public float MagicAttack => magicAttack;
    public float Mind => mind;
    public float MagicDefense => magicDefense;
    public float Speed => speed;
    public float CritProb => critProb;
    public float Precision => precision;
    public float Evasion => evasion;

    public BaseActiveSkill BasicAttack => basicAttack;
    public List<SkillBase> Skills => skills;
    public SpecialActiveSkill GuardSkill => guardSkill;
    public SpecialActiveSkill WaitSkill => waitSkill;
    public SpecialActiveSkill FleeSkill => fleeSkill;

    public int Exp => experience;
    public int ExpNextLevel => expNextLevel;
    public int InitialExpNextLevel => so.InitialExpNextLevel;
    public float ExpNextLvlMult => expNextLvlMult;
    public string LevelUpFormula => so.LevelUpFormula;

    #endregion

    public Job(JobSO jobModel)
    {
        Init(jobModel);
    }

    public Job(JobSO jobModel, string jobName, int[] expArray, float[] statsArray, BaseActiveSkill basicAttack, List<SkillBase> skills, SpecialActiveSkill[] specialSkillsArray, List<string> jobEquipmentTypeEquipable, List<EquipPartSO> equipParts)
    {
        so = jobModel;
        name = jobName;

        level = expArray[0];
        maxLevel = expArray[1];
        experience = expArray[2];

        life = statsArray[0];
        mana = statsArray[1];
        stamina = statsArray[2];
        attack = statsArray[3];
        strength = statsArray[4];
        defense = statsArray[5];
        magicAttack = statsArray[6];
        mind = statsArray[7];
        magicDefense = statsArray[8];
        dexterity = statsArray[9];
        speed = statsArray[10];
        critProb = statsArray[11];
        precision = statsArray[12];
        evasion = statsArray[13];

        this.basicAttack = basicAttack;
        this.skills = skills;

        guardSkill = specialSkillsArray[0];
        waitSkill = specialSkillsArray[1];
        fleeSkill = specialSkillsArray[2];

        equipmentTypeEquipable = jobEquipmentTypeEquipable;
        this.equipParts = equipParts;
    }

    internal void Init(JobSO jobModel)
    {
        so = jobModel;
        Reset();
    }

    public void Reset()
    {
        name = so.InitialName;
        level = so.InitialLevel;
        maxLevel = so.InitialMaxLevel;
        life = so.InitialLife;
        mana = so.InitialMana;
        stamina = so.InitialStamina;
        strength = so.InitialStrength;
        dexterity = so.InitialDex;
        mind = so.InitialMind;
        defense = so.InitialDefense;
        magicDefense = so.InitialMagicDefense;
        speed = so.InitialSpeed;

        precision = so.InitialPrecision;
        critProb = so.InitialCritProb;
        evasion = so.InitialEvasion;

        experience = so.InitialExp;
        expNextLevel = so.InitialExpNextLevel;
        expNextLvlMult = so.InitialExpReqMult;
        basicAttack = so.InitialBasicAttack;
        skills = so.InitialSkills;
        guardSkill = so.InitialGuardSkill;
        waitSkill = so.InitialWaitSkill;
        fleeSkill = so.InitialFleeSkill;

        equipmentTypeEquipable = so.equipmentTypeEquipable;
        equipParts = so.equipParts;
    }

    internal void LevelUp(float nextLevelJP)
    {
        level++;

        expNextLevel = (int)nextLevelJP;
    }

    internal float Get(Stats stat)
    {
        return stat switch
        {
            Stats.HP_MAX => life,
            Stats.MP_MAX => mana,
            Stats.STAMINA_MAX => stamina,
            Stats.MAX_ATTACK => attack,
            Stats.MAX_STRENGTH => strength,
            Stats.MAX_DEFENSE => defense,
            Stats.MAX_MAGIC_ATTACK => magicAttack,
            Stats.MAX_MIND => mind,
            Stats.MAX_MAGIC_DEFENSE => magicDefense,
            Stats.MAX_DEX => dexterity,
            Stats.MAX_SPEED => speed,
            Stats.MAX_PRECISION => precision,
            Stats.MAX_EVASION => evasion,
            Stats.MAX_CRIT_PROB => critProb,
            _ => 0,
        };
    }

    internal void Add(Stats stat, float amount)
    {
        Dictionary<Stats, Action> statActions = new()
    {
        { Stats.HP_MAX, () => life += amount},
        { Stats.MP_MAX, () => mana += amount },
        { Stats.STAMINA_MAX, () => stamina += amount },
        { Stats.MAX_STRENGTH, () => strength += amount },
        { Stats.MAX_DEX, () => dexterity += amount },
        { Stats.MAX_MIND, () => mind += amount },
        { Stats.MAX_DEFENSE, () => defense += amount },
        { Stats.MAX_MAGIC_DEFENSE, () => magicDefense += amount },
        { Stats.MAX_SPEED, () => speed += amount },
        { Stats.MAX_PRECISION, () => precision += amount },
        { Stats.MAX_EVASION, () => evasion += amount },
        { Stats.MAX_CRIT_PROB, () => critProb += amount }
    };

        if (statActions.TryGetValue(stat, out Action action))
        {
            action.Invoke();
        }

    }

    internal void AddExp(int jp)
    {
        experience += jp;
    }

    internal void SetLevel(int level)
    {
        this.level = level;
    }

    internal void AddSkill(SkillBase skill)
    {
        if (skill == null) return;

        skills.Add(skill);
    }
}
