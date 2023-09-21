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
    private float life, mana, stamina, attack, strength, dexterity, magicAttack, mind, defense, magicDefense, speed;

    private float critProb, precision, evasion;

    private int experience;

    // deprecated ---
    private int expNextLevel;
    private float expNextLvlMult;
    // -------------

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


    public List<PersonajeDropSO> Drops => so.drops;
    public BaseActiveSkill BasicAttack => basicAttack;
    public List<SkillBase> Skills => skills;
    public SpecialActiveSkill GuardSkill => guardSkill;
    public SpecialActiveSkill WaitSkill => waitSkill;
    public SpecialActiveSkill FleeSkill => fleeSkill;

    public int Exp => experience;
    public int ExpNextLevel => expNextLevel;
    //    public int InitialExpNextLevel => so.InitialExpNextLevel;
    public float ExpNextLvlMult => expNextLvlMult;
    public string LevelUpFormula => so.LevelUpFormula;

    public string JobPointsFormula => so.JobPointsFormula;

    public Dictionary<EquipPartSO, EquipmentSO> InitialEquip => initialEquip;

    #endregion
    #endregion


    #region Methods

    public PersonajeStats(PersonajeStatsSO statsModel)
    {
        so = statsModel;
        DefaultValues();
    }

    public PersonajeStats(PersonajeStatsSO statsModel, string characterName, int[] expArray, float[] statsArray, BaseActiveSkill basicAttack, List<SkillBase> skills, SpecialActiveSkill[] specialSkillsArray, List<string> equipmentTypeEquipable, List<EquipPartSO> equipParts)
    {
        so = statsModel;
        name = characterName;

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

        this.equipmentTypeEquipable = equipmentTypeEquipable;
        this.equipParts = equipParts;
    }

    public void LevelUp(float nextLevelExp)
    {
        level++;

        expNextLevel = (int)nextLevelExp;
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
        life = so.initialLife;
        mana = so.initialMana;
        stamina = so.initialStamina;
        attack = so.initialAttack;
        strength = so.initialStrength;
        dexterity = so.initialDexterity;
        magicAttack = so.initialMagicAttack;
        mind = so.initialMind;
        defense = so.initialDefense;
        magicDefense = so.initialMagicDefense;
        speed = so.initialSpeed;

        precision = so.initialPrecision;
        critProb = so.initialCritProb;
        evasion = so.initialEvasion;

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
        { Stats.MAX_ATTACK, () => attack += amount },
        { Stats.MAX_STRENGTH, () => strength += amount },
        { Stats.MAX_DEX, () => dexterity += amount },
        { Stats.MAX_MAGIC_ATTACK, () => magicAttack += amount },
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
        return $"{GetHashCode()} {name}\n Nivel {Level}\n Vida {life} \n Mana {mana} \n Stamina {stamina} \n Fuerza {strength} \n Destreza {dexterity} \n Mente {mind} \n Defensa {defense} \n Defensa M. {magicDefense} \n Velocidad {speed} \n Precision {precision} \n CritProb {critProb}%";
    }
    #endregion

}
