using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class PersonajeStats
{
    #region Variables

    #region Logical

    [SerializeField] private PersonajeStatsSO so;

    #endregion

    #region Stats

    [Header("Stats")]
    private string name;
    private int level, maxLevel;
    private int experience;
    private readonly BaseStats stats;

    private BaseActiveSkill basicAttack;
    private List<SkillBase> skills;
    private SpecialActiveSkill guardSkill, waitSkill, fleeSkill;

    [Header("Equip")]
    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;

    private Dictionary<EquipPartSO, EquipmentSO> initialEquip;


    #endregion

    #endregion

    #region G+S
    public string Name { get { return name; } set { name = value; } }
    #region Getters
    public PersonajeStatsSO SO => so;
    public Sprite SpriteInMenu => so.spriteInMenu;
    public int Level => level;
    public int InitialLevel => so.initialLevel;
    public int MaxLevel => maxLevel;
    public BaseStats Stats => stats;

    public List<PersonajeDropSO> Drops => so.drops;
    public BaseActiveSkill BasicAttack => basicAttack;
    public List<SkillBase> Skills => skills;
    public SpecialActiveSkill GuardSkill => guardSkill;
    public SpecialActiveSkill WaitSkill => waitSkill;
    public SpecialActiveSkill FleeSkill => fleeSkill;

    public int Exp => experience;
    //    public int InitialExpNextLevel => so.InitialExpNextLevel;
    public string LevelUpFormula => so.LevelUpFormula;

    public string JobPointsFormula => so.JobPointsFormula;

    public Dictionary<EquipPartSO, EquipmentSO> InitialEquip => initialEquip;

    #endregion
    #endregion


    #region Methods

    public PersonajeStats(PersonajeStatsSO statsModel)
    {
        so = statsModel;
        stats = new(statsModel.initialStats);
        DefaultValues();
    }

    public PersonajeStats(PersonajeStatsSO statsModel, string characterName, int[] expArray, Dictionary<Stats, float> stats, BaseActiveSkill basicAttack, List<SkillBase> skills, SpecialActiveSkill[] specialSkillsArray, List<string> equipmentTypeEquipable, List<EquipPartSO> equipParts)
    {
        so = statsModel;
        name = characterName;

        level = expArray[0];
        maxLevel = expArray[1];
        experience = expArray[2];

        this.stats = new(so.initialStats, stats);

        this.basicAttack = basicAttack;
        this.skills = skills;

        guardSkill = specialSkillsArray[0];
        waitSkill = specialSkillsArray[1];
        fleeSkill = specialSkillsArray[2];

        this.equipmentTypeEquipable = equipmentTypeEquipable;
        this.equipParts = equipParts;
    }

    public void LevelUp()
    {
        level++;

        //        expNextLevel += (int)(expNextLevel * expNextLvlMult);
    }

    internal void SetLevel(int level)
    {
        this.level = level;
    }

    public void DefaultValues()
    {
        name = so.initialName;
        level = so.initialLevel;
        maxLevel = so.initialMaxLevel;

        experience = so.InitialExp;

        basicAttack = so.initialBasicAttack;
        skills = new();
        skills.AddRange(so.initialSkills);
        guardSkill = so.InitialGuardSkill;
        waitSkill = so.InitialWaitSkill;
        fleeSkill = so.InitialFleeSkill;

        equipmentTypeEquipable = so.equipmentTypeEquipable;
        equipParts = so.equipParts;
        initialEquip = GetInitialEquip();
    }

    private Dictionary<EquipPartSO, EquipmentSO> GetInitialEquip()
    {
        List<EquipPartSO> equipParts = so.equipParts;
        List<EquipmentSO> initialEquip = so.initialEquipment;
        Dictionary<EquipPartSO, EquipmentSO> equipDict = new();

        for (int i = 0; i < equipParts.Count; i++)
        {
            equipDict.Add(equipParts[i], initialEquip[i]);
        }
        return equipDict;
    }

    internal void AddExp(int expObtenida)
    {
        if (Level < MaxLevel)
            experience += expObtenida;
    }

    internal float Get(Stats stat)
    {
        return stats.Get(stat);
    }

    internal void Add(Stats stat, float amount)
    {
        stats.Add(stat, amount);
    }

    internal void ChangeSkills(AddRemove changeOption, List<SkillBase> skills)
    {
        if (changeOption == AddRemove.REMOVE)
            foreach (SkillBase skill in skills)
            {
                if (changeOption == AddRemove.ADD)
                    AddSkill(skill);
                else if (this.skills.Contains(skill))
                    this.skills.Remove(skill);
            }
    }

    internal void SetSkills(List<SkillBase> skills)
    {
        this.skills = skills;
    }


    internal void AddSkill(SkillBase skill)
    {
        if (skill == null) return;

        skills.Add(skill);
        //        DebugManager.DebugList(skills, "\n");
    }

    internal float GetExpValue(GameObject fighter)
    {
        NCalcManager NCM = NCalcManager.Instance;
        string formula = so.ExpFormula;
        return NCM.CalculateObjectToFloat(fighter, formula);
    }

    internal float GetMoneyValue(GameObject fighter)
    {
        NCalcManager NCM = NCalcManager.Instance;
        string formula = so.MoneyFormula;
        return NCM.CalculateObjectToFloat(fighter, formula);
    }

    internal List<PersonajeDropSO> GetRandomDrops()
    {
        List<PersonajeDropSO> itemsDropped = new List<PersonajeDropSO>();
        foreach (PersonajeDropSO drop in Drops)
            if (Random.Range(0, 101f) < drop.probDrop)
                itemsDropped.Add(drop);
        return itemsDropped;
    }

    internal PersonajeDropSO GetRandomDrop()
    {
        foreach (PersonajeDropSO drop in Drops)
            if (Random.Range(0, 101f) < drop.probDrop)
                return drop;
        return null;
    }

    public override string ToString()
    {
        return $"{GetHashCode()} {name}\n Nivel {Level}\n {stats}";
    }
    #endregion

}
