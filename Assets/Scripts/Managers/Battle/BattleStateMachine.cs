using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum FighterTeam
{
    ALLY, ENEMY,
    ANY
}

public enum ActionButtonsFunctions
{
    NULL, ATTACK,
    SKILLS,
    DEFEND,
    ITEMS,
    FLEE
}

public enum TargetQuantity { SINGLE, MULTIPLE }

public class BattleStateMachine : Singleton<BattleStateMachine>
{
    private const string PLAYER_TAG = "Player";
    private const string ALLY_TAG = "Ally";
    private const string ENEMY_TAG = "Enemy";
    private const string DEAD_TAG = "Dead";

    public string FIGHTER_DEAD_TAG(FighterTeam fighter, bool dead)
    {
        string fighterTag = "";
        string deadTag = dead ? DEAD_TAG : "";
        switch (fighter)
        {
            case FighterTeam.ALLY:
                fighterTag = ALLY_TAG;
                break;
            case FighterTeam.ENEMY:
                fighterTag = ENEMY_TAG;
                break;
            case FighterTeam.ANY:
                break;
        }
        return fighterTag + deadTag;
    }

    public enum PerformAction
    {
        WAIT,
        TAKE_ACTION, PERFORM_ACTION,
        CHECK_ALIVE,
        WIN, LOSE,
        FREEZE
    }

    public enum BattleInitiative
    {
        PREEMPTIVE, SURPRISE, NORMAL
    }

    #region Variables
    #region Public

    [SerializeField][Range(0, 100)] private float preemptiveProb = 10;
    [SerializeField][Range(0, 100)] private float surpriseProb = 10;
    [SerializeField][Range(0, 100)] private float normalProb = 80;

    [Header("Battle State")]

    public PerformAction battleState;

    public List<TurnHandler> PerformList = new();
    public List<GameObject> AllysInGame = new();
    public List<GameObject> DeadAllys = new();
    private List<GameObject> allys = new();
    public List<GameObject> EnemysInGame = new();
    public List<GameObject> DeadEnemys = new();
    private List<GameObject> enemys = new();
    public List<GameObject> FightersInGame = new();
    private List<GameObject> deadFighters = new();
    private List<GameObject> fighters = new();

    public List<GameObject> Allys => allys;
    public List<GameObject> Enemys => enemys;
    public List<GameObject> Fighters => fighters;

    public enum AllyGUI
    {
        ACTIVATE, WAITING, INPUT1, INPUT2, DONE,
        END_OF_BATTLE
    }

    public AllyGUI allyInput;

    public List<GameObject> HerosToManage;

    [Header("Glosary")]
    public string preemptiveAttackText = "Preemptive Attack!";
    public string ambushText = "Ambush!";

    public string basicAttackActionText = "Attack";
    public string skillsActionText = "Skills";
    public string defendActionText = "Defend";
    public string itemsActionText = "Items";
    public string fleeActionText = "Flee";

    public string enemyFleeText = "The enemy flees";
    public string groupFleeText = "The group flees";
    public string failedFleeText = "Failed escape attempt";
    public string cantFleeText = "You can't flee";

    public string victoryText = "Victory";

    [Header("Parameters")]
    public float battleSpeed = 3;
    public float battleSpeedMult = 0.7f;
    public float CritMult = 1.5f;
    public bool DivideExpForEachAlly = true;
    public float ExpMultForDeadAllies = 0;
    public bool EnemyCanDropMultipleItems = false;
    public bool relativeATB = true;

    #endregion

    #region Private

    private TurnHandler AllyChoice;

    private List<GameObject> actionBtns = new();

    private readonly List<SkillButtonSlot> skillButtonSlots = new();
    private readonly List<ItemSlot> itemButtonSlots = new();

    private int victoryQueue = 0;
    private float[] speedRatio = new float[3];

    private bool canEscape;
    private bool canBeDefeated;

    private BattleManagerUI BMUI;

    #endregion
    #endregion

    #region G & S
    public List<GameObject> ActionBtns => actionBtns;
    public float[] SpeedRatio => speedRatio;
    #endregion

    // Update is called once per frame
    void Update()
    {
        TurnUpdate();
    }

    internal string GetTag(FighterTeam f, bool dead)
    {
        return FIGHTER_DEAD_TAG(f, dead);
    }

    internal FighterTeam getFighterType(string tag)
    {
        return tag switch
        {
            ALLY_TAG => FighterTeam.ALLY,
            ENEMY_TAG => FighterTeam.ENEMY,
            _ => FighterTeam.ANY,
        };
    }

    internal string GetTag(GameObject gameObject, bool isDead)
    {
        string tag = gameObject.tag;
        if(tag == null || tag == "") return "";

        if (tag == ALLY_TAG || tag == ENEMY_TAG)
        {
            return tag + (isDead ? DEAD_TAG : "");
        }
        else
        {
            return "";
        }
    }

    internal string GetTag(string tag, bool isDead)
    {
        Dictionary<string, string> tagMappings = new()
    {
        { ALLY_TAG, ALLY_TAG + (isDead ? DEAD_TAG : "") },
        { ENEMY_TAG, ENEMY_TAG + (isDead ? DEAD_TAG : "") },
        { ALLY_TAG + DEAD_TAG, ALLY_TAG + (isDead ? DEAD_TAG : "") },
        { ENEMY_TAG + DEAD_TAG, ENEMY_TAG + (isDead ? DEAD_TAG : "") }
    };

        if (tagMappings.ContainsKey(tag))
        {
            return tagMappings[tag];
        }

        return tag;
    }

    // Start is called before the first frame update
    void Initialize()
    {
        HerosToManage = new();
        battleState = PerformAction.WAIT;
        UpdateLists();
        speedRatio = GetSpeedRatio();

        allyInput = AllyGUI.ACTIVATE;


        BMUI.CreateButtons(EnemysInGame);
        /*
        setFightersInBattle(getFighters(allys), false); // Used to Blend Tree animations
        setFightersInBattle(getFighters(enemys), true); // Used to Blend Tree animations
        */

        BMUI.SetFightersInBattle(ALLY_TAG, DEAD_TAG, false);
        BMUI.SetFightersInBattle(ENEMY_TAG, DEAD_TAG, true);

        decideInitiative();
    }

    internal void StartBattle(List<GameObject> allies, EnemyGroupSO enemyGroup, DungeonSO actualDungeon, bool canEscape, bool canBeDefeated)
    {

        this.canEscape = canEscape;
        this.canBeDefeated = canBeDefeated;

        BMUI = BattleManagerUI.Instance;
        List<GameObject> groupGO = PartyManager.Instance.GetGameObjects(enemyGroup.FinalMembers, BMUI.heroInBattlePrefab);

        for (int e = 0; e < groupGO.Count; e++)
        {
            //            Debug.Log(groupGO[e].GetInstanceID());
            //            SetESM(groupGO[e]);
            groupGO[e].name = groupGO[e].GetComponent<PersonajeHandler>().Stats.Name + " " + e;
        }

        allyInput = AllyGUI.ACTIVATE;

        BMUI.StartBattle(allies, groupGO, actualDungeon);

        Initialize();
    }

    private float[] GetSpeedRatio()
    {
        float minSpeed = GetMinMaxSpeed(true);
        float maxSpeed = GetMinMaxSpeed(false);
        float diference = maxSpeed - minSpeed;

        return new float[] { minSpeed, maxSpeed, diference };
    }

    private float GetMinMaxSpeed(bool min)
    {
        float speed = 0.1f;
        for (int i = 0; i < FightersInGame.Count; i++)
        {
            float fighterSpeed = FightersInGame[i].GetComponent<PersonajeHandler>().Stats.ActualSpeed;
            if (min && speed > fighterSpeed)
                speed = fighterSpeed;
            else if (!min && fighterSpeed > speed)
                speed = fighterSpeed;
        }
        return speed;
    }

    private void decideInitiative()
    {
        float max = preemptiveProb + surpriseProb + normalProb;
        float num = Random.Range(0, max);

        if (num < preemptiveProb)
            setInitiative(BattleInitiative.PREEMPTIVE);
        else if (num > preemptiveProb && num < preemptiveProb + surpriseProb)
            setInitiative(BattleInitiative.SURPRISE);
        else
            setInitiative(BattleInitiative.NORMAL);
    }

    private void setInitiative(BattleInitiative initiative)
    {
        if (initiative == BattleInitiative.NORMAL) return;

        List<GameObject> fightersBuffed = new List<GameObject>();
        switch (initiative)
        {
            case BattleInitiative.PREEMPTIVE:
                fightersBuffed = AllysInGame;
                BMUI.Notify(preemptiveAttackText);
                break;
            case BattleInitiative.SURPRISE:
                fightersBuffed = EnemysInGame;
                BMUI.Notify(ambushText);
                break;
        }

        for (int i = 0; i < fightersBuffed.Count; i++)
        {
            FighterStateMachine fSM = fightersBuffed[i].GetComponent<AllyStateMachine>();
            if (fSM != null)
                fSM.ATB = fSM.MaxATB;
            fSM = fightersBuffed[i].GetComponent<EnemyStateMachine>();
            if (fSM != null)
                fSM.ATB = fSM.MaxATB;
            //            Debug.Log($"BSM Set Initiative fsm ID {fSM.GetInstanceID()} Name {fSM.Hero.Stats.Name} {fSM.ATB}");
        }
    }

    public void SetESM(GameObject gameObject)
    {
        GameObject selector = gameObject.GetComponent<AllyStateMachine>().Selector;
        GameObject dmgInd = gameObject.GetComponent<AllyStateMachine>().DamageIndicator;

        EnemyStateMachine ESM = gameObject.AddComponent<EnemyStateMachine>();

        ESM.Selector = selector;
        ESM.DamageIndicator = dmgInd;

        Destroy(gameObject.GetComponent<AllyStateMachine>());

        //        Debug.Log($"{gameObject.GetInstanceID()} {gameObject} ESM{gameObject.GetComponent<EnemyStateMachine>()} ASM{gameObject.GetComponent<AllyStateMachine>()}");
    }

    public void BattleComponents(GameObject fighter, bool battle)
    {
        fighter.GetComponent<SpriteRenderer>().enabled = battle;
        fighter.GetComponent<PlayerMovement>().enabled = !battle;
        fighter.GetComponent<FighterStateMachine>().enabled = battle;

        fighter.GetComponent<Animator>().SetBool("InBattle", battle);
    }

    internal void UpdateLists()
    {
        ClearLists();
        ListsAdds();
    }

    public void ClearLists()
    {
        AllysInGame = new List<GameObject>();
        EnemysInGame = new List<GameObject>();
        FightersInGame = new List<GameObject>();

        DeadAllys = new List<GameObject>();
        DeadEnemys = new List<GameObject>();
        deadFighters = new List<GameObject>();

        allys = new List<GameObject>();
        enemys = new List<GameObject>();
        fighters = new List<GameObject>();
    }

    private void ListsAdds()
    {
        //Alives
        AllysInGame = AddByTag(FIGHTER_DEAD_TAG(FighterTeam.ALLY, false));
        EnemysInGame = AddByTag(FIGHTER_DEAD_TAG(FighterTeam.ENEMY, false));

        FightersInGame.AddRange(AllysInGame);
        FightersInGame.AddRange(EnemysInGame);

        //Dead
        DeadAllys = AddByTag(FIGHTER_DEAD_TAG(FighterTeam.ALLY, true));
        DeadEnemys = AddByTag(FIGHTER_DEAD_TAG(FighterTeam.ENEMY, true));

        deadFighters.AddRange(DeadAllys);
        deadFighters.AddRange(DeadEnemys);

        //ALL
        allys.AddRange(AllysInGame);
        allys.AddRange(DeadAllys);

        enemys.AddRange(EnemysInGame);
        enemys.AddRange(DeadEnemys);

        fighters.AddRange(allys);
        fighters.AddRange(enemys);
    }

    private void SetTag(List<GameObject> fighters, string tag)
    {
        for (int i = 0; i < fighters.Count; i++)
            fighters[i].tag = tag;
    }

    private List<GameObject> AddByTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject obj in objectsWithTag)
        {
            //            Debug.Log(obj.name + " " + obj.GetInstanceID());
            FighterStateMachine fsm = obj.GetComponent<FighterStateMachine>();
            if (fsm != null)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    private List<PersonajeHandler> getFighters(List<GameObject> gameObjects)
    {

        List<PersonajeHandler> fighters = new List<PersonajeHandler>();

        foreach (GameObject gameObject in gameObjects)
        {
            //           Debug.Log("Get Fighters ID " + gameObject.GetInstanceID() + " Handler ID " + gameObject.GetComponent<PersonajeHandler>().GetInstanceID() + " FSM ID " + gameObject.GetComponent<FighterStateMachine>().GetInstanceID());

            FighterStateMachine fighterStateMachine = gameObject.GetComponent<FighterStateMachine>();

            //            Debug.Log("Get Fighters (ASM) ID " + fighterStateMachine.gameObject.GetInstanceID() + " Handler ID " + fighterStateMachine.gameObject.GetComponent<PersonajeHandler>().GetInstanceID() + " FSM ID " + fighterStateMachine.gameObject.GetComponent<FighterStateMachine>().GetInstanceID());
            //                        Debug.Log("Get Fighters (ESM) ID " + enemyStateMachine.gameObject.GetInstanceID() + " Handler ID " + enemyStateMachine.gameObject.GetComponent<PersonajeHandler>().GetInstanceID() + " FSM ID " + enemyStateMachine.gameObject.GetComponent<FighterStateMachine>().GetInstanceID());


            if (fighterStateMachine != null)
            {
                fighters.Add(fighterStateMachine.Hero);
            }

        }

        return fighters;
    }

    private void SetFightersInBattle(List<PersonajeHandler> fighters, bool flipX)
    {
        foreach (PersonajeHandler fighter in fighters)
        {
            if (fighter != null)
            {
                if (fighter.IsDead())
                    fighter.ChangeBlendTreeAnimation(AnimationSheet.DEAD);
                else
                    fighter.ChangeBlendTreeAnimation(AnimationSheet.IN_BATTLE);
                PersonajeAnimations.Instance.FlipAnimation(flipX, fighter.gameObject, new Vector2(1, 0));
            }
        }
    }


    private void TurnUpdate()
    {
        switch (battleState)
        {
            case PerformAction.WAIT:
                ApplyUpdateStates();
                if (PerformList.Count > 0)
                {
                    battleState = PerformAction.TAKE_ACTION;
                }
                break;
            case PerformAction.TAKE_ACTION:
                TakeActionState();
                break;
            case PerformAction.PERFORM_ACTION:
                break;
            case PerformAction.CHECK_ALIVE:
                CheckAlive();
                break;
            case PerformAction.WIN:
                Victory();
                break;
            case PerformAction.LOSE:
                BMUI.Notify("Lose");
                if (!canBeDefeated)
                {
                    HealAll(allys);
                    SetHPAll(allys, 1, false);
                }
                break;
            case PerformAction.FREEZE:
                break;
        }

        switch (allyInput)
        {
            case AllyGUI.ACTIVATE:
                if (HerosToManage.Count > 0)
                {
                    HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    AllyChoice ??= new TurnHandler();
                    BMUI.SetActive(UIElements.ACTION, true);
                    BMUI.SetActive(UIElements.BACK_BUTTON, true);
                    CreateActionButtons(HerosToManage[0].GetComponent<FighterStateMachine>());
                    allyInput = AllyGUI.WAITING;
                }
                break;
            case AllyGUI.WAITING:
                break;
            case AllyGUI.INPUT1:
                break;
            case AllyGUI.INPUT2:
                break;
            case AllyGUI.DONE:
                AllyInputDone();
                break;
            case AllyGUI.END_OF_BATTLE:
                break;
        }
    }

    private void SetHPAll(List<GameObject> fighters, int amount, bool percent)
    {
        for (int i = 0; i < fighters.Count; i++)
            fighters[i].GetComponent<PersonajeHandler>().SetHP(amount, percent);
    }

    private void HealAll(List<GameObject> fighters)
    {
        for (int i = 0; i < fighters.Count; i++)
            fighters[i].GetComponent<PersonajeHandler>().Heal();
    }

    private void Victory()
    {
        EndBattle();
        AddJobPoints();
        StartCoroutine(VictoryCoroutine(2f));
        //        battleState = PerformAction.FREEZE;

    }

    private IEnumerator VictoryCoroutine(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        BMUI.Notify(victoryText, 0);
        int expToAdd = (int)getTotalExp(DeadEnemys);
        float moneyDropped = getTotalGold(DeadEnemys);
        BMUI.Victory(expToAdd, moneyDropped);
    }

    private void AddJobPoints()
    {
        if (victoryQueue != 0) return;
        victoryQueue = 1;
        float totalJP = 0;
        for (int i = 0; i < DeadEnemys.Count; i++)
        {
            GameObject enemy = DeadEnemys[i];
            string jPFormula = enemy.GetComponent<PersonajeHandler>().Stats.Character.JobPointsFormula;
            totalJP += NCalcManager.Instance.CalculateObjectToFloat(enemy, jPFormula);
        }
        for (int i = 0; i < AllysInGame.Count; i++)
        {
            AllysInGame[i].GetComponent<PersonajeHandler>().AddJP((int)totalJP);
        }
    }

    public float GetExpToAddValue(float totalExp, FighterStateMachine fighter)
    {
        if (!DivideExpForEachAlly)
            return totalExp;

        int totalAllys = AllysInGame.Count + DeadAllys.Count;

        float indValue = totalExp / totalAllys;
        float expIndToDeads = indValue * ExpMultForDeadAllies;

        if (fighter.Hero.IsDead())
            return expIndToDeads;

        float totalExpToAllives = totalExp - (expIndToDeads * DeadAllys.Count);
        float expIndToAllives = totalExpToAllives / AllysInGame.Count;
        return expIndToAllives;
    }

    private float getTotalGold(List<GameObject> deadEnemys)
    {
        float totalMoney = 0;
        for (int i = 0; i < deadEnemys.Count; i++)
            totalMoney += deadEnemys[i].GetComponent<FighterStateMachine>().Hero.GetMoneyValue();
        return totalMoney;
    }

    private float getTotalExp(List<GameObject> deadEnemys)
    {
        float expTotal = 0;
        for (int i = 0; i < deadEnemys.Count; i++)
            expTotal += deadEnemys[i].GetComponent<FighterStateMachine>().Hero.GetExpValue();
        return expTotal;
    }

    private void ApplyUpdateStates()
    {
        for (int a = 0; a < AllysInGame.Count; a++)
        {
            ApplyUpdateStates(AllysInGame[a]);
        }
        for (int e = 0; e < EnemysInGame.Count; e++)
        {
            ApplyUpdateStates(EnemysInGame[e]);
        }
    }

    private void ApplyUpdateStates(GameObject gameObject)
    {
        FighterStateMachine fighter = gameObject.GetComponent<FighterStateMachine>();

        if (gameObject != null && gameObject.GetComponent<PersonajeHandler>() != null)
            ApplyAilments(gameObject.GetComponent<PersonajeHandler>());

        BMUI.UpdateUIElements(fighter);

    }

    private void ApplyAilments(PersonajeHandler handler)
    {
        List<Ailment> ailments = handler.Stats.Ailments;

        for (int a = 0; a < ailments.Count; a++)
        {
            //                Debug.Log(ailments[a].Name);
            ailments[a].UpdateAilment(handler);
            //                ailments[a].showAnim(fighterStateMachine);
        }
    }

    private void CheckAlive()
    {
        UpdateLists();
        if (AllysInGame.Count <= 0 && EnemysInGame.Count <= 0) return;
        if (AllysInGame.Count < 1)
        {
            battleState = PerformAction.LOSE;

        }
        else if (EnemysInGame.Count < 1)
        {
            battleState = PerformAction.WIN;
        }
        else
        {
            allyInput = AllyGUI.ACTIVATE;
            battleState = PerformAction.WAIT;
        }
    }


    private void TakeActionState()
    {
        GameObject performer = GameObject.Find(PerformList[0].Attacker);
        FighterStateMachine FSM = default;
        if (performer != null)
            FSM = performer.GetComponent<FighterStateMachine>();
        if (performer == null || FSM.Hero.IsDead())
        {
            PerformList.RemoveAt(0);
            battleState = PerformAction.WAIT;
            return;
        }
        List<GameObject> searchList = GetSearchList(PerformList[0]);

        for (int t = 0; t < PerformList[0].AttackersTargets.Count; t++)
        {
            bool found = false;
            for (int i = 0; i < searchList.Count; i++)
            {
                //                Debug.Log($"{PerformList[0].AttackersTargets[t]} == {searchList[i]} ? {PerformList[0].AttackersTargets[t] == searchList[i]}");
                if (PerformList[0].AttackersTargets[t] == searchList[i])
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                PerformList[0].AttackersTargets[t] = GetRandomTarget(PerformList[0], PerformList[0].Type)[0];
        }
        FSM.currentState = FighterStateMachine.TurnState.ACTION;

        battleState = PerformAction.PERFORM_ACTION;
    }

    private List<GameObject> GetSearchList(TurnHandler attack) //Posible targets (include both teams)
    {
        List<GameObject> searchList = new();

        switch (attack.Skill.targetType)
        {
            case TargetType.USER:
                searchList.Add(attack.AttackersGameObject);
                break;
            case TargetType.ALLY:
            case TargetType.ALL_ALLIES:
            case TargetType.ENEMY:
            case TargetType.ALL_ENEMIES:
            case TargetType.ALL:
                searchList.AddRange(FightersInGame);
                break;
            case TargetType.DEAD_ALLY:
            case TargetType.DEAD_ALLIES:
            case TargetType.DEAD_ENEMY:
            case TargetType.DEAD_ENEMIES:
                searchList.AddRange(deadFighters);
                break;
        }

        return searchList;
    }


    public void CollectActions(TurnHandler input)
    {
        PerformList.Add(input);
    }

    internal bool ExistInPerformList(FighterStateMachine fighter)
    {
        foreach (TurnHandler turn in PerformList)
            if (turn.AttackersGameObject == fighter.gameObject)
                return true;
        return false;
    }

    public void SetAttacker(FighterTeam type)
    {
        AllyChoice.Attacker = HerosToManage[0].name;
        AllyChoice.AttackersGameObject = HerosToManage[0];
        AllyChoice.Type = type;
    }

    public void AllyInputDone()
    {
        PerformList.Add(AllyChoice);
        BMUI.SetActive(UIElements.TARGET_SELECT, false);
        BMUI.SetActive(UIElements.BACK_BUTTON, false);

        //clean the attackpanel
        foreach (GameObject actBtn in actionBtns)
        {
            Destroy(actBtn);
        }
        actionBtns.Clear();

        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HerosToManage.RemoveAt(0);
        allyInput = AllyGUI.ACTIVATE;
    }

    internal List<GameObject> GetGroupList(FighterTeam type, TargetType targetType)
    {
        List<GameObject> groupList = new();

        bool ally = type == FighterTeam.ALLY;

        switch (targetType)
        {
            case TargetType.USER:
            case TargetType.ALLY:
            case TargetType.ALL_ALLIES:
                if (!ally)
                    groupList = EnemysInGame;
                else
                    groupList = AllysInGame;
                break;
            case TargetType.DEAD_ALLY:
            case TargetType.DEAD_ALLIES:
                if (!ally)
                    groupList = DeadEnemys;
                else
                    groupList = DeadAllys;
                break;
            case TargetType.ENEMY:
            case TargetType.ALL_ENEMIES:
                if (ally)
                    groupList = EnemysInGame;
                else
                    groupList = AllysInGame;
                break;
            case TargetType.DEAD_ENEMY:
            case TargetType.DEAD_ENEMIES:
                if (!ally)
                    groupList = DeadAllys;
                else
                    groupList = DeadEnemys;
                break;
            case TargetType.ALL:
                groupList = FightersInGame;
                break;
        }
        return groupList;
    }

    public List<GameObject> GetRandomTarget(TurnHandler myAttack, FighterTeam team)
    {
        List<GameObject> targets = new();
        if (myAttack.Skill != null)
        {
            TargetType targetType = myAttack.Skill.targetType;
            GameObject targetToAdd;
            List<GameObject> groupList = GetGroupList(team, targetType);

            switch (targetType)
            {
                case TargetType.USER:
                    targets.Add(myAttack.AttackersGameObject);
                    break;
                case TargetType.ALLY:
                case TargetType.DEAD_ALLY:
                case TargetType.ENEMY:
                case TargetType.DEAD_ENEMY:
                    targetToAdd = getRandomTarget(groupList);
                    targets.Add(targetToAdd);
                    break;
                case TargetType.ALL_ALLIES:
                case TargetType.DEAD_ALLIES:
                case TargetType.ALL_ENEMIES:
                case TargetType.DEAD_ENEMIES:
                case TargetType.ALL:
                    targets = groupList;
                    break;
            }
        }
        return targets;
    }

    public GameObject getRandomTarget(List<GameObject> targetsList)
    {
        if (targetsList.Count > 0)
            return targetsList[Random.Range(0, targetsList.Count)];
        return null;
    }

    void CreateActionButtons(FighterStateMachine fighter)
    {
        if (fighter == null)
            return;

        BMUI.DestroyInstances(UIElements.ACTION);
        actionBtns = new();

        Dictionary<BattleCommands, bool> battleCommandsDict = fighter.BattleCommandsEnable;
        if (battleCommandsDict[BattleCommands.ATTACK])
            CreateActionButton(basicAttackActionText, ActionButtonsFunctions.ATTACK);
        if (battleCommandsDict[BattleCommands.SKILLS])
        {
            PersonajeHandler heroToManage = HerosToManage[0].GetComponent<AllyStateMachine>().Hero;
            List<BaseActiveSkill> heroToManageSkills = heroToManage.GetContextSkills(BaseActiveSkill.UsableOn.BATTLE);
            if (heroToManageSkills.Count > 0)
            {
                CreateActionButton(skillsActionText, ActionButtonsFunctions.SKILLS);
                //            BMUI.fillSkillSpacer(heroToManage);
            }
        }
        if (battleCommandsDict[BattleCommands.DEFEND])
            CreateActionButton(defendActionText, ActionButtonsFunctions.DEFEND);
        if (battleCommandsDict[BattleCommands.ITEMS])
            CreateActionButton(itemsActionText, ActionButtonsFunctions.ITEMS);
        if (battleCommandsDict[BattleCommands.FLEE])
            CreateActionButton(fleeActionText, ActionButtonsFunctions.FLEE);

        BMUI.SetActive(UIElements.ACTION, true);
    }

    private void CreateActionButton(string text, ActionButtonsFunctions func)
    {
        BMUI.CreateActionButtons(text, func);
    }

    public void InputBasicSkill(BaseActiveSkill skill)
    {
        AllyChoice.AttackersTargets = new List<GameObject>();
        BMUI.SetActive(UIElements.ACTION, false);

        AllyActionInput(skill);
    }

    public void Input2(GameObject chosenEnemy)
    { //enemy selection
        SetAttacker(FighterTeam.ALLY);
        AllyChoice.AttackersTargets = new List<GameObject> { chosenEnemy };

        AllyInputDone();
    }

    public void SkillInput(BaseActiveSkill skill)
    {
        AllyChoice.Item = null;
        BMUI.SetActive(UIElements.SKILL, false);

        DestroySkillsOnSkillPanel();

        AllyActionInput(skill);
    }

    internal void ItemInput(ItemSO item)
    {
        BaseActiveSkill skillToInput = item.Effect;
        skillToInput.power = item.Power;

        AllyChoice.Item = item;
        BMUI.SetActive(UIElements.ITEMS, false);

        DestroyItemsOnItemsPanel();

        AllyActionInput(skillToInput);
    }

    internal void AllyActionInput(BaseActiveSkill skillToInput)
    {
        SetAttacker(FighterTeam.ALLY);

        AllyChoice.Skill = skillToInput;
        BMUI.SetTarget(AllyChoice);
        Debug.Log(AllyChoice.Skill);
    }

    internal void Add(SkillButtonSlot skillButton)
    {
        skillButtonSlots.Add(skillButton);
    }

    internal void Add(ItemSlot itemButton)
    {
        itemButtonSlots.Add(itemButton);
    }

    private void DestroySkillsOnSkillPanel()
    {
        foreach (var skillButtonSlot in skillButtonSlots)
        {
            if (skillButtonSlot != null)
                Destroy(skillButtonSlot.gameObject);
        }

        skillButtonSlots.RemoveAll(skillButton => true);
        skillButtonSlots.Clear();
    }

    private void DestroyItemsOnItemsPanel()
    {
        foreach (var itemButtonSlot in itemButtonSlots)
        {
            if (itemButtonSlot != null)
                Destroy(itemButtonSlot.gameObject);
        }

        itemButtonSlots.RemoveAll(skillButton => true);
        itemButtonSlots.Clear();
    }

    public void CheckDeads()
    {
        List<GameObject> fightersTemp = new();
        fightersTemp.AddRange(FightersInGame);

        foreach (GameObject fighter in fightersTemp)
            if (fighter.GetComponent<PersonajeHandler>().IsDead())
                RemoveFromLists(fighter);
        UpdateLists();

        battleState = PerformAction.CHECK_ALIVE;
    }

    public void RemoveFromLists(GameObject fighter)
    {
        if (battleState == PerformAction.PERFORM_ACTION) return;

        List<GameObject> fightersAlive = getFighterType(fighter.tag) == FighterTeam.ALLY ? AllysInGame : EnemysInGame;

        fightersAlive.Remove(fighter);

        for (int i = 0; i < HerosToManage.Count; i++)
        {
            if (HerosToManage[i] == fighter)
                HerosToManage.Remove(fighter);
        }

        for (int i = 0; i < PerformList.Count; i++)
        {
            TurnHandler myAttack = PerformList[i];

            if (myAttack.AttackersGameObject == fighter) //attacker dead
            {
                PerformList.Remove(myAttack);
            }
            if (myAttack.AttackersTargets.Count > 0)
            {
                for (int t = 0; t < myAttack.AttackersTargets.Count; t++)
                {
                    GameObject target = myAttack.AttackersTargets[t];

                    if (target == fighter) // target Dead
                    {
                        FighterStateMachine attacker = myAttack.AttackersGameObject.GetComponent<FighterStateMachine>();
                        if (fightersAlive.Count > 0) // left enemys alive
                        {
                            myAttack.AttackersTargets.Remove(target);
                            TargetType targetType = myAttack.Skill.targetType;
                            GameObject alternativeTarget = attacker.actionStarted ? null : fightersAlive[Random.Range(0, fightersAlive.Count)]; // To avoid hit chain between different enemys

                            if (targetType != TargetType.ALL && targetType != TargetType.ALL_ALLIES && targetType != TargetType.ALL_ENEMIES) // To avoid multi-hit in all team target skills
                                myAttack.AttackersTargets.Add(alternativeTarget);

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }



    internal void OutOFBattle(FighterStateMachine fighterStateMachine)
    {
        RemoveFromList(fighterStateMachine, ref AllysInGame);
        RemoveFromList(fighterStateMachine, ref EnemysInGame);
        RemoveFromList(fighterStateMachine, ref FightersInGame);
        RemoveFromList(fighterStateMachine, ref DeadAllys);
        RemoveFromList(fighterStateMachine, ref DeadEnemys);
        RemoveFromList(fighterStateMachine, ref deadFighters);
        RemoveFromList(fighterStateMachine, ref allys);
        RemoveFromList(fighterStateMachine, ref enemys);
        RemoveFromList(fighterStateMachine, ref fighters);
    }

    private void RemoveFromList(FighterStateMachine fighterStateMachine, ref List<GameObject> battleList)
    {
        if (battleList.Contains(fighterStateMachine.gameObject))
        {
            battleList.Remove(fighterStateMachine.gameObject);
        }
    }

    internal void Flee(FighterStateMachine character)
    {
        if (character is AllyStateMachine)
        {
            if (canEscape)
            {
                BMUI.Notify(groupFleeText);
                InterrupBattle();
            }
            else
                BMUI.Notify(cantFleeText);
        }
        else
            BMUI.Notify(enemyFleeText);
    }

    private void InterrupBattle()
    {
        EndBattle();
        FinishBattle();
    }


    private void EndBattle() //stop battle
    {
        DungeonManager.sceneChanged = false;
        allyInput = AllyGUI.END_OF_BATTLE;
        if (HerosToManage.Count > 0) HerosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
    }

    private void FinishBattle() //battle scene exit
    {
        SetTag(allys, PLAYER_TAG);

        BMUI.SetActive(UIElements.BATTLE_BOX, false);
        BMUI.SetActive(UIElements.INFO_PANEL, false);
        BMUI.SetActive(UIElements.ITEMS, false);
        BMUI.SetActive(UIElements.SKILL, false);
        BMUI.SetActive(UIElements.FINISH_BUTTON, false);
        if (DebugManager.Instance.keyDebugs) Debug.Log("Battle Finished");

        AllyResultPanel.characterAnimationStates.Clear();

        battleState = PerformAction.FREEZE;
        victoryQueue = 0;

        ReturnToMap();
    }

    public void FinishButton()
    {
        if (BMUI.ElementIsActive(UIElements.EXP_AND_GOLD_RESULT_PANEL))
        {
            bool multipleDrop = EnemyCanDropMultipleItems;
            List<PersonajeDropSO> itemsDropped = new();
            foreach (GameObject enemy in DeadEnemys)
            {
                EnemyStateMachine enemySM = enemy.GetComponent<EnemyStateMachine>();
                PersonajeStats stats = enemySM.Hero.Stats.Character;
                if (stats.Drops.Count > 0)
                {
                    if (multipleDrop)
                    {
                        List<PersonajeDropSO> dropsByEnemy = stats.GetRandomDrops();
                        itemsDropped.AddRange(dropsByEnemy);
                    }
                    else
                    {
                        PersonajeDropSO dropByEnemy = stats.GetRandomDrop();
                        itemsDropped.Add(dropByEnemy);
                    }
                }
            }
            List<PersonajeDropSO> itemsDroppedStacked = StackDrops(itemsDropped);
            if (itemsDroppedStacked.Count > 0 && itemsDroppedStacked[0] != null)
            {
                BMUI.SetActive(UIElements.EXP_AND_GOLD_RESULT_PANEL, false);
                BMUI.FillItemsDropSpacer(itemsDroppedStacked);
                BMUI.SetActive(UIElements.ITEMS_DROP_RESULT_PANEL, true);
                //InventoryManager.Instance.AddToInventory(itemsDroppedStacked);
            }
            else FinishBattle();
        }
        else if (BMUI.ElementIsActive(UIElements.ITEMS_DROP_RESULT_PANEL))
        {
            BMUI.SetActive(UIElements.ITEMS_DROP_RESULT_PANEL, false);
            FinishBattle();
        }

        //        allyInput = AllyGUI.ACTIVATE;
    }

    private List<PersonajeDropSO> StackDrops(List<PersonajeDropSO> itemsDropped)
    {
        List<PersonajeDropSO> stackedItems = new List<PersonajeDropSO>();

        foreach (PersonajeDropSO drop in itemsDropped)
        {
            PersonajeDropSO existingDrop = stackedItems.Find(itemDrop => itemDrop != null && itemDrop.item != null && itemDrop.item.Name == drop.item.Name);

            if (existingDrop != null)
            {
                existingDrop.quantity += drop.quantity;
            }
            else
            {
                PersonajeDropSO newDrop = drop.Clone(drop.item, drop.probDrop, drop.quantity);
                stackedItems.Add(newDrop);
            }
        }
        return stackedItems;
    }



    private void ReturnToMap()
    {
        for (int i = 0; i < allys.Count; i++)
            BattleComponents(allys[i], false);
        GameManager.EndBattle();
    }

    #region Debug

    internal bool Search(Ailment ailment)
    {
        for (int i = 0; i < Allys.Count; i++)
            if (Allys[i].GetComponent<FighterStateMachine>().Search(ailment))
                return true;
        return false;
    }

    internal object Search(AilmentSO ailmentSO)
    {
        for (int i = 0; i < Allys.Count; i++)
            if (Allys[i].GetComponent<FighterStateMachine>().Search(ailmentSO))
                return true;
        return false;
    }
    #endregion
}
