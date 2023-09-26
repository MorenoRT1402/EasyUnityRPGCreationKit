using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleCommands { ATTACK, SKILLS, DEFEND, ITEMS, FLEE, WAIT }

public class FighterStateMachine : MonoBehaviour
{

    public enum TurnState { PROCESSING, ADD_TO_LIST, CHOOSE_ACTION, SELECTING, WAITING, ACTION, DEAD }

    protected BattleStateMachine BSM;
    protected BattleManagerUI BMUI;
    protected PersonajeHandler hero;

    public PersonajeHandler Hero => hero;

    //for the progressBar
    public float ATB = 0f;
    public float MaxATB = 100f;

    //    public float ATB => atb;
    public float MaxAtb => MaxATB;

    //this gameobject
    public Vector2 StartPosition;

    public GameObject Selector;
    public GameObject DamageIndicator;

    //timeforaction stuff
    public bool actionStarted = false;
    public List<GameObject> targetsToAttack;
    protected float animSpeed = 5f;
    protected float moveMarginX;

    public TurnState currentState;
    public bool CanMove = true;
    public bool Confusion = false;
    public bool OnGuard = false;
    public bool Waiting = false;
    public FighterTeam Faction;
    public Dictionary<BattleCommands, bool> BattleCommandsEnable;

    private void Awake()
    {
        BSM = BattleStateMachine.Instance;
        BMUI = BattleManagerUI.Instance;
    }
    protected void Start()
    {
        hero = GetComponent<PersonajeHandler>();
        CanMove = true;
        if (Hero.Stats != null && ATB < MaxATB && !Hero.IsDead())
            ATB = Mathf.Max(Random.Range(0, Hero.Stats.ActualSpeed), 0);

        StartPosition = transform.position;

        currentState = TurnState.PROCESSING;

        BattleCommandsEnable = new Dictionary<BattleCommands, bool>
        {
            {BattleCommands.ATTACK, true},
            {BattleCommands.SKILLS, true},
            {BattleCommands.DEFEND, true},
            {BattleCommands.ITEMS, true},
            {BattleCommands.FLEE, true}
        };
    }

    public void ForceStart()
    {
        Start();

    }

    protected void TurnUpdate()
    {
        BSM = BattleStateMachine.Instance;

        if (Waiting)
            Hero.Stats.WaitSkill.Run(GetComponent<PersonajeHandler>());
        else
        {
            if (BSM.allyInput == BattleStateMachine.AllyGUI.END_OF_BATTLE)
                return;
            if (Hero.IsDead()) currentState = TurnState.DEAD;
        }
    }

    protected void UpdateDeadState()
    {
        if (!Hero.IsDead())
        {
            currentState = TurnState.PROCESSING;
            gameObject.tag = BSM.FIGHTER_DEAD_TAG(Faction, false);
            BSM.UpdateLists();
        }
    }

    internal void SetData(GameObject heroGO)
    {
        //        copyFrom(heroGO);

        hero = GetComponent<PersonajeHandler>();

        PersonajeHandler heroGOHandler = heroGO.GetComponent<PersonajeHandler>();

        hero = heroGOHandler;

    }

    public void CopyFrom(GameObject heroGO)
    {
        PersonajeHandler heroHandler = heroGO.GetComponent<PersonajeHandler>();
        if (heroHandler != null)
        {
            PersonajeHandler thisHandler = GetComponent<PersonajeHandler>();
            thisHandler.SO = heroHandler.SO;
        }
        else
        {
            Debug.LogWarning("GameObject heroGO havent component called PersonajeHandler.");
        }
    }

    protected virtual void UpgradeProgressBar()
    {
        if (Hero != null)
            if (BSM.relativeATB)
                ATB += Time.deltaTime * (Hero.Stats.ActualSpeed - BSM.SpeedRatio[0]) * BSM.battleSpeed * BSM.battleSpeedMult;
            else
                ATB += Mathf.Max(Time.deltaTime * Hero.Stats.ActualSpeed * BSM.battleSpeed, 0.1f);

        //            Debug.Log($"UB {Time.deltaTime} * {Hero.Stats.ActualSpeed} * {BSM.battleSpeed} = {Time.deltaTime * Hero.Stats.ActualSpeed * BSM.battleSpeed}");
    }


    protected IEnumerator TimeForAction()
    {
        StatsHandler stats = Hero.Stats;
        if (OnGuard)
            stats.GuardSkill.Run(GetComponent<PersonajeHandler>());

        PersonajeAnimations anims = PersonajeAnimations.Instance;
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        TurnHandler myAttack = BSM.PerformList[0];

        Hero.UseMP(myAttack.Skill.manaCost);
        Hero.UseStamina(myAttack.Skill.staminaCost);

        for (int t = 0; t < myAttack.AttackersTargets.Count; t++)
        {
            GameObject target = myAttack.AttackersTargets[t];
            if (target != null)
            {

                FighterStateMachine targetFighter = target.GetComponent<FighterStateMachine>();
                DamageIndicator dmgInd = targetFighter.DamageIndicator.GetComponent<DamageIndicator>();

                //Move anim

                if (myAttack.Skill.moveAnimationType == MoveAnimationType.MOVE_TO_TARGET)
                {
                    float targetPositionX = target.transform.position.x;
                    float targetPositionY = target.transform.position.y;
                    Vector2 targetPosition = new(targetPositionX - moveMarginX, targetPositionY);
                    Vector3 targetPosition3D = new(targetPositionX - moveMarginX, targetPositionY, target.transform.position.z);

                    anims.ChangeBlendTreeAnimation(AnimationSheet.WALK_LEFT, gameObject);
                    while (MoveTowardsTarget3D(targetPosition3D)) { yield return null; };
                }

                //wait a bit
                //        yield return new WaitForSeconds(0.5f);

                //do damge
                for (int h = 0; h < myAttack.Skill.hits; h++)
                {
                    if (myAttack.Skill is SpecialActiveSkill sSkill)
                        sSkill.Run(GetComponent<PersonajeHandler>());

                    else if (Damageable(target, myAttack) && myAttack.AttackersTargets != null && myAttack.AttackersTargets.Count > 0)
                    {
                        anims.ChangeBlendTreeAnimation(myAttack.Skill.hitAnimationType, gameObject);

                        yield return new WaitForSeconds(anims.GetCurrentAnimationDuration(gameObject) * 0.9f); //Time to show animation on target
                        anims.PlayAnimation(target.transform.position, myAttack.Skill.animationOnTarget);
                        yield return new WaitForSeconds(anims.GetCurrentAnimationDuration(gameObject) * 0.4f); //Time to come back

                        ResultDamage result = myAttack.CalculateResult(target, true); // Obtener el resultado como objeto
                        dmgInd.ShowDamage(result);
                        if (DebugManager.Instance.keyDebugs)
                            Debug.Log($"{gameObject.name} has chosen {myAttack.Skill.Name} and does {result} damage to {target} {DebugManager.String(result.Critic, "(Critical)", "")}");
                        dmgInd.HideDamage();

                        anims.ChangeBlendTreeAnimation(AnimationSheet.IN_BATTLE, gameObject);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                if (myAttack.Item != null)
                    InventoryManager.Instance.Remove(myAttack.Item, 1);
                target.GetComponent<FighterStateMachine>().Reaction(myAttack);
            }
        }


        //animate back to starposition
        Vector3 firstPosition = StartPosition;
        anims.ChangeBlendTreeAnimation(AnimationSheet.WALK_RIGHT, gameObject);

        while (MoveTowardsTarget3D(firstPosition)) { yield return null; };
        anims.ChangeBlendTreeAnimation(AnimationSheet.IN_BATTLE, gameObject);

        //remove this performer from the list in BSM
        BSM.PerformList.Remove(myAttack);

        //reset BSM -> Wait
        if (BSM.battleState == BattleStateMachine.PerformAction.PERFORM_ACTION) BSM.battleState = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset this fighter state
        if (!myAttack.Reaction) ATB = 0f;
        currentState = TurnState.PROCESSING;

        BSM.CheckDeads();
    }

    private void Reaction(TurnHandler thisTurn)
    {
        if (!thisTurn.Counterable || thisTurn.AttackersGameObject == gameObject) return;
        //Check if have any reaction state/passive
        List<AilmentSO> reactionStates = Hero.Stats.GetReactionStates();
        if (reactionStates.Count <= 0) return;
        //Check if it meets the requirements to be activated
        List<AilmentSO> activableReactions = new();
        for (int i = 0; i < reactionStates.Count; i++)
            if (reactionStates[i].CheckReactionRequirements(thisTurn))
                activableReactions.Add(reactionStates[i]);
        if (activableReactions.Count <= 0) return;
        //Create TurnHandler based on its and put in the first place of PerformList
        for (int i = 0; i < activableReactions.Count; i++)
        {
            FighterStateMachine targetOfCounter = thisTurn.AttackersGameObject.GetComponent<FighterStateMachine>();
            BaseActiveSkill skillOfCounter = activableReactions[i].GetSkillOfCounter(Hero.Stats, thisTurn.Skill);
            TurnHandler reactionTurn = new(this, targetOfCounter, skillOfCounter)
            {
                Counterable = false, //Flag of counter in order to avoid infinite counters
                Reaction = true
            };
            //            Debug.Log(reactionTurn.ToString());
            BSM.PerformList.Insert(0, reactionTurn);
        }
    }

    private bool Damageable(GameObject target, TurnHandler myAttack)
    {
        if (myAttack.AttackersTargets == null) { return false; }
        ResultDamage result = myAttack.CalculateResult(target, false); // Obtain result
        FighterStateMachine targetSM = target.GetComponent<FighterStateMachine>();
        float targetLife = targetSM.Hero.Stats.ActualLife;
        if (targetLife <= 0)
        {
            return targetLife - result.Result > 0; //For revive
        }
        return true;
    }

    protected bool MoveTowardsTarget(Vector2 target)
    {
        Vector2 currentPosition = new(transform.position.x, transform.position.y);
        float speed = animSpeed * Time.deltaTime;
        return target != (currentPosition = Vector2.MoveTowards(currentPosition, target, speed));
    }
    protected bool MoveTowardsTarget3D(Vector3 target)
    {
        float speed = animSpeed * Time.deltaTime;
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, speed));
    }

    public virtual void TakeDamage(float damage)
    {
    }

    protected void SetRandomTarget()
    {
        TurnHandler myAttack = new(){Attacker = Hero.name};
        FighterTeam faction = Faction == FighterTeam.ANY ? GetRandomFaction() : Faction;
        myAttack.Type = GetAttackerType();
        myAttack.AttackersGameObject = gameObject;

        // Choose skill / action
        myAttack.Skill = ChooseSkill(myAttack);

        myAttack.AttackersTargets = BSM.GetRandomTarget(myAttack, myAttack.Type);

        // Add Action
        BSM.CollectActions(myAttack);
    }

    private BaseActiveSkill ChooseSkill(TurnHandler myAttack)
    {
        StatsHandler stats = Hero.Stats;
        List<BaseActiveSkill> availableSkills = GetAvailableSkills(myAttack);
        int num = Random.Range(0, availableSkills.Count);
        BaseActiveSkill skillChoosen = availableSkills[num];
        return availableSkills.Count > 0 ? skillChoosen : stats.WaitSkill;
    }

    private List<BaseActiveSkill> GetAvailableSkills(TurnHandler myAttack)
    {
        StatsHandler stats = Hero.Stats;
        List<BaseActiveSkill> availableSkills = new();
        if (BattleCommandsEnable[BattleCommands.ATTACK])
            availableSkills.Add(stats.BasicAttack);
        if (BattleCommandsEnable[BattleCommands.SKILLS])
            foreach (BaseActiveSkill skill in stats.GetActiveSkills())
                if (skill.IsUsable(BaseActiveSkill.UsableOn.BATTLE) && AvailableTargets(skill, myAttack) && SufficientResources(skill))
                    availableSkills.Add(skill);
        if (BattleCommandsEnable[BattleCommands.DEFEND])
            availableSkills.Add(stats.GuardSkill);
        if (BattleCommandsEnable[BattleCommands.FLEE])
            availableSkills.Add(stats.FleeSkill);
        return availableSkills;
    }

    public bool SufficientResources(BaseActiveSkill skill)
    {
        return Hero.SufficientResources(skill);
    }

    private bool AvailableTargets(BaseActiveSkill skill, TurnHandler myAttack)
    {
        bool ally = myAttack.Type == FighterTeam.ALLY;
        TargetType targetType = skill.targetType;
        BSM.UpdateLists();

        if (targetType == TargetType.DEAD_ALLY || targetType == TargetType.DEAD_ALLIES)
            if (ally)
                return BSM.DeadAllys.Count > 0;
            else
                return BSM.DeadEnemys.Count > 0;
        else if (targetType == TargetType.DEAD_ENEMY || targetType == TargetType.DEAD_ENEMIES)
            if (ally)
                return BSM.DeadEnemys.Count > 0;
            else
                return BSM.DeadAllys.Count > 0;
        return true;
    }

    private FighterTeam GetAttackerType()
    {
        if (Confusion)
        {
            FighterTeam factionConf = Faction == FighterTeam.ANY ? GetRandomFaction() : Faction;
            if (this is AllyStateMachine)
                return factionConf == FighterTeam.ALLY ? FighterTeam.ALLY : FighterTeam.ENEMY;
            else
                return factionConf == FighterTeam.ALLY ? FighterTeam.ENEMY : FighterTeam.ALLY;
        }
        return Faction;
    }

    protected FighterTeam GetRandomFaction()
    {
        int num = Random.Range(0, 2);
        return num == 0 ? FighterTeam.ALLY : FighterTeam.ENEMY;
    }

    protected void Dead()
    {

        currentState = TurnState.DEAD;
        ATB = 0;
        Selector.SetActive(false);
        BMUI.SetDeadPanels();
        //        BSM.RemoveFromLists(gameObject);
        gameObject.tag = BSM.GetTag(gameObject, true);
        //        BSM.UpdateLists();

        //       hero.changeAnimation(AnimationType.DEAD);
        Hero.ChangeBlendTreeAnimation(AnimationSheet.DEAD);
        //reset input
        BSM.allyInput = BattleStateMachine.AllyGUI.ACTIVATE;
        BMUI.DestroyInstances(UIElements.ACTION);
        BMUI.CreateButtons(BSM.EnemysInGame);
        BSM.battleState = BattleStateMachine.PerformAction.CHECK_ALIVE;
    }

    private void Resurrect()
    {
        currentState = TurnState.PROCESSING;
        ATB = 0;
        gameObject.tag = BSM.GetTag(gameObject.tag, false);

        BSM.UpdateLists();

        Hero.ChangeBlendTreeAnimation(AnimationSheet.IN_BATTLE);
        BSM.allyInput = BattleStateMachine.AllyGUI.ACTIVATE;
        BMUI.CreateButtons(BSM.EnemysInGame);
    }

    internal void AilmentTakeDamage()
    {
        Hero.AilmentTakeDamage(this);
    }

    internal void OutOfBattle(float hp, bool leavesCombat)
    {
        Hero.Stats.TakeDamage(Hero.Stats.ActualLife - hp);
        Dead();
        BSM.UpdateLists();
        if (leavesCombat)
            BSM.OutOFBattle(this);
        gameObject.SetActive(!leavesCombat);
    }

    public void ComeBackBattle(float hp, bool leavesCombat)
    {
        Hero.Stats.TakeDamage(Hero.Stats.ActualLife - hp);
        gameObject.SetActive(true);
        Resurrect();
        Hero.Stats.SetDead(false);
        BSM.UpdateLists();
    }

    #region Debug

    internal bool Search(Ailment ailment)
    {
        return Hero.Search(ailment);
    }

    internal bool Search(AilmentSO ailmentSO)
    {
        return Hero.Search(ailmentSO);
    }

    #endregion
}
