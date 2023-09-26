using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Skills/New Ailment")]
public class AilmentSO : ScriptableObject
{
    public enum CounterSkill { CUSTOM, BASIC_ATTACK, GUARD, FLEE, SKILL_RECEIVED, RANDOM }

    private NCalcManager NCM;


    [Header("Basic Info")]
    [SerializeField] private string ailmentName;
    [SerializeField][TextArea] private string description;
    [SerializeField] Sprite icon;
    [SerializeField] AnimationClip animation;

    [Header("Specific info")]
    [SerializeField][Range(0, 100f)] private float accurate;
    [SerializeField] private AilmentType effect;

    [Header("Conditions to Heal")]

    [SerializeField] private List<float> timeInSeconds;
    [SerializeField] private bool afterTakeDamage;
    [SerializeField][Range(0, 100)] private float healProb;
    [SerializeField] private bool afterBattle;
    [SerializeField] private bool afterDeath;

    [SerializeField] private List<AilmentSO> secundaryAilments;

    #region Getters

    public string Name => ailmentName;
    public float Accurate => accurate;
    public List<AilmentType> Effects;
    public List<AilmentSO> SecundaryAilments => secundaryAilments;
    public bool AfterBattle => afterBattle;
    public bool AfterDeath => afterDeath;
    public List<float> TimeInSeconds => timeInSeconds;

    public bool IsReaction => effect == AilmentType.COUNTER;

    #endregion


    #region EffectVariables

    #region Damage Per Time

    [Header("Damage Per Time")]
    [SerializeField] private string damagePerInterval;
    [SerializeField] private float interval;

    #endregion

    #region Affinity Mod

    [Header("Affinity Mod")]
    [SerializeField] private Affinity affinityToModify;
    [SerializeField] private float weaknessMult;

    #endregion

    #region Stat mod

    [Header("Stat Mod")]
    [SerializeField] private Stats stat;
    [SerializeField] private float statMult;



    #endregion

    #region Cant Move

    [Header("Cant Move")]
    [SerializeField][Range(0, 100)] private float actionProb;

    #endregion

    #region Confusion

    [Header("Confusion")]
    [SerializeField] private FighterTeam faction;

    #endregion

    #region Action Invalid

    [Header("Action Invalid")]
    [SerializeField] private BattleCommands actionToInvalid;

    #endregion

    #region Out of battle

    [Header("Out Of Battle")]
    [SerializeField] private bool hpTo0;
    [SerializeField] private bool leavesCombat;

    #endregion

    #region Counter

    [Header("Counter")]
    [SerializeField] private SkillType typeThatCounter;
    [SerializeField] private CounterSkill counterSkill;
    [SerializeField] private BaseActiveSkill customSkill;
    [SerializeField][Range(0, 100)] private float counterProb;

    #endregion

    #endregion

    #region Getters
    public CounterSkill SkillOfCounter => counterSkill;
    #endregion

    #region Methods

    /* Apply With FighterStateMachine

        public void applyEffect(FighterStateMachine character, ref bool activate, ref float timerTemp)
        {
            NCM = NCalcManager.Instance;

            switch (effect)
            {
                case AilmentType.DAMAGE_PER_TIME:
                    DamagePerTime(character, ref timerTemp);
                    break;
                case AilmentType.AFFINITY_MOD:
                    affinityMod(character, ref activate);
                    break;
                case AilmentType.STATS_MOD:
                    statMod(character, ref activate);
                    break;
                case AilmentType.CANT_MOVE:
                    cantMove(character);
                    break;
                case AilmentType.CONFUSION:
                    confusion(character);
                    break;
                case AilmentType.ACTION_INVALID:
                    actionInvalid(character, false);
                    break;
                case AilmentType.OUT_OF_BATTLE:
                    outOfBattle(character);
                    break;
                default:
                    break;
            }
        }


        private void outOfBattle(FighterStateMachine character)
        {
            float hp = hpTo0 ? 0 : character.Hero.Stats.ActualLife;
            character.OutOfBattle(hp, leavesCombat);
        }

        private void actionInvalid(FighterStateMachine character, bool enable)
        {
            character.BattleCommandsEnable[actionToInvalid] = enable;
        }

        private void confusion(FighterStateMachine character)
        {
            character.Faction = faction;
            character.Confusion = true;
        }

        private void cantMove(FighterStateMachine character)
        {
            if (Random.Range(0, 100f) > actionProb)
            { // Cant move
                character.CanMove = false;
            }
        }

        private void statMod(FighterStateMachine character, ref bool activate)
        {
            StatsHandler characterStats = character.Hero.Stats;
            if (!activate)
            {
                characterStats.ActualMult(stat, statMult);
                activate = true;
            }
        }

        private void affinityMod(FighterStateMachine character, ref bool activate)
        {
            PersonajeAffinities characterAffinities = character.Hero.Affinities;
            if (!activate)
            {
                characterAffinities.mult(affinityToModify, weaknessMult);
                activate = true;
            }
        }

        private void DamagePerTime(FighterStateMachine character, ref float timerTemp)
        {
            DamageIndicator dmgInd = character.DamageIndicator.GetComponent<DamageIndicator>();

            //        Debug.Log($"{timerTemp} >= {interval} ? {timerTemp >= interval}");

            if (timerTemp >= interval)
            {
                timerTemp = 0;
                object damageObj = NCM.calculateObject(character.gameObject, damagePerInterval);
                float tickDamage = Mathf.CeilToInt(NCM.ConvertToFloat(damageObj));
                //                   float damagePerFrame = damagePerSecond * Time.deltaTime;
                character.TakeDamage(tickDamage);
                dmgInd.ShowDamage(tickDamage);
                dmgInd.HideDamage();
            }
        }


        internal void showAnim(FighterStateMachine fighterStateMachine)
        {
            PersonajeAnimations animations = PersonajeAnimations.Instance;

            if (animation != null)
                animations.PlayAnimation(fighterStateMachine.gameObject.transform.position, animation);

            if (secundaryAilments != null) //for ailments with more than 1 effect
                for (int sA = 0; sA < secundaryAilments.Count; sA++)
                    secundaryAilments[sA].showAnim(fighterStateMachine);
        }
        */

    /* Revert With FighterStateMachine

    internal void revertEffect(FighterStateMachine character)
    {
        //        Debug.Log("Revert effect - " + character.hero.Stats.Name + " - " + this.ailmentName);
        NCM = NCalcManager.Instance;

        switch (effect)
        {
            case AilmentType.AFFINITY_MOD:
                affinityRestore(character);
                break;
            case AilmentType.STATS_MOD:
                statRestore(character);
                break;
            case AilmentType.CANT_MOVE:
                character.CanMove = true;
                break;
            case AilmentType.CONFUSION:
                confusionRestore(character);
                break;
            case AilmentType.ACTION_INVALID:
                actionInvalid(character, true);
                break;
            case AilmentType.OUT_OF_BATTLE:
                comeBackBattle(character);
                break;
            default:
                break;
        }

        for (int i = 0; i < secundaryAilments.Count; i++)
            secundaryAilments[i].revertEffect(character);
    }
    */

    /*

 private void comeBackBattle(FighterStateMachine character)
 {
     character.gameObject.SetActive(true);
     float actualLife = character.Hero.Stats.ActualLife;
     character.ComeBackBattle(actualLife, true);
 }

 private void confusionRestore(FighterStateMachine character)
 {
     if (character is AllyStateMachine)
         character.Faction = FighterTeam.ALLY;
     else
         character.Faction = FighterTeam.ENEMY;
     character.Confusion = false;
 }

 private void statRestore(FighterStateMachine character)
 {
     StatsHandler characterStats = character.Hero.Stats;
     float inverseMult = 1 / statMult;
     characterStats.ActualMult(stat, inverseMult);
 }

 private void affinityRestore(FighterStateMachine character)
 {
     PersonajeAffinities characterAffinities = character.Hero.Affinities;
     float inverseMult = 1 / weaknessMult;
     characterAffinities.mult(affinityToModify, inverseMult);

 }
 */

    internal float GetRandomTimeToFinish()
    {
        //        Debug.Log("ASO " + BattleStateMachine.Instance.Search(this));
        if (timeInSeconds.Count > 0)
        {
            float minValue = Mathf.Min(timeInSeconds.ToArray());
            float maxValue = Mathf.Max(timeInSeconds.ToArray());
            return Random.Range(minValue, maxValue);
        }
        return 0;
    }

    public void ApplyEffect(PersonajeHandler character, ref bool activate, ref float timerTemp)
    {
        NCM = NCalcManager.Instance;

        switch (effect)
        {
            case AilmentType.DAMAGE_PER_TIME:
                DamagePerTime(character, ref timerTemp);
                break;
            case AilmentType.AFFINITY_MOD:
                affinityMod(character, ref activate);
                break;
            case AilmentType.STATS_MOD:
                statMod(character, ref activate);
                break;
            case AilmentType.CANT_MOVE:
                cantMove(character);
                break;
            case AilmentType.CONFUSION:
                Confusion(character);
                break;
            case AilmentType.ACTION_INVALID:
                ActionInvalid(character, false);
                break;
            case AilmentType.OUT_OF_BATTLE:
                OutOfBattle(character);
                break;
            default:
                break;
        }
    }

    private void OutOfBattle(PersonajeHandler character)
    {
        if (!character.TryGetComponent<FighterStateMachine>(out var fighter)) return;
        float hp = hpTo0 ? 0 : character.Stats.ActualLife;
        fighter.OutOfBattle(hp, leavesCombat);
    }

    private void ActionInvalid(PersonajeHandler character, bool enable)
    {
        if (character.TryGetComponent<FighterStateMachine>(out var fighter))
            fighter.BattleCommandsEnable[actionToInvalid] = enable;
    }

    private void Confusion(PersonajeHandler character)
    {
        FighterStateMachine fighter = character.GetComponent<FighterStateMachine>();
        if (fighter == null) return;
        fighter.Faction = faction;
        fighter.Confusion = true;
    }

    private void cantMove(PersonajeHandler character)
    {
        FighterStateMachine fighter = character.GetComponent<FighterStateMachine>();
        if (fighter == null) return;

        if (Random.Range(0, 100f) > actionProb)
        { // Cant move
            fighter.CanMove = false;
        }
    }

    private void statMod(PersonajeHandler character, ref bool activate)
    {
        StatsHandler characterStats = character.Stats;
        if (!activate)
        {
            characterStats.ActualMult(stat, statMult);
            activate = true;
        }
    }

    private void affinityMod(PersonajeHandler character, ref bool activate)
    {
        PersonajeAffinities characterAffinities = character.Affinities;
        if (!activate)
        {
            characterAffinities.Mult(affinityToModify, weaknessMult);
            activate = true;
        }
    }

    private void DamagePerTime(PersonajeHandler character, ref float timerTemp)
    {
        FighterStateMachine fighter = character.GetComponent<FighterStateMachine>();
        if (timerTemp >= interval)
        {
            timerTemp = 0;
            object damageObj = NCM.CalculateObject(character.gameObject, damagePerInterval);
            float tickDamage = Mathf.CeilToInt(NCM.ConvertToFloat(damageObj));
            //                   float damagePerFrame = damagePerSecond * Time.deltaTime;
            character.Stats.TakeDamage(tickDamage);

            if (fighter != null)
            {
                DamageIndicator dmgInd = fighter.DamageIndicator.GetComponent<DamageIndicator>();

                dmgInd.ShowDamage(tickDamage);
                dmgInd.HideDamage();
            }
        }
    }

    internal void revertEffect(PersonajeHandler handler)
    {
        //        Debug.Log("Revert effect - " + character.hero.Stats.Name + " - " + this.ailmentName);
        NCM = NCalcManager.Instance;
        StatsHandler stats = handler.Stats;

        switch (effect)
        {
            case AilmentType.AFFINITY_MOD:
                affinityRestore(handler);
                break;
            case AilmentType.STATS_MOD:
                statRestore(handler);
                break;
            case AilmentType.CANT_MOVE:
                handler.GetComponent<FighterStateMachine>().CanMove = true;
                break;
            case AilmentType.CONFUSION:
                confusionRestore(handler);
                break;
            case AilmentType.ACTION_INVALID:
                ActionInvalid(handler, true);
                break;
            case AilmentType.OUT_OF_BATTLE:
                comeBackBattle(handler);
                break;
            default:
                break;
        }

        for (int i = 0; i < secundaryAilments.Count; i++)
            secundaryAilments[i].revertEffect(handler);
    }
    private void comeBackBattle(PersonajeHandler character)
    {
        FighterStateMachine fighter = character.GetComponent<FighterStateMachine>();
        if (fighter == null) return;
        character.gameObject.SetActive(true);
        float actualLife = character.Stats.ActualLife;
        fighter.ComeBackBattle(actualLife, true);
    }

    private void confusionRestore(PersonajeHandler character)
    {
        FighterStateMachine fighter = character.GetComponent<FighterStateMachine>();
        if (fighter == null) return;
        if (fighter is AllyStateMachine)
            fighter.Faction = FighterTeam.ALLY;
        else
            fighter.Faction = FighterTeam.ENEMY;
        fighter.Confusion = false;
    }

    private void statRestore(PersonajeHandler character)
    {
        StatsHandler characterStats = character.Stats;
        float inverseMult = 1 / statMult;
        characterStats.ActualMult(stat, inverseMult);
    }

    private void affinityRestore(PersonajeHandler character)
    {
        PersonajeAffinities characterAffinities = character.Affinities;
        float inverseMult = 1 / weaknessMult;
        characterAffinities.Mult(affinityToModify, inverseMult);

    }

    internal bool TakeDamageCheck()
    {
        if (afterTakeDamage && Random.Range(0, 100f) < healProb)
            return true;
        return false;
    }
    #region Counter

    public List<AilmentSO> GetReactionStates()
    {
        List<AilmentSO> reactionStates = new();

        if (IsReaction)
            reactionStates.Add(this);

        for (int i = 0; i < secundaryAilments.Count; i++)
            if (secundaryAilments[i].IsReaction) reactionStates.Add(secundaryAilments[i]);

        return reactionStates;
    }

    internal BaseActiveSkill GetSkillOfCounter(StatsHandler stats, BaseActiveSkill skillReceived)
    {
        CounterSkill localCounterSkill = counterSkill;
        while (localCounterSkill == CounterSkill.RANDOM)
        {
            CounterSkill[] counterSkillReactionTypes = (CounterSkill[])Enum.GetValues(typeof(CounterSkill));
            int rndmNum = Random.Range(0, counterSkillReactionTypes.Length - 1);
            localCounterSkill = counterSkillReactionTypes[rndmNum];
        }
        return localCounterSkill switch
        {
            CounterSkill.CUSTOM => customSkill,
            CounterSkill.BASIC_ATTACK => stats.BasicAttack,
            CounterSkill.GUARD => stats.GuardSkill,
            CounterSkill.FLEE => stats.FleeSkill,
            CounterSkill.SKILL_RECEIVED => skillReceived,
            _ => customSkill,
        };
    }

    internal bool CheckReactionRequirements(TurnHandler thisTurn)
    {
        if (!(Random.Range(0, 100) < counterProb)) return false;

        if (thisTurn.Skill is DamageActiveSkill skill)
        {
            if (skill.type == typeThatCounter) //Include SkillType.NOTHING
            return true;
            if (skill.type == SkillType.HYBRID && (typeThatCounter == SkillType.PHYSICAL || typeThatCounter == SkillType.MAGICAL))
            return true;
        }

        return false;
    }

    #endregion

    #endregion
}
