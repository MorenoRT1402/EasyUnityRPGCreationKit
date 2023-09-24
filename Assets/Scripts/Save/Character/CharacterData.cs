using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterData
{
    #region Variables
    #region Stats
    #region Character
    public string personajeStatsPath;
    public string characterName;
    public int level, maxLevel, experience;
    Dictionary<Stats, float> statsDict;
    public string basicAttackPath;
    public string[] skillsPaths;
    public string guardSkillPath, waitSkillPath, fleeSkillPath;
    public string[] equipmentTypeEquipable;
    public List<string> equipPartsPaths;
    #endregion
    #region Job
    JobData job;
    #endregion
    public Dictionary<string, string> equipPath;
    public string spriteInMenuPath;
    public List<JobData> jobDomainsDatas;
    #endregion

    #region Affinities
    public string affinitiesSOPath;
    public Dictionary<Affinity, float> actualResistances;
    public Dictionary<Affinity, float> baseResistances;
    public Affinity[] affinities;
    #endregion
    #endregion

    public CharacterData() { }
    public CharacterData(PersonajeHandler character)
    {
        SetHandler(character);
    }

    #region Methods
    #region Setters

    private void SetHandler(PersonajeHandler character)
    {
        SetStats(character.Stats);
        SetAffinities(character.Affinities);
    }

    private void SetAffinities(PersonajeAffinities personajeAffinities)
    {
        affinitiesSOPath = GeneralManager.ToPath(personajeAffinities.SO);
        actualResistances = personajeAffinities.ActualResistance;
        baseResistances = personajeAffinities.Resistances;
        affinities = personajeAffinities.Affinities.ToArray();
    }

    private void SetStats(StatsHandler stats)
    {
        SetStatsCharacter(stats.Character);
        job = new(stats.Job);
        equipPath = GeneralManager.ToPath(stats.EquipDict);
        spriteInMenuPath = GeneralManager.ToPath(stats.SpriteInMenu);
        jobDomainsDatas = JobData.Get(stats.JobDomains);

    }

    private void SetStatsCharacter(PersonajeStats character)
    {
        personajeStatsPath = GeneralManager.ToPath(character.SO);
        characterName = character.Name;

        level = character.Level;
        maxLevel = character.MaxLevel;
        experience = character.Exp;

        SetStatsDict(character);

        basicAttackPath = GeneralManager.ToPath(character.BasicAttack);
        skillsPaths = GeneralManager.ToPath(character.Skills).ToArray();
        guardSkillPath = GeneralManager.ToPath(character.GuardSkill);
        waitSkillPath = GeneralManager.ToPath(character.WaitSkill);
        fleeSkillPath = GeneralManager.ToPath(character.FleeSkill);

        equipmentTypeEquipable = character.equipmentTypeEquipable.ToArray();
        equipPartsPaths = GeneralManager.ToPath(character.equipParts);
    }

    private void SetStatsDict(PersonajeStats character)
    {
        Dictionary<Stats, float> chStats = character.Stats.statsDict;
        statsDict = new();
        foreach (KeyValuePair<Stats, float> pair in chStats)
            statsDict[pair.Key] = pair.Value;
    }
    #endregion

    internal void Load(List<GameObject> party)
    {
        PersonajeHandler handler = ToHandler();
        GameObject characterGO = PartyManager.Instance.CreateCharacterGO(handler);
        UnityEngine.Object.DontDestroyOnLoad(characterGO);

        party.Add(characterGO);
    }

    internal void Load(List<PersonajeHandler> party)
    {
        PersonajeHandler handler = ToHandler();
        party.Add(handler);
    }

    internal PersonajeHandler ToHandler()
    {
        GameObject structGO = PartyManager.Instance.heroPrefab;
        PersonajeHandler handler = structGO.GetComponent<PersonajeHandler>();

        //Stats Handler
        //PersonajeStats
        PersonajeStatsSO personajeStatsSO = (PersonajeStatsSO)GeneralManager.To(personajeStatsPath);
        int[] expArray = new int[3] { level, maxLevel, experience };
        BaseActiveSkill basicAttack = (BaseActiveSkill)GeneralManager.To(basicAttackPath);
        List<SkillBase> skills = GeneralManager.ToSkillBase(skillsPaths.ToList());
        SpecialActiveSkill[] specialSkillsArray = GetSpecialActiveSkills();
        List<EquipPartSO> equipParts = GeneralManager.To<EquipPartSO>(equipPartsPaths);

        PersonajeStats personajeStats = new(personajeStatsSO, characterName, expArray, statsDict, basicAttack, skills, specialSkillsArray, equipmentTypeEquipable.ToList(), equipParts);
        //Job
        Job job = this.job.GetJob();
        //----
        Dictionary<EquipPartSO, EquipmentSO> equip = GeneralManager.ToEquip(equipPath);
        Sprite spriteInMenu = GeneralManager.ToSprite(spriteInMenuPath);
        List<Job> jobDomains = JobData.GetJobs(jobDomainsDatas);
        //Affinities
        PersonajeAffinitiesSO affinitiesSO = (PersonajeAffinitiesSO)GeneralManager.To(affinitiesSOPath);
        Dictionary<Affinity, float>[] resistances = new Dictionary<Affinity, float>[2] { actualResistances, baseResistances };
        handler.SetData(personajeStats, job, equip, spriteInMenu, jobDomains, affinitiesSO, resistances, affinities);
        return handler;
    }

    private SpecialActiveSkill[] GetSpecialActiveSkills()
    {
        SpecialActiveSkill guard = (SpecialActiveSkill)GeneralManager.To(guardSkillPath);
        SpecialActiveSkill wait = (SpecialActiveSkill)GeneralManager.To(waitSkillPath);
        SpecialActiveSkill flee = (SpecialActiveSkill)GeneralManager.To(fleeSkillPath);
        SpecialActiveSkill[] specialSkillsArray = new SpecialActiveSkill[] { guard, wait, flee };
        return specialSkillsArray;
    }

    internal static CharacterData[] GetParty(List<GameObject> partyGameObjects)
    {
        List<PersonajeHandler> handlers = new();

        for (int i = 0; i < partyGameObjects.Count; i++)
            handlers.Add(partyGameObjects[i].GetComponent<PersonajeHandler>());

        return GetParty(handlers);
    }

    internal static CharacterData[] GetParty(List<PersonajeHandler> handlers)
    {
        CharacterData[] characterDatas = new CharacterData[handlers.Count];

        for (int i = 0; i < handlers.Count; i++)
            characterDatas[i] = new CharacterData(handlers[i]);
        return characterDatas;
    }

    internal Sprite GetSprite()
    {
        return GeneralManager.ToSprite(spriteInMenuPath);
    }

    #endregion

}
