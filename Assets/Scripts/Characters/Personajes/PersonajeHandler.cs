using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeHandler : MonoBehaviour
{
    [SerializeField] private PersonajeHandlerSO sO;

    private StatsHandler stats;
    //    private PersonajeStats stats;
    private PersonajeExperiencia personajeExperiencia;
    private PersonajeAffinities affinities;

    private Animator animator;

    private PersonajeAnimations pAnimations;

    //    public PersonajeStats Stats { get; private set; }
    public PersonajeHandlerSO SO
    {
        get { return sO; }
        set { sO = value; }
    }
    public PersonajeStatsSO StatsModel { get { return sO.stats; } }
    public StatsHandler Stats { get { return stats; } }
    public PersonajeAnimations PAnimations => pAnimations;
    public PersonajeExperiencia PEXP => personajeExperiencia;
    public PersonajeAffinities Affinities => affinities;
    public Animator Animator => animator;

    //    public PersonajeAnimations Animations { get {return animations;} }

        internal void SetData(PersonajeStats personajeStats, Job job, Dictionary<EquipPartSO, EquipmentSO> equip, Sprite spriteInMenu, List<Job> jobDomains, PersonajeAffinitiesSO affinitiesSO, Dictionary<Affinity, float>[] resistances, Affinity[] elementalAffinities)
    {
                CommonInit();

//                DebugManager.DebugDict(equip);

        stats = new(personajeStats, job, equip, spriteInMenu, jobDomains);
        affinities = new(affinitiesSO, resistances, elementalAffinities);

        RunComponents();
    }

    public void Init(PersonajeHandlerSO personajeHandlerSO)
    {
        CommonInit();

        sO = personajeHandlerSO;

        if (sO == null) return;

        PersonajeStats statsToHand = new(sO.stats);
        Job jobToHand = new(sO.job);
        stats = new StatsHandler(statsToHand, jobToHand);

        affinities = new PersonajeAffinities(sO.affinities);
        RunComponents();
        //        if (TryGetComponent<Animator>(out var animator)) animator.runtimeAnimatorController = sO.animationSheet;
    }

    private void CommonInit()
    {
        pAnimations = PersonajeAnimations.Instance;
        personajeExperiencia = new PersonajeExperiencia();
    }

    internal void CopyFrom(PersonajeHandler other)
    {
        if (other.pAnimations != null)
            pAnimations = other.PAnimations;
        personajeExperiencia = other.PEXP;

        sO = other.SO;
        stats = other.Stats;

        affinities = other.Affinities;
        animator = other.Animator;

        RunComponents();
    }

    private void RunComponents()
    {
        personajeExperiencia.SetData(this);
        StartAnimator();
    }

    private void StartAnimator()
    {
        animator = gameObject.GetComponent<Animator>();


        if (sO != null)
        {
            ChangeAnimations(sO.animationSheet);
            //            animator.runtimeAnimatorController = sO.animationSheet;
        }
    }

    internal List<BaseActiveSkill> GetContextSkills(BaseActiveSkill.UsableOn context)
    {
        return stats.GetContextSkills(context);
    }

    internal void SetHP(int amount, bool percent)
    {
        stats.SetHP(amount, percent);
    }

    internal void TakeDamage(float damage)
    {
        stats.TakeDamage(damage);
    }

    internal void UseMP(float manaCost)
    {
        stats.UseMP(manaCost);
    }

    internal void UseStamina(float staminaCost)
    {
        stats.UseStamina(staminaCost);
    }

    internal void ChangeBlendTreeAnimation(AnimationSheet animation)
    {
        if (pAnimations != null)
            pAnimations.ChangeBlendTreeAnimation(animation, gameObject);
    }

    internal float GetAffinityMult(List<Affinity> affinities)
    {
        float mult = 1f;
        if (affinities.Count > 0)
            foreach (Affinity affinity in affinities)
                mult *= GetAffinityMult(affinity);
        return mult;
    }

    private float GetAffinityMult(Affinity affinity)
    {
        float mult = 1;
        mult *= stats.GetAffinityMult(affinity);
        mult *= affinities.GetMult(affinity);
        return mult;
    }

    internal bool SufficientResources(BaseActiveSkill skill)
    {
        return stats.ActualMana >= skill.manaCost && stats.ActualStamina >= skill.staminaCost;
    }

    internal bool IsDead()
    {
        return stats.Dead;
    }

    internal void AilmentTakeDamage(FighterStateMachine character)
    {
        stats.AilmentTakeDamage(this);
    }
    internal void Heal()
    {
        Stats.Heal();
    }

    internal void Heal(float life, float mana, float stamina, bool status)
    {
        Stats.Heal(life, mana, stamina, status);
    }

    internal float GetExpValue()
    {
        return stats.GetExpValue(gameObject);
    }

    internal float GetMoneyValue()
    {
        return stats.GetMoneyValue(gameObject);
    }

    internal float GetParcialExpNextLevel()
    {
        return personajeExperiencia.GetParcialExpNextLevel();
    }

    internal void AddExp(int exp)
    {
        personajeExperiencia.AddExp(exp);
    }

    internal void AddJP(int jp)
    {
        personajeExperiencia.AddJP(jp);
    }

    internal void CharacterLevelUp(float nextLevelExp)
    {
        PersonajeStats character = stats.Character;
        stats.CharacterLevelUp(nextLevelExp);
        Dictionary<Stats, string> growthDict = sO.statsGrowth.growthFormulas;
        foreach (KeyValuePair<Stats, string> stat in growthDict)
        {
            //            float statToGrwoth = stats.Get(stat.Key);
            string formula = stat.Value;
            //            Debug.Log($"formula {stat.Key} {formula}");
            if (formula != "")
                Growth(stat.Key, formula);
        }
        //        Debug.Log($"{sO} {sO.skillsLearn}");
        if (sO.skillsLearn != null)
            sO.skillsLearn.Learn(character, character.Level);
    }

    private void Growth(Stats statToGrowth, string formula)
    {
        float valueAdd = NCalcManager.Instance.CalculateObjectToFloat(gameObject, formula);
        stats.Add(statToGrowth, valueAdd, StatsHandler.StatsReference.CHARACTER);
        //       statToGrowth += valueAdd;

    }

    internal void CharacterJobLevelUp(float nextLevelJP)
    {
        Job job = stats.Job;
        JobSO jobSO = sO.job;
        stats.JobLevelUp(nextLevelJP);
        Dictionary<Stats, string> growthDict = jobSO.growthFormulas;
        foreach (KeyValuePair<Stats, string> stat in growthDict)
        {
            string formula = stat.Value;
            JobGrowth(stat.Key, formula);
        }
        if (jobSO.SkillsLearn != null) jobSO.SkillsLearn.Learn(job, job.Level);

    }

    private void JobGrowth(Stats statToGrowth, string formula)
    {
        float valueAdd = NCalcManager.Instance.CalculateObjectToFloat(gameObject, formula);
        stats.Add(statToGrowth, valueAdd, StatsHandler.StatsReference.JOB);
    }

    #region Equip

    internal EquipmentSO GetEquipment(EquipPartSO part)
    {
        return Stats.GetEquipment(part);
    }

    internal void Equip(EquipmentSO equipment, EquipPartSO part)
    {
        Stats.Equip(equipment, part);
    }

    internal void Unequip(EquipPartSO part)
    {
        Stats.Unequip(part);
    }

    internal void Optimize()
    {
        Stats.Optimize();
    }
    internal void ChangeEquipment(EquipmentSO equipItem)
    {
        stats.ChangeEquipment(equipItem);
    }

    internal float CompareTotalAtributes(EquipmentSO equip)
    {
        return Stats.CompareTotalAtributes(equip);
    }

    internal int GetEquipped(EquipmentSO equip)
    {
        return Stats.GetEquipped(equip);
    }
    #endregion
    internal void SetMenuSprite(Sprite newSpriteInMenu)
    {
        if (newSpriteInMenu != null)
            stats.SetMenuSprite(newSpriteInMenu);
    }
    internal void ChangeAnimations(RuntimeAnimatorController newAnimations)
    {
        if (animator != null && newAnimations != null)
            animator.runtimeAnimatorController = newAnimations;
    }

    internal void ModStats(Stats stat, ModScale scale, float amount)
    {
        stats.ModStats(stat, scale, amount);
    }
    internal void ModAffinities(Affinity affinityToModify, ModScale modScale, float amount)
    {
        affinities.Mod(affinityToModify, modScale, amount);
    }

    internal void SetJob(JobSO newJob, bool reset)
    {
        stats.SetJob(newJob, reset);
    }

    internal void ChangeSkills(AddRemove changeOption, List<SkillBase> skills)
    {
        Stats.ChangeSkills(changeOption, skills);
    }

    #region Debug

    internal bool Search(Ailment ailment)
    {
        return stats.Search(ailment);
    }

    internal bool Search(AilmentSO ailmentSO)
    {
        return stats.Search(ailmentSO);
    }
    #endregion
}
