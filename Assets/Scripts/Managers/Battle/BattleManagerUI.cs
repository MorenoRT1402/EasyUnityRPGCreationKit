using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIElements
{
    ACTION, TARGET_SELECT, ALLY_PARTY, SKILL, SKILL_SPACER,
    ITEMS,
    BACK_BUTTON,
    INFO_PANEL,
    RESULT_PANEL,
    BATTLE_BOX,
    FINISH_BUTTON,
    EXP_AND_GOLD_RESULT_PANEL,
    ITEMS_DROP_RESULT_PANEL,
    ITEM_SPACER
}

public class BattleManagerUI : Singleton<BattleManagerUI>
{
    [Header("MainPanel")]
    [SerializeField] private GameObject battleBox;

    [Header("InfoPanel")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoPanelText;
    [SerializeField] private float notifyTime = 2;

    [Header("ActionPanels")]
    [SerializeField] private GameObject ActionPanel;
    [SerializeField] private Transform actionSpacer;
    [SerializeField] private GameObject actionButton;

    [Header("TargetsPanel")]
    [SerializeField] private GameObject TargetSelectPanel;
    [SerializeField] private GameObject TargetButton;
    [SerializeField] private Transform TargetsSlotSpacer;

    [Header("AllysPanel")]
    [SerializeField] private GameObject AllyPanel;
    [SerializeField] private GameObject AllyPartyPanel;

    [Header("SkillPanel")]
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private SkillButtonSlot skillButtonPrefab;
    [SerializeField] private GameObject skillButtonParent;
    [SerializeField] private TextMeshProUGUI skillDescriptionTMP;
    [SerializeField] private TextMeshProUGUI skillAllyNameTMP;
    [SerializeField] private TextMeshProUGUI skillAllyManaTMP;
    [SerializeField] private TextMeshProUGUI skillAllyStaminaTMP;
    [SerializeField] private TextMeshProUGUI skillManaCostTMP;
    [SerializeField] private TextMeshProUGUI skillStaminaCostTMP;

    [Header("ItemPanel")]
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private ItemSlot itemButtonPrefab;
    [SerializeField] private GameObject itemButtonParent;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;

    [Header("Result Panel")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Transform resultAllyPanelSpacer;
    [SerializeField] private GameObject expAndGoldPanel;
    [SerializeField] private GameObject allyResultPanelPrefab;
    [SerializeField] private bool showMoney = true;
    [SerializeField] private GameObject moneyPanel;
    [SerializeField] private TextMeshProUGUI moneyTMP;
    [SerializeField] private TextMeshProUGUI expTMP;
    [SerializeField] private GameObject itemsDropResultPanel;
    [SerializeField] private Transform itemsDropSpacer;
    [SerializeField] private GameObject itemDropPrefab;

    [Header("Positions")]
    [SerializeField] public GameObject heroInBattlePrefab;
    [SerializeField] private GameObject bgAllyPartyParent;
    [SerializeField] private List<Transform> alliesPositions;
    [SerializeField] private GameObject bgEnemyPartyParent;
    [SerializeField] private List<Transform> enemiesPositions;

    [Header("Other")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Image background;

    private BattleStateMachine BSM;
    private bool notifying;


    internal void CreateButtons(List<GameObject> Fighters)
    {
        // Clear existing buttons
        foreach (Transform child in TargetsSlotSpacer)
        {
            Destroy(child.gameObject);
        }

        //Create buttons
        foreach (GameObject fighter in Fighters)
        {
            //            GameObject newButton = Instantiate(EnemyButton) as GameObject;
            GameObject newButton = Instantiate(TargetButton, TargetsSlotSpacer);
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            FighterStateMachine currentEnemy = fighter.GetComponent<FighterStateMachine>();

            TextMeshProUGUI buttonText = button.TargetName;

            if (currentEnemy.Hero != null)
            {
                buttonText.text = currentEnemy.Hero.Stats.Name;
            }

            button.EnemyPrefab = fighter;

            //            newButton.transform.SetParent(EnemiesSlotSpacer, false);
        }
    }

    public void UpdateUI(SkillButtonSlot slot, PersonajeHandler ally, BaseActiveSkill skill)
    {
        slot.UpdateUI(ally, skill, skillDescriptionTMP, skillAllyNameTMP, skillAllyManaTMP, skillAllyStaminaTMP, skillManaCostTMP, skillStaminaCostTMP);
    }

    public void FillSkillSpacer(PersonajeHandler hero)
    {
        skillDescriptionTMP.text = "";

        DestroyInstances(UIElements.SKILL_SPACER);
        SkillButtonSlot.SetAlly(hero);
        List<SkillBase> heroToManageSkills = hero.Stats.Skills;
        foreach (SkillBase skill in heroToManageSkills)
        {
            if (skill is BaseActiveSkill activeSkill)
            {
                SkillButtonSlot skillButton = Instantiate(skillButtonPrefab, skillButtonParent.transform);
                TextMeshProUGUI skillButtonText = skillButton.NameTMP;
                skillButton.SetSkill(activeSkill);
                skillButtonText.text = activeSkill.Name;
                BattleStateMachine.Instance.Add(skillButton);
            }
        }
    }

    internal void UpdateSkillPanelUI(PersonajeHandler ally, BaseActiveSkill skill)
    {

        skillDescriptionTMP.text = skill.Description;
        skillAllyNameTMP.text = ally.Stats.Name;
        skillAllyManaTMP.text = ally.Stats.ActualMana.ToString();
        skillAllyStaminaTMP.text = ally.Stats.ActualStamina.ToString();
        skillManaCostTMP.text = (skill.manaCost * -1).ToString();
        skillStaminaCostTMP.text = (skill.staminaCost * -1).ToString();

    }

    private void FillItemSpacer(Dictionary<ItemSO, int> items)
    {
        itemDescriptionTMP.text = "";

        DestroyInstances(UIElements.ITEM_SPACER);
        if (itemButtonParent != null)
            foreach (KeyValuePair<ItemSO, int> itemSlot in items)
            {
                ItemSlot inventoryItemSlot = Instantiate(itemButtonPrefab, itemButtonParent.transform);
                inventoryItemSlot.SetItem(itemSlot.Key);
                inventoryItemSlot.SetAmount(itemSlot.Value);
                inventoryItemSlot.UpdateUI(ItemSlot.Context.BATTLE);
                BattleStateMachine.Instance.Add(inventoryItemSlot);
            }

    }

    internal void UpdateItemPanelUI(ItemSO item)
    {
        itemDescriptionTMP.text = item.Description;
    }

    public void BackButton()
    {
        SetActive(UIElements.ACTION, true);
        SetActive(UIElements.TARGET_SELECT, false);
        SetActive(UIElements.SKILL, false);
        SetActive(UIElements.ITEMS, false);
    }

    public void SetActive(UIElements element, bool enabled)
    {
        BattleStateMachine BSM = BattleStateMachine.Instance;
        InventoryManager IM = InventoryManager.Instance;
        switch (element)
        {
            case UIElements.BATTLE_BOX:
                battleBox.SetActive(enabled);
                break;
            case UIElements.INFO_PANEL:
                infoPanel.SetActive(enabled);
                break;
            case UIElements.ACTION:
                if (ActionPanel != null)
                    ActionPanel.SetActive(enabled);
                break;
            case UIElements.TARGET_SELECT:
                TargetSelectPanel.SetActive(enabled);
                break;
            case UIElements.SKILL:
                if (BSM.HerosToManage.Count > 0)
                    FillSkillSpacer(BSM.HerosToManage[0].GetComponent<AllyStateMachine>().Hero);
                if (skillPanel != null) skillPanel.SetActive(enabled);
                break;
            case UIElements.SKILL_SPACER:
                //            fillSkillSpacer(BSM.HerosToManage[0].GetComponent<AllyStateMachine>().hero);
                skillButtonParent.SetActive(enabled);
                break;
            case UIElements.ITEMS:
                if (IM.items.Count > 0)
                    FillItemSpacer(IM.items);
                itemPanel.SetActive(enabled);
                break;
            case UIElements.ITEM_SPACER:
                itemButtonParent.SetActive(enabled);
                break;
            case UIElements.RESULT_PANEL:
                resultPanel.SetActive(enabled);
                break;
            case UIElements.EXP_AND_GOLD_RESULT_PANEL:
                expAndGoldPanel.SetActive(enabled);
                break;
            case UIElements.ITEMS_DROP_RESULT_PANEL:
                itemsDropResultPanel.SetActive(enabled);
                break;
            case UIElements.BACK_BUTTON:
                backButton.gameObject.SetActive(enabled);
                break;
            case UIElements.FINISH_BUTTON:
                finishButton.gameObject.SetActive(enabled);
                break;
        }
    }

    internal bool ElementIsActive(UIElements element)
    {
        switch (element)
        {
            case UIElements.BATTLE_BOX:
                return battleBox.activeSelf;
            case UIElements.INFO_PANEL:
                return infoPanel.activeSelf;
            case UIElements.ACTION:
                return ActionPanel.activeSelf;
            case UIElements.TARGET_SELECT:
                return TargetSelectPanel.activeSelf;
            case UIElements.SKILL:
                return skillPanel.activeSelf;
            case UIElements.SKILL_SPACER:
                return skillButtonParent.activeSelf;
            case UIElements.RESULT_PANEL:
                return resultPanel.activeSelf;
            case UIElements.EXP_AND_GOLD_RESULT_PANEL:
                return expAndGoldPanel.activeSelf;
            case UIElements.ITEMS_DROP_RESULT_PANEL:
                return itemsDropResultPanel.activeSelf;
            case UIElements.BACK_BUTTON:
                return backButton.gameObject.activeSelf;
            case UIElements.FINISH_BUTTON:
                return finishButton.gameObject.activeSelf;
        }
        return false;
    }

    public void DestroyInstances(UIElements panel)
    {
        switch (panel)
        {
            case UIElements.ACTION:
                DestroyChildren(ActionPanel);
                break;
            case UIElements.TARGET_SELECT:
                DestroyChildren(TargetSelectPanel);
                break;
            case UIElements.ALLY_PARTY:
                DestroyChildren(AllyPartyPanel);
                break;
            case UIElements.SKILL_SPACER:
                DestroyChildren(skillButtonParent);
                break;
            case UIElements.ITEM_SPACER:
                DestroyChildren(itemButtonParent);
                break;
        }
    }

    private void DestroyChildren(GameObject parent)
    {
        if (parent == null) return;
        int childCount = parent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    public void CreateAllyPanel(AllyStateMachine ally)
    {
        AllyPanel = Instantiate(AllyPanel, AllyPartyPanel.transform);

        ally.PanelStats = AllyPanel.GetComponent<AllyPanelStats>();

        UpdateAllyPanel(ally);

        ally.SetATBBar(ally.PanelStats.atbBar);
    }

    public void UpdateAllyPanel(AllyStateMachine ally)
    {
        PersonajeHandler hero = ally.Hero;
        AllyPanelStats panelStats = ally.PanelStats;

        //        Debug.Log("Update AllyPanel ATB " + ally.ATB);

        if (hero != null && hero.Stats != null)
        {
            //            Debug.Log("Update Ally Panel " + hero.Stats);

            panelStats.heroName.text = hero.Stats.Name;
            panelStats.heroHP.text = $"{(int)hero.Stats.ActualLife}/{(int)hero.Stats.MaxLife}";
            panelStats.heroMP.text = $"{(int)hero.Stats.ActualMana}/{(int)hero.Stats.MaxMana}";
            panelStats.heroStamina.text = $"{(int)hero.Stats.ActualStamina}/{(int)hero.Stats.MaxStamina}";
            if (hero.IsDead()) ally.ATB = 0;
            UpdateATBBar(ally);
        }
    }

    internal void UpdateUIElements(FighterStateMachine fighter) //Update graphics relative to fighters
    {
        if (fighter is AllyStateMachine ally)
            UpdateAllyPanel(ally);
        /*else
        Debug.Log ("Update HP Bar of enemy for example");*/

        if (fighter.Hero.IsDead())
            PersonajeAnimations.Instance.ChangeBlendTreeAnimation(AnimationSheet.DEAD, fighter.gameObject);
    }

    private void UpdateATBBar(AllyStateMachine ally)
    {
        if (ally.ATBBar != null)
        {
            float calc_atb = ally.ATB / ally.MaxAtb;
            ally.ATBBar.fillAmount = Mathf.Clamp(calc_atb, 0, 1);
        }
    }

    internal void SetInterfaceElements(AllyStateMachine ally)
    {
        CreateAllyPanel(ally);
        ally.Selector.SetActive(false);
        ally.DamageIndicator.SetActive(false);
    }

    internal void UpdateAllyPanel(List<GameObject> allys, bool destroy)
    {
        switch (destroy)
        {
            case true:
                //        DestroyInstances(UIPanels.ALLY_PARTY);
                foreach (GameObject ally in allys)
                {
                    AllyStateMachine allySM = ally.GetComponent<AllyStateMachine>();
                    UpdateAllyPanel(allySM);
                }
                break;
            case false:
                DestroyInstances(UIElements.ALLY_PARTY);
                foreach (GameObject ally in allys)
                {
                    AllyStateMachine allySM = ally.GetComponent<AllyStateMachine>();
                    CreateAllyPanel(allySM);
                }

                break;
        }
    }

    internal void SetTarget(TurnHandler allyChoice)
    {
        allyChoice.AttackersTargets = new List<GameObject>(1);
        BSM = BattleStateMachine.Instance;
        BSM.UpdateLists();

        switch (allyChoice.Skill.targetType)
        {
            case TargetType.USER:
                allyChoice.AttackersTargets.Add(allyChoice.AttackersGameObject);
                BSM.AllyInputDone();
                break;
            case TargetType.ALLY:
                CreateButtons(BSM.AllysInGame);
                SetActive(UIElements.TARGET_SELECT, true);
                break;
            case TargetType.ALL_ALLIES:
                allyChoice.AttackersTargets.AddRange(GameObject.FindGameObjectsWithTag(BSM.FIGHTER_DEAD_TAG(FighterTeam.ALLY, false)));
                BSM.AllyInputDone();
                break;
            case TargetType.DEAD_ALLY:
                CreateButtons(BSM.DeadAllys);
                SetActive(UIElements.TARGET_SELECT, true);
                break;
            case TargetType.DEAD_ALLIES:
                allyChoice.AttackersTargets.AddRange(BSM.DeadAllys);
                BSM.AllyInputDone();
                break;
            case TargetType.ENEMY:
                CreateButtons(BSM.EnemysInGame);
                SetActive(UIElements.TARGET_SELECT, true);
                break;
            case TargetType.ALL_ENEMIES:
                allyChoice.AttackersTargets.AddRange(GameObject.FindGameObjectsWithTag(BSM.FIGHTER_DEAD_TAG(FighterTeam.ENEMY, false)));
                BSM.AllyInputDone();
                break;
            case TargetType.DEAD_ENEMY:
                CreateButtons(BSM.DeadEnemys);
                SetActive(UIElements.TARGET_SELECT, true);
                break;
            case TargetType.DEAD_ENEMIES:
                allyChoice.AttackersTargets.AddRange(BSM.DeadEnemys);
                BSM.AllyInputDone();
                break;
        }
    }

    internal void SetDeadPanels()
    {
        SetActive(UIElements.ACTION, false);
        SetActive(UIElements.SKILL, false);
        SetActive(UIElements.ITEMS, false);
        SetActive(UIElements.TARGET_SELECT, false);
    }



    internal void CreateActionButtons(string text, ActionButtonsFunctions func)
    {
        GameObject targetButton = Instantiate(actionButton, actionSpacer);
        //        GameObject targetButton = Instantiate(actionButton) as GameObject;
        TextMeshProUGUI targetButtonText = targetButton.transform.Find("TMP").gameObject.GetComponent<TextMeshProUGUI>();
        targetButtonText.text = text;
        AddActionListener(targetButton, func);

        //        targetButton.transform.SetParent(actionSpacer, false);
        BSM.ActionBtns.Add(targetButton);
    }

    private void AddActionListener(GameObject button, ActionButtonsFunctions func)
    {
        BSM = BattleStateMachine.Instance;
        StatsHandler stats = BSM.HerosToManage[0].GetComponent<AllyStateMachine>().Hero.Stats;
        switch (func)
        {
            case ActionButtonsFunctions.ATTACK:
                button.GetComponent<Button>().onClick.AddListener(() => InputBasicSkill(stats.BasicAttack));
                break;
            case ActionButtonsFunctions.SKILLS:
                button.GetComponent<Button>().onClick.AddListener(() => SetActive(UIElements.SKILL, true));
                break;
            case ActionButtonsFunctions.DEFEND:
                button.GetComponent<Button>().onClick.AddListener(() => InputBasicSkill(stats.GuardSkill));
                break;
            case ActionButtonsFunctions.ITEMS:
                button.GetComponent<Button>().onClick.AddListener(() => SetActive(UIElements.ITEMS, true));
                break;
            case ActionButtonsFunctions.FLEE:
                button.GetComponent<Button>().onClick.AddListener(() => InputBasicSkill(stats.FleeSkill));
                break;
            case ActionButtonsFunctions.NULL:
                break;
        }
    }

    private void InputBasicSkill(BaseActiveSkill skill)
    {
        BSM.InputBasicSkill(skill);
    }

    internal void StartBattle(List<GameObject> allies, List<GameObject> enemys, DungeonSO actualDungeon)
    {
        if (actualDungeon.BattleBackground != null) background.sprite = actualDungeon.BattleBackground;

        SetPositions(allies, enemys);

        SetActive(UIElements.BATTLE_BOX, true);
        SetActive(UIElements.INFO_PANEL, false);
        SetActive(UIElements.ACTION, false);
        SetActive(UIElements.TARGET_SELECT, false);
        SetActive(UIElements.SKILL, false);
        SetActive(UIElements.ITEMS, false);
        SetActive(UIElements.BACK_BUTTON, false);
        SetActive(UIElements.RESULT_PANEL, false);
        SetActive(UIElements.ITEMS_DROP_RESULT_PANEL, false);
        SetActive(UIElements.FINISH_BUTTON, false);

        for (int i = 0; i < allies.Count; i++)
            SetUpASM(allies[i]);
    }

    private void SetUpASM(GameObject ally)
    {
        AllyStateMachine allyASM = ally.GetComponent<AllyStateMachine>();

        if (allyASM.Hero.Stats != null)
            allyASM.ATB = UnityEngine.Random.Range(0, allyASM.Hero.Stats.ActualSpeed);

        allyASM.StartPosition = transform.position;

        allyASM.currentState = FighterStateMachine.TurnState.PROCESSING;

        SetInterfaceElements(allyASM);

        allyASM.StartPosition = ally.transform.position;


    }

    private void SetPositions(List<GameObject> allies, List<GameObject> enemys)
    {
        BSM = BattleStateMachine.Instance;

        UpdatePositions(bgAllyPartyParent, allies, alliesPositions, FighterTeam.ALLY);
        UpdatePositionsWithPrefab(bgEnemyPartyParent, enemys, enemiesPositions, FighterTeam.ENEMY);

        /*
                for (int i = 0; i < allies.Count; i++)
                    Destroy(allies[i]);
                //            allies[i].SetActive(false);
                */

       for (int i = 0; i < enemys.Count; i++)
            Destroy(enemys[i]);

        //            enemys[i].SetActive(false);

    }

    private void UpdatePositions(GameObject bgGroupParent, List<GameObject> fighters, List<Transform> positions, FighterTeam ft)
    {
        DestroyChildren(bgGroupParent);

        for (int i = 0; i < fighters.Count; i++)
        {
            //            fighters[i].transform.SetParent(bgGroupParent.transform, false);
            FighterStateMachine fsm = fighters[i].GetComponent<FighterStateMachine>();
            fsm.ForceStart();
            BSM.BattleComponents(fighters[i], true);
            fighters[i].transform.position = positions[i].transform.position;
            //            fsm.StartPosition = fsm.transform.position;
            fighters[i].tag = BSM.FIGHTER_DEAD_TAG(ft, fsm.Hero.IsDead());
        }
    }

    private void UpdatePositionsWithPrefab(GameObject bgGroupParent, List<GameObject> fighters, List<Transform> positions, FighterTeam ft)
    {
        //        List<Transform> positionsList = ft == FighterTeam.ALLY ? alliesPositions : enemiesPositions;

        DestroyChildren(bgGroupParent);
        for (int f = 0; f < fighters.Count; f++)
        {
            GameObject prefab = Instantiate(heroInBattlePrefab, bgGroupParent.transform);

            PersonajeHandler prefabHandler = prefab.GetComponent<PersonajeHandler>();
            PersonajeHandler fighterHandler = fighters[f].GetComponent<PersonajeHandler>();
            prefabHandler.CopyFrom(fighterHandler);

            prefab.name = fighterHandler.Stats.Name + " " + f;
            FighterStateMachine prefabFSM = prefab.GetComponent<FighterStateMachine>();
            if (ft == FighterTeam.ENEMY)
                BSM.SetESM(prefab);
            prefabFSM.SetData(fighters[f]);
            prefab.transform.position = positions[f].transform.position;
            prefab.tag = BSM.FIGHTER_DEAD_TAG(ft, prefab.GetComponent<FighterStateMachine>().Hero.IsDead());
        }
    }

    internal void SetFightersInBattle(string tag, string deadTag, bool flip)
    {
        List<GameObject> fighters = new();
        fighters.AddRange(GameObject.FindGameObjectsWithTag(tag));
        fighters.AddRange(GameObject.FindGameObjectsWithTag(tag + deadTag));

        for (int i = 0; i < fighters.Count; i++)
        {
            fighters[i].GetComponent<SpriteRenderer>().flipX = flip;
        }
    }

    internal void Victory(int expToAdd, float moneyDropped)
    {
        SetActive(UIElements.BATTLE_BOX, false);
        UpdateResultResourcesGainedPanel(expToAdd, moneyDropped);
        UpdateResultExpToAddPanel(expToAdd);
        SetActive(UIElements.RESULT_PANEL, true);
    }

    private void UpdateResultResourcesGainedPanel(int expToAdd, float moneyDropped)
    {
        expTMP.SetText(expToAdd.ToString());
        moneyTMP.SetText(moneyDropped.ToString());
        moneyPanel.SetActive(showMoney);
    }

    private void UpdateResultExpToAddPanel(int expToAdd)
    {
        if (BSM.Allys.Count <= 0) return;

        AllyResultPanel[] existingPanels = resultAllyPanelSpacer.GetComponentsInChildren<AllyResultPanel>();
        FighterStateMachine fighter;
        int expToAddInd;

        if (existingPanels.Length == 0)
        {
            //There arent instances, create news
            for (int i = 0; i < BSM.Allys.Count; i++)
            {
                fighter = BSM.Allys[i].GetComponent<FighterStateMachine>();
                expToAddInd = (int)BSM.GetExpToAddValue(expToAdd, fighter);

                GameObject allyResultPanelInstance = Instantiate(allyResultPanelPrefab, resultAllyPanelSpacer);
                allyResultPanelInstance.SetActive(true);
                AllyResultPanel allyResultPanel = allyResultPanelInstance.GetComponent<AllyResultPanel>();

                allyResultPanel.SetData(fighter);
                allyResultPanel.Run(expToAddInd, fighter.gameObject);
            }
        }
        else
        {
            // There are instances, update
            for (int i = 0; i < existingPanels.Length; i++)
            {
                if (BSM.Allys[i] != null)
                {
                    fighter = BSM.Allys[i].GetComponent<FighterStateMachine>();
                    expToAddInd = (int)BSM.GetExpToAddValue(expToAdd, fighter);
                    existingPanels[i].SetData(fighter);
                    existingPanels[i].Run(expToAddInd, fighter.gameObject);
                }
            }
        }
    }

    internal void FillItemsDropSpacer(List<PersonajeDropSO> itemsDropped)
    {
        DestroyChildren(itemsDropSpacer.gameObject);
        foreach (PersonajeDropSO drop in itemsDropped)
        {
            GameObject itemDropInstance = Instantiate(itemDropPrefab, itemsDropSpacer);
            ItemDrop itemDropHandler = itemDropInstance.GetComponent<ItemDrop>();
            itemDropHandler.SetData(drop);
            itemDropInstance.SetActive(true);
        }
    }


    internal void Notify(string text)
    {
        if (!notifying)
        {
            notifying = true;
            StartCoroutine(NotifyCoroutine(text));
        }
        return;
    }

    internal IEnumerator NotifyCoroutine(string v)
    {
        infoPanelText.SetText(v);
        SetActive(UIElements.INFO_PANEL, true);
        yield return new WaitForSeconds(notifyTime);
        SetActive(UIElements.INFO_PANEL, false);
        notifying = false;
        yield break;
    }

    internal void Notify(string text, float time)
    {
        if (!notifying)
        {
            notifying = true;
            StartCoroutine(NotifyCoroutine(text, time));
        }
        return;
    }

    internal IEnumerator NotifyCoroutine(string v, float time)
    {
        infoPanelText.SetText(v);
        SetActive(UIElements.INFO_PANEL, true);
        if (time != 0)
        {
            yield return new WaitForSeconds(time);
            SetActive(UIElements.INFO_PANEL, false);
            notifying = false;
        }
        yield break;
    }
}
