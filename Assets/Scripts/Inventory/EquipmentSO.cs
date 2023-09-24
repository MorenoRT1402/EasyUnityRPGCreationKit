using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equip/Create new Equipment")]
public class EquipmentSO : ItemSO
{
    public string equipmentType;
    public List<string> equipUbications;

    public List<string> equipUbicationsDisabled;

    public BaseActiveSkill basicAttack;
    public List<SkillBase> skillsAdded;

    [Header("Stats mod")]
    [SerializeField] private InitialStatsSO statsMod;

    [Header("Elemental mod")]
    [SerializeField] PersonajeAffinitiesSO affinities;

    private BaseStats stats;
    private PersonajeAffinities personajeAffinities;

    //    public Dictionary<Stats, float> StatsDict;
    public Dictionary<Stats, float> StatsWeightDict;

    private void OnEnable() {
        Init();
    }

    private void Init()
    {
        stats = new(statsMod);
        personajeAffinities = new(affinities);
    }

    private void InitStatsWeightDict()
    {
        GeneralManager GM = GeneralManager.Instance;
        StatsWeightDict = new Dictionary<Stats, float>()
        {
            { Stats.HP_MAX, GM.maxHPW },
            { Stats.MP_MAX, GM.maxMPW },
            { Stats.STAMINA_MAX, GM.maxStaminaW },
            { Stats.ATTACK, GM.maxAttackW },
            { Stats.STRENGTH, GM.maxStrengthW },
            { Stats.DEFENSE, GM.maxDefenseW },
            { Stats.MAGIC_ATTACK, GM.maxMagicAttackW },
            { Stats.MIND, GM.maxMindW },
            { Stats.MAGIC_DEFENSE, GM.maxMagicDefenseW },
            { Stats.DEX, GM.maxDexterityW },
            { Stats.SPEED, GM.maxSpeedW },
            { Stats.PRECISION, GM.maxPrecisionW },
            { Stats.EVASION, GM.maxEvasionW },
            { Stats.CRIT_PROB, GM.maxCritProbW }
        };
    }

    internal float Get(Stats stat)
    {
        float sum = 0;

        stats ??= new(statsMod);

        sum += stats.Get(stat);
        sum += SkillsGet(stat);

        return sum;
    }

    private float SkillsGet(Stats stat)
    {
        float sum = 0;
        if (skillsAdded.Count <= 0) return 0;
        for (int i = 0; i < skillsAdded.Count; i++)
            if (skillsAdded[i] is BasePassiveSkill passiveSkill)
                sum += passiveSkill.Get(stat, ModScale.SUM);
        return sum;
    }

    public float SkillsMultGet(Stats stat)
    {
        float mult = 1;
        if (skillsAdded.Count <= 0) return mult;
        for (int i = 0; i < skillsAdded.Count; i++)
            if (skillsAdded[i] is BasePassiveSkill passiveSkill)
                mult *= passiveSkill.Get(stat, ModScale.MULT);
        return mult;
    }

    internal Dictionary<Stats, float> GetChangingStats(EquipmentSO other, bool oldFirst)
    {
        int mult = oldFirst ? -1 : 1;
        Dictionary<Stats, float> otherStatsMod;
        if (other != null) otherStatsMod = other.GetStatsModded();//throw new Exception("Other equipment is null");
        else otherStatsMod = SetStatsToCero();

        Dictionary<Stats, float> thisStatsMod = GetStatsModded();


        Dictionary<Stats, float> totalStatsModded = new();

        foreach (KeyValuePair<Stats, float> statMod in thisStatsMod)
        {
            if (otherStatsMod.ContainsKey(statMod.Key))
                totalStatsModded.Add(statMod.Key, otherStatsMod[statMod.Key] - statMod.Value * (-1 * mult));
            else
                totalStatsModded.Add(statMod.Key, mult * statMod.Value);
        }

        return totalStatsModded;

    }

    private Dictionary<Stats, float> SetStatsToCero()
    {
        Dictionary<Stats, float> statsToMod = GetStatsModded();
        Dictionary<Stats, float> statsModded = new();


        foreach (KeyValuePair<Stats, float> statMod in statsToMod)
            if (statMod.Value != 0f) statsModded.Add(statMod.Key, 0);

        return statsModded;
    }

    private Dictionary<Stats, float> GetStatsModded() //return stats which increments or decrements
    {
        Dictionary<Stats, float> statsModded = new();
        Dictionary<Stats, float> statsDict = stats.GetStats();

        foreach (KeyValuePair<Stats, float> statMod in statsDict)
            if (statMod.Value != 0f) statsModded.Add(statMod.Key, statMod.Value);

        return statsModded;
    }

    internal float GetStatsMagnitude()
    {
        float count = 0;

        InitStatsWeightDict();
        Dictionary<Stats, float> statsDict = stats.GetStats();

        foreach (KeyValuePair<Stats, float> stat in statsDict)
            count += stat.Value != 0 ? stat.Value / StatsWeightDict[stat.Key] : 0;

        return count;
    }

    internal bool IsEquipable(StatsHandler character, EquipPartSO part)
    {
        List<string> equipType = character.equipmentTypeEquipable;
        for (int i = 0; i < equipType.Count; i++)
            for (int j = 0; j < part.equipAvailables.Count; j++)
            {
                //                Debug.Log($"{this.Name} : {equipmentType} == {equipType[i]} ? {equipmentType == equipType[i]} && {part.equipAvailables[j]} == {equipmentType} ? {part.equipAvailables[j] == equipmentType} = {equipmentType == equipType[i] && part.equipAvailables[j] == equipmentType}");
                if (equipmentType == equipType[i] && part.equipAvailables[j] == equipmentType)
                    return true;
            }
        return false;
    }

    internal List<Affinity> GetAffinities()
    {
        List<Affinity> affinitiesList = new();
        if (affinities != null)
            affinitiesList.AddRange(affinities.Affinities);
        return affinitiesList;
    }

    internal float GetResistanceMult(Affinity affinity)
    {
        if (personajeAffinities == null)
            Init();
        return personajeAffinities.GetMult(affinity);
    }
}
