using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Scripting;
using UnityEngine;
using Random = UnityEngine.Random;


public enum Stats
{
    HP_MAX, MP_MAX, STAMINA_MAX, STRENGTH, DEX, MIND, SPEED, PRECISION, CRIT_PROB, EVASION,
    HP,
    MP,
    STAMINA,
    LEVEL, JOB_LEVEL,
    DEFENSE,
    MAGIC_DEFENSE,
    ATTACK,
    MAGIC_ATTACK,
    MAX_ATTACK,
    MAX_STRENGTH,
    MAX_DEFENSE,
    MAX_MIND,
    MAX_MAGIC_DEFENSE,
    MAX_MAGIC_ATTACK,
    MAX_DEX,
    MAX_SPEED,
    MAX_PRECISION,
    MAX_EVASION,
    MAX_CRIT_PROB
}

public class StatsHandler
{
    public enum StatsReference { CHARACTER, JOB }
    private PersonajeStats character;
    private Job job;

    [Header("Equip")]
    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;

    Dictionary<EquipPartSO, EquipmentSO> equip;
    private Sprite spriteInMenu;

    [Header("Stats actuales")]
    private bool dead = false;
    private float actualLife;
    private float actualMana, actualStamina, actualStrength, actualAttack, actualDexterity, actualMind, actualMagicAttack, actualDefense, actualMagicDefense, actualSpeed;

    private float actualCritProb, actualPrecision, actualEvasion;
    public Dictionary<Stats, float> statsDict;
    public List<Ailment> ailments;
    //    [Range(0f, 100f)] private float moralActual;

    BaseActiveSkill basicAttack;
    List<SkillBase> skills;
    SpecialActiveSkill guardSkill, fleeSkill, waitSkill;

    public Dictionary<EquipPartSO, EquipmentSO> EquipDict => equip;
    private List<Job> jobDomain;

    #region G+S
    public string Name { get { return character.Name; } set { character.Name = value; } }
    public float ActualLife { get { return actualLife; } set { actualLife = value; } }

    #region Getters
    public Sprite SpriteInMenu => spriteInMenu;
    public PersonajeStats Character => character;
    public Job Job => job;
    public float ActualMana => actualMana;
    public float ActualStamina => actualStamina;
    public float ActualStrength => actualStrength;
    public float ActualAttack => actualAttack;
    public float ActualDexterity => actualDexterity;
    public float ActualMind => actualMind;
    public float ActualMagicAttack => actualMagicAttack;
    public float ActualDefense => actualDefense;
    public float ActualMagicDefense => actualMagicDefense;
    public float ActualSpeed => actualSpeed;
    public float ActualCritProb => actualCritProb;
    public float ActualPrecision => actualPrecision;
    public float ActualEvasion => actualEvasion;
    public List<Ailment> Ailments => ailments;
    public bool Dead => dead;

    public float MaxLife => GetSum(Stats.HP_MAX);
    public float MaxMana => GetSum(Stats.MP_MAX);
    public float MaxStamina => GetSum(Stats.STAMINA_MAX);
    public float MaxAttack => GetSum(Stats.MAX_ATTACK);
    public float MaxStrength => GetSum(Stats.MAX_STRENGTH);
    public float MaxDefense => GetSum(Stats.MAX_DEFENSE);
    public float MaxDexterity => GetSum(Stats.MAX_DEX);
    public float MaxMagicAttack => GetSum(Stats.MAX_MAGIC_ATTACK);
    public float MaxMind => GetSum(Stats.MAX_MIND);
    public float MaxMagicDefense => GetSum(Stats.MAX_MAGIC_DEFENSE);
    public float MaxSpeed => GetSum(Stats.MAX_SPEED);
    public float MaxCritProb => GetSum(Stats.MAX_CRIT_PROB);
    public float MaxPrecision => GetSum(Stats.MAX_PRECISION);
    public float MaxEvasion => GetSum(Stats.MAX_EVASION);

    public BaseActiveSkill BasicAttack => basicAttack;
    public List<SkillBase> Skills => skills;
    public SpecialActiveSkill GuardSkill => guardSkill;
    public SpecialActiveSkill FleeSkill => fleeSkill;
    public SpecialActiveSkill WaitSkill => waitSkill;

    public List<Job> JobDomains => jobDomain;


    #endregion
    #endregion


    public StatsHandler(PersonajeStats stats, Job job)
    {
        character = stats;
        this.job = job;

        DefaultValues();
        Heal();
    }

    public StatsHandler(PersonajeStats stats, Job job, Dictionary<EquipPartSO, EquipmentSO> equip, Sprite spriteInMenu, List<Job> jobDomains)
    {
        character = stats;
        this.job = job;
        this.equip = equip;
        this.spriteInMenu = spriteInMenu;
        jobDomain = jobDomains;
        

        DefaultValues();
        Heal();
    }

    private void DefaultValues()
    {
        jobDomain = new();
        spriteInMenu = character.SpriteInMenu; //By the moment only depends of character stats
        equipmentTypeEquipable = new List<string>();
        equipmentTypeEquipable.AddRange(character.equipmentTypeEquipable);
        equipmentTypeEquipable.AddRange(job.equipmentTypeEquipable);

        equipParts = job.equipParts.Count > 0 ? job.equipParts : character.equipParts;

        equip ??= character.InitialEquip;

        InitDict();
    }

    private void InitDict()
    {
        statsDict = new Dictionary<Stats, float>(){
            { Stats.LEVEL, character.Level},
            { Stats.JOB_LEVEL, job.Level},
            { Stats.HP, actualLife },
            { Stats.HP_MAX, MaxLife },
            { Stats.MP, actualMana },
            { Stats.MP_MAX, MaxMana },
            { Stats.STAMINA, actualStamina },
            { Stats.STAMINA_MAX, MaxStamina },
            { Stats.ATTACK, ActualAttack },
            { Stats.MAX_ATTACK, MaxAttack },
            { Stats.STRENGTH, actualStrength },
            { Stats.MAX_STRENGTH, MaxStrength },
            { Stats.DEFENSE, actualDefense },
            { Stats.MAX_DEFENSE, MaxDefense },
            { Stats.MAGIC_ATTACK, actualMagicAttack },
            { Stats.MAX_MAGIC_ATTACK, MaxMagicAttack },
            { Stats.MIND, actualMind },
            { Stats.MAX_MIND, MaxMind },
            { Stats.MAGIC_DEFENSE, actualMagicDefense },
            { Stats.MAX_MAGIC_DEFENSE, MaxMagicDefense },
            { Stats.DEX, actualDexterity },
            { Stats.MAX_DEX, MaxDexterity },
            { Stats.SPEED, actualSpeed },
            { Stats.MAX_SPEED, MaxSpeed },
            { Stats.PRECISION, actualPrecision },
            { Stats.MAX_PRECISION, MaxPrecision },
            { Stats.EVASION, actualEvasion },
            { Stats.MAX_EVASION, MaxEvasion },
            { Stats.CRIT_PROB, actualCritProb },
            { Stats.MAX_CRIT_PROB, MaxCritProb },
        };
    }
    public Dictionary<Stats, float> GetDictAtr()
    {
        return statsDict;
    }

    public float Get(Stats stat)
    {
        InitDict();
        return statsDict[stat];
    }

    internal void ModStats(Stats stat, ModScale scale, float amount)
    {
        float actualValue = Get(stat);
        float totalAmount = scale == ModScale.SUM ? amount : actualValue * amount;

        Add(stat, totalAmount, StatsReference.CHARACTER);
    }

    internal void Add(Stats statToGrowth, float valueAdd, StatsReference statsReference)
    {
        if (statsReference == StatsReference.CHARACTER)
            character.Add(statToGrowth, valueAdd);
        if (statsReference == StatsReference.JOB)
            job.Add(statToGrowth, valueAdd);

    }

    public void Heal()
    {
        Heal(MaxLife, MaxMana, MaxStamina);
        StatusHeal();

    }

    internal void Heal(float life, float mana, float stamina, bool status)
    {
        life *= MaxLife;
        mana *= MaxMana;
        stamina *= MaxStamina;

        if (life > 0)
            dead = false;

        Heal(life, mana, stamina);

        if (status)
            StatusHeal();
    }

    private void StatusHeal()
    {
        actualStrength = MaxStrength;
        actualDefense = MaxDefense;
        actualDexterity = MaxDexterity;
        actualMind = MaxMind;
        actualMagicDefense = MaxMagicDefense;
        actualSpeed = MaxSpeed;

        actualCritProb = MaxCritProb;
        actualPrecision = MaxPrecision;
        actualEvasion = MaxEvasion;

        ailments = new();

        UpdateSkills();
    }

    internal void Heal(float life, float mana, float stamina)
    {

        PartialHeal(ref actualLife, MaxLife, life);
        PartialHeal(ref actualMana, MaxMana, mana);
        PartialHeal(ref actualStamina, MaxStamina, stamina);

    }

    private void PartialHeal(ref float actual, float max, float added)
    {
        float result = actual + added;
        actual = result >= max ? max : result;
    }

    #region Get Sum

    private float GetSum(Stats stat)
    {
        UpdateSkills();
        //      Debug.Log($"SH Get Stat {stat} Value character {character.Get(stat)} + Job {job.Get(stat)} + Equip {GetEquipStat(stat)} = {character.Get(stat) + job.Get(stat) + GetEquipStat(stat)}");

        float totalStat = character.Get(stat) + job.Get(stat) + GetFixedBonus(stat);
        //        Debug.Log($"{totalStat} * {GetMultBonus(stat)} = {totalStat * GetMultBonus(stat)}");
        totalStat *= GetMultBonus(stat);
        return totalStat == 0 ? 0 : totalStat;
    }

    private float GetMultBonus(Stats stat)
    {
        float multSum = 1;

        multSum *= GetPassiveSkillsMultStat(stat);
        multSum *= GetEquipMultStat(stat);

        return multSum;
    }

    private float GetEquipMultStat(Stats stat)
    {
        float multSum = 1;
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in equip)
        {
            multSum *= equipSlot.Value != null ? equipSlot.Value.SkillsMultGet(stat) : 1;
        }
        return multSum;
    }

    private float GetPassiveSkillsMultStat(Stats stat)
    {
        float multSum = 1;
        List<BasePassiveSkill> passiveSkills = GetPassiveSkills();
        foreach (BasePassiveSkill skill in passiveSkills)
        {
            multSum *= skill.Get(stat, ModScale.MULT);
        }
        return multSum;
    }

    private float GetFixedBonus(Stats stat)
    {
        float fixedSum = 0;

        fixedSum += GetEquipStat(stat);
        fixedSum += GetPassiveSkillsFixedStat(stat);

        return fixedSum;
    }

    private float GetPassiveSkillsFixedStat(Stats stat)
    {
        float totalSum = 0;
        List<BasePassiveSkill> passiveSkills = GetPassiveSkills();
        if (passiveSkills.Count <= 0) return 0;
        foreach (BasePassiveSkill skill in passiveSkills)
        {
            totalSum += skill.Get(stat, ModScale.SUM);
        }
        return totalSum;
    }

    private float GetEquipStat(Stats stat)
    {
        float totalSum = 0;
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in equip)
        {
            totalSum += equipSlot.Value != null ? equipSlot.Value.Get(stat) : 0;
        }

        return totalSum;
    }

    #endregion

    internal List<Affinity> GetAffinities()
    {
        List<Affinity> affinities = new();
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipment in equip)
        {
            EquipmentSO equipmentPiece = equipment.Value;
            if (equipmentPiece != null)
            {
                List<Affinity> equipmentAffinities = equipmentPiece.GetAffinities();
                affinities.AddRange(equipmentAffinities);
            }
        }

        foreach (BasePassiveSkill passiveSkill in GetPassiveSkills())
            affinities.AddRange(passiveSkill.GetAffinities());

        return affinities.Distinct().ToList();
    }

    internal List<BaseActiveSkill> GetContextSkills(BaseActiveSkill.UsableOn context)
    {
        List<BaseActiveSkill> contextSkills = new();
        List<BaseActiveSkill> activeSkills = GetActiveSkills();
        for (int i = 0; i < activeSkills.Count; i++)
            if (activeSkills[i].IsUsable(context))
                contextSkills.Add(activeSkills[i]);
        return contextSkills;
    }

    public List<BaseActiveSkill> GetActiveSkills()
    {
        List<BaseActiveSkill> activeSkills = new();
        for (int i = 0; i < skills.Count; i++)
            if (skills[i] is BaseActiveSkill activeSkill)
                activeSkills.Add(activeSkill);
        return activeSkills;
    }
    private List<BasePassiveSkill> GetPassiveSkills()
    {
        List<BasePassiveSkill> passiveSkills = new();
        if (skills != null)
            for (int i = 0; i < skills.Count; i++)
                if (skills[i] is BasePassiveSkill passiveSkill)
                    passiveSkills.Add(passiveSkill);
        return passiveSkills;
    }

    internal List<AilmentSO> GetReactionStates()
    {
        List<AilmentSO> reactionStates = new();

        foreach (Ailment ailment in ailments)
            reactionStates.AddRange(ailment.GetReactionStates());

        List<BasePassiveSkill> passiveSkills = GetPassiveSkills();

        foreach (BasePassiveSkill passiveSkill in passiveSkills)
            if (passiveSkill.IsReaction) reactionStates.AddRange(passiveSkill.Effect.GetReactionStates());

        return reactionStates;
    }

    internal void ChangeSkills(AddRemove changeOption, List<SkillBase> skills)
    {
        if (changeOption == AddRemove.SET) character.SetSkills(skills);
        else
            character.ChangeSkills(changeOption, skills);
    }

    private void UpdateSkills()
    {
        skills = new();
        basicAttack = job.BasicAttack != null ? job.BasicAttack : character.BasicAttack;
        guardSkill = job.GuardSkill != null ? job.GuardSkill : character.GuardSkill;
        fleeSkill = job.FleeSkill != null ? job.FleeSkill : character.FleeSkill;
        waitSkill = job.WaitSkill != null ? job.WaitSkill : character.WaitSkill;

        skills.AddRange(character.Skills);
        skills.AddRange(job.Skills);

        DeleteDuplicates(skills);
    }

    private void DeleteDuplicates(List<SkillBase> skills)
    {
        List<BaseActiveSkill> uniqueActiveSkills = GetActiveSkills().Distinct().ToList();
        List<BasePassiveSkill> uniquePassiveSkills = GetPassiveSkills();
        if (!GeneralManager.Instance.allowPassiveSkillsDuplicates)
            uniquePassiveSkills.Distinct().ToList();

        skills.Clear();
        this.skills.AddRange(uniqueActiveSkills);
        this.skills.AddRange(uniquePassiveSkills);

    }

    internal List<string> GetSkillFamilysAvailable()
    {
        UpdateSkills();
        //        DebugManager.DebugList(skills, "\n");


        List<string> skillFamilyNames = new();
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i] is BaseActiveSkill activeSkill)
            {
                string family = activeSkill.family;
                if (family != null && family != "" && !skillFamilyNames.Contains(family))
                    skillFamilyNames.Add(activeSkill.family);
            }
        }
        return skillFamilyNames;
    }

    internal List<BaseActiveSkill> GetSkillsByFamily(string family)
    {
        List<BaseActiveSkill> skillsByFamily = new();

        for (int i = 0; i < skills.Count; i++)
            if (skills[i] is BaseActiveSkill activeSkill)
                if (activeSkill.family == family)
                    skillsByFamily.Add(activeSkill);
        return skillsByFamily;
    }

    internal void Remove(Ailment ailment)
    {
        ailment.Activate = false;
        Ailments.Remove(ailment);
    }

    internal void TakeDamage(float damage)
    {
        actualLife -= damage;
        actualLife = actualLife > MaxLife ? MaxLife : actualLife; // To avoid exceeding the maximum life value.
        if (actualLife < 1)
        {
            Ailment deathAilment = new(GeneralManager.Instance.DeathState);

            actualLife = 0;
            ailments.Add(deathAilment);
            dead = true;
        }
        else
            dead = false;
    }

    internal void SetHP(int amount, bool percent)
    {
        actualLife = percent ? MaxLife / 100 * amount : amount;
    }

    internal void UseMP(float manaCost)
    {
        actualMana -= manaCost;
        if (actualMana <= 0)
            actualMana = 0;
    }

    internal void UseStamina(float staminaCost)
    {
        actualStamina -= staminaCost;
        if (ActualStamina <= 0)
            actualStamina = 0;
    }

    internal void SetDead(bool v)
    {
        dead = v;
    }

    public void CharacterLevelUp(float nextLevelExp)
    {
        character.LevelUp(nextLevelExp);
    }

    internal void JobLevelUp(float nextLevelJP)
    {
        job.LevelUp(nextLevelJP);
    }

    internal void ActualMult(Stats stat, float statMult)
    {
        Dictionary<Stats, Action> statActions = new Dictionary<Stats, Action>()
    {
        { Stats.HP_MAX, () => actualLife *= statMult },
        { Stats.MP_MAX, () => actualMana *= statMult },
        { Stats.STAMINA_MAX, () => actualStamina *= statMult },
        { Stats.STRENGTH, () => actualStrength *= statMult },
        { Stats.ATTACK, () => actualAttack *= statMult },
        { Stats.DEX, () => actualDexterity *= statMult },
        { Stats.MIND, () => actualMind *= statMult },
        { Stats.MAGIC_ATTACK, () => actualMagicAttack *= statMult },
        { Stats.DEFENSE, () => actualDefense *= statMult },
        { Stats.MAGIC_DEFENSE, () => actualMagicDefense *= statMult },
        { Stats.SPEED, () => actualSpeed *= statMult },
        { Stats.PRECISION, () => actualPrecision *= statMult },
        { Stats.EVASION, () => actualEvasion *= statMult },
        { Stats.CRIT_PROB, () => actualCritProb *= statMult }
    };

        if (statActions.TryGetValue(stat, out Action action))
        {
            action.Invoke();
        }
    }

    internal float GetAffinityMult(Affinity affinity)
    {
        float mult = 1;

        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipment in equip)
            if (equipment.Value != null) mult *= equipment.Value.GetResistanceMult(affinity);

        List<BasePassiveSkill> passiveSkills = GetPassiveSkills();
        for (int i = 0; i < passiveSkills.Count; i++)
        {
            mult *= passiveSkills[i].GetResistance(affinity);
        }

        return mult;
    }

    internal void AilmentTakeDamage(PersonajeHandler character)
    {
        if (ailments.Count > 0)
        {

            for (int i = 0; i < ailments.Count; i++)
            {
                //                Debug.Log($"{character} {ailments[i]} {ailments[i].SO}");
                ailments[i].TakeDamageCheck(character);
            }
        }
    }

    internal float GetExpValue(GameObject fighter)
    {
        return character.GetExpValue(fighter);
    }

    internal float GetMoneyValue(GameObject fighter)
    {
        return character.GetMoneyValue(fighter);
    }

    #region Character

    #endregion
    #region Equip

    internal void Equip(EquipmentSO equipment, EquipPartSO part)
    {
        InventoryManager IM = InventoryManager.Instance;

        EquipmentSO oldEquipment = equip[part];
        IM.Add(oldEquipment, 1);
        equip[part] = equipment;
        IM.Remove(equipment, 1);
    }
    internal void Optimize()
    {
        List<EquipPartSO> partsList = new();
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in equip)
            partsList.Add(equipSlot.Key);

        for (int i = 0; i < partsList.Count; i++)
            EquipBestEquipment(partsList[i]);
    }

    private void EquipBestEquipment(EquipPartSO part)
    {
        float initialMagnitude = GetStatsMagnitude(part);
        Dictionary<EquipmentSO, float> equipables = new();

        if (equip[part] != null)
        {
            equipables.Add(equip[part], initialMagnitude);
        };


        //First we need to obtain all equipable pieces
        Dictionary<EquipmentSO, int> equipPieces = InventoryManager.Instance.GetEquipPieces();
        foreach (KeyValuePair<EquipmentSO, int> keyValuePair in equipPieces)
        {
            EquipmentSO inventoryEquipment = keyValuePair.Key;
            if (inventoryEquipment.IsEquipable(this, part))
                equipables.Add(inventoryEquipment, inventoryEquipment.GetStatsMagnitude());
        }

        //Next we have to compare the magnitude growth of each equipment
        foreach (KeyValuePair<EquipmentSO, float> equipableEquipment in equipables)
            if (equip[part] == null || equipableEquipment.Value > equip[part].GetStatsMagnitude())
                Equip(equipableEquipment.Key, part);

        //In case empty is better option
        if (equip[part] != null && equip[part].GetStatsMagnitude() < 0)
            Unequip(part);

    }

    private float GetStatsMagnitude(EquipPartSO part)
    {
        if (equip[part] == null)
            return 0;
        return equip[part].GetStatsMagnitude();
    }

    internal float CompareTotalAtributes(EquipmentSO equipToCompare)
    {
        float maxStatsMagnitude = float.MinValue;

        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipment in equip)
        {
            if (equipToCompare.IsEquipable(this, equipment.Key))
            {
                float statsMagnitude = GetStatsMagnitude(equipment.Key);
                maxStatsMagnitude = statsMagnitude > maxStatsMagnitude ? statsMagnitude : maxStatsMagnitude;
            }
        }
        return maxStatsMagnitude;
    }

    internal void RemoveAll()
    {
        List<EquipPartSO> partsList = new();
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in equip)
            partsList.Add(equipSlot.Key);

        for (int i = 0; i < partsList.Count; i++)
            Unequip(partsList[i]);
    }

    internal void Unequip(EquipPartSO part)
    {
        InventoryManager IM = InventoryManager.Instance;

        EquipmentSO oldEquipment = equip[part];
        IM.Add(oldEquipment, 1);
        equip[part] = null;
    }

    internal EquipmentSO GetEquipment(EquipPartSO part)
    {
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> keyValuePairs in equip)
            if (part == keyValuePairs.Key)
                return keyValuePairs.Value;
        return null;
    }

    internal int GetEquipped(EquipmentSO equipToCount)
    {
        int count = 0;
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipment in equip)
            count += equipment.Value == equipToCount ? 1 : 0;

        return count;
    }
    internal void ChangeEquipment(EquipmentSO equipItem)
    {
        List<EquipPartSO> availableParts = GetPosibleEquipParts(equipItem);
        if (availableParts.Count <= 0)
            availableParts.AddRange(equip.Keys);

        EquipInBestSpace(equipItem, availableParts);
    }

    private void EquipInBestSpace(EquipmentSO equipItem, List<EquipPartSO> availableParts)
    {
        KeyValuePair<float, EquipPartSO> maxIncrement = new(float.MinValue, availableParts[Random.Range(0, availableParts.Count)]);
        float statsMagnitude = equipItem.GetStatsMagnitude();
        foreach (EquipPartSO part in availableParts)
        {
            float oldEquipMagnitude = equip[part] == null ? 0 : equip[part].GetStatsMagnitude();
            float difference = statsMagnitude - oldEquipMagnitude;
            //            Debug.Log($"Follow {Character.Name} {equipItem} {part} : {difference} > {maxIncrement.Key} ? {difference > maxIncrement.Key}");
            if (difference > maxIncrement.Key)
                maxIncrement = new(difference, part);
        }
        Equip(equipItem, maxIncrement.Value);
    }

    private List<EquipPartSO> GetPosibleEquipParts(EquipmentSO equipItem)
    {
        List<EquipPartSO> availableParts = new();
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipment in equip)
            if (equipItem.IsEquipable(this, equipment.Key))
                availableParts.Add(equipment.Key);
        return availableParts;
    }
    #endregion

    #region Job

    internal void SetJob(JobSO newJob, bool reset)
    {
        SaveJob();

        // Set Job
        int indexOfJob = FindJob(newJob);
        if (reset || indexOfJob < 0)
        {
            job.Init(newJob);
            return;
        }
        job = jobDomain[indexOfJob];
    }

    private void SaveJob()
    {
        if (job == null) return;
        int jobToSaveIndex = FindJob(job.SO);

        if (jobToSaveIndex < 0)
            jobDomain.Add(job);
        else
            jobDomain[jobToSaveIndex] = job;
    }

    private int FindJob(JobSO job)
    {
        if (job == null) return -1;
        for (int i = 0; i < jobDomain.Count; i++)
            if (jobDomain[i].SO.name == job.name)
                return i;
        return -1;
    }
    #endregion

    internal void SetMenuSprite(Sprite newSpriteInMenu)
    {
        spriteInMenu = newSpriteInMenu;
    }

    #region Debug

    internal bool Search(Ailment ailmentToSearch)
    {
        foreach (Ailment ailment in ailments)
            if (ailment == ailmentToSearch)
                return true;
        return false;
    }

    internal bool Search(AilmentSO ailmentSO)
    {
        foreach (Ailment ailment in ailments)
            if (ailment.SO == ailmentSO)
                return true;
        return false;
    }

    #endregion
}
