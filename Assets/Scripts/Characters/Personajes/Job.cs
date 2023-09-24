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
    private int experience;

    private BaseStats stats;

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

    public BaseStats Stats => stats;

    public BaseActiveSkill BasicAttack => basicAttack;
    public List<SkillBase> Skills => skills;
    public SpecialActiveSkill GuardSkill => guardSkill;
    public SpecialActiveSkill WaitSkill => waitSkill;
    public SpecialActiveSkill FleeSkill => fleeSkill;

    public int Exp => experience;
    public string LevelUpFormula => so.LevelUpFormula;

    #endregion

    public Job(JobSO jobModel)
    {
        Init(jobModel);
    }

    public Job(JobSO jobModel, string jobName, int[] expArray, Dictionary<Stats, float> statsDict, BaseActiveSkill basicAttack, List<SkillBase> skills, SpecialActiveSkill[] specialSkillsArray, List<string> jobEquipmentTypeEquipable, List<EquipPartSO> equipParts)
    {
        so = jobModel;
        name = jobName;

        level = expArray[0];
        maxLevel = expArray[1];
        experience = expArray[2];

        stats = new(so.initialStats, statsDict);

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

        stats = new(so.initialStats);

        experience = so.InitialExp;
        basicAttack = so.InitialBasicAttack;
        skills = so.InitialSkills;
        guardSkill = so.InitialGuardSkill;
        waitSkill = so.InitialWaitSkill;
        fleeSkill = so.InitialFleeSkill;

        equipmentTypeEquipable = so.equipmentTypeEquipable;
        equipParts = so.equipParts;
    }

    internal void LevelUp()
    {
        level++;
    }

    internal float Get(Stats stat)
    {
        return so.Get(stat);
    }

    internal void Add(Stats stat, float amount)
    {
        stats.Add(stat, amount);
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
