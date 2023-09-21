using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class JobData
{
    public string jobPath;
    public string jobName;
    public int jobLevel, jobMaxLevel, jobPoints;
    public float jobLife, jobMana, jobStamina, jobAttack, jobStrength, jobDexterity, jobMagicAttack, jobMind, jobDefense, jobMagicDefense, jobSpeed;
    public float jobCritProb, jobPrecision, jobEvasion;
    public string jobBasicAttackPath;
    public string[] jobSkillsPaths;
    public string jobGuardSkillPath, jobWaitSkillPath, jobFleeSkillPath;
    public string[] jobEquipmentTypeEquipable;
    public List<string> equipPartsPaths;

    public JobData()
    {
    }
    public JobData(Job job)
    {
        SetStats(job);
    }

    public void SetStats(Job job)
    {
        jobPath = GeneralManager.ToPath(job.SO);
        jobName = job.Name;

        jobLevel = job.Level;
        jobMaxLevel = job.MaxLevel;
        jobPoints = job.Exp;

        jobLife = job.Life;
        jobMana = job.Mana;
        jobStamina = job.Stamina;
        jobAttack = job.Attack;
        jobStrength = job.Strength;
        jobDexterity = job.Dexterity;
        jobMagicAttack = job.MagicAttack;
        jobMind = job.Mind;
        jobDefense = job.Defense;
        jobMagicDefense = job.MagicDefense;
        jobSpeed = job.Speed;

        jobCritProb = job.CritProb;
        jobPrecision = job.Precision;
        jobEvasion = job.Evasion;

        jobBasicAttackPath = GeneralManager.ToPath(job.BasicAttack);
        jobSkillsPaths = GeneralManager.ToPath(job.Skills).ToArray();
        jobGuardSkillPath = GeneralManager.ToPath(job.GuardSkill);
        jobWaitSkillPath = GeneralManager.ToPath(job.WaitSkill);
        jobFleeSkillPath = GeneralManager.ToPath(job.FleeSkill);

        jobEquipmentTypeEquipable = job.equipmentTypeEquipable.ToArray();
        equipPartsPaths = GeneralManager.ToPath(job.equipParts);
    }

    internal static List<Job> GetJobs(List<JobData> jobDatas)
    {
        List<Job> jobs = new();
        foreach (JobData jobData in jobDatas)
            jobs.Add(jobData.GetJob());
        return jobs;
    }

    internal Job GetJob()
    {
        JobSO sO = (JobSO)GeneralManager.To(jobPath);
        int[] expArray = new int[3] { jobLevel, jobMaxLevel, jobPoints };
        float[] statsArray = new float[] { jobLife, jobMana, jobStamina, jobAttack, jobStrength, jobDefense, jobMagicAttack, jobMind, jobMagicDefense, jobDexterity, jobSpeed, jobCritProb, jobPrecision, jobEvasion };
        BaseActiveSkill basicAttack = (BaseActiveSkill)GeneralManager.To(jobBasicAttackPath);
        List<SkillBase> skills = GeneralManager.To<SkillBase>(jobSkillsPaths.ToList());
        SpecialActiveSkill[] specialSkillsArray = GetSpecialSkillsArray();
        List<EquipPartSO> equipParts = GeneralManager.To<EquipPartSO>(equipPartsPaths);

        return new(sO, jobName, expArray, statsArray, basicAttack, skills, specialSkillsArray, jobEquipmentTypeEquipable.ToList(), equipParts);
    }

    private SpecialActiveSkill[] GetSpecialSkillsArray()
    {
        SpecialActiveSkill guard = (SpecialActiveSkill)GeneralManager.To(jobGuardSkillPath);
        SpecialActiveSkill wait = (SpecialActiveSkill)GeneralManager.To(jobWaitSkillPath);
        SpecialActiveSkill flee = (SpecialActiveSkill)GeneralManager.To(jobFleeSkillPath);
        SpecialActiveSkill[] specialSkillsArray = new SpecialActiveSkill[] { guard, wait, flee };
        return specialSkillsArray;
    }

    internal static List<JobData> Get(List<Job> jobDomains)
    {
        List<JobData> jobDatas = new();
        foreach (Job job in jobDomains)
            jobDatas.Add(GetData(job));
        return jobDatas;
    }

    private static JobData GetData(Job job)
    {
        JobData jobData = new();
        jobData.SetStats(job);
        return jobData;
    }
}
