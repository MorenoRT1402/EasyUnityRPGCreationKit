using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum MenuOptions
{
    ITEMS, SKILLS, EQUIP, STATUS, FORMATION, SETTINGS, SAVE, EXIT,
    Resumen
}

public class MenuManager : Singleton<MenuManager>
{

    public enum LevelShowed { CHARACTER_LEVEL, JOB_LEVEL }

    [Header("Config")]
    public LevelShowed LevelShowedInMenu;

    [Header("References")]
    [SerializeField] private GameObject menuGO;
    [SerializeField] private GameObject menuOptionsSpacer;
    [SerializeField] private GameObject menuPartyResumen;

    [Header("References/Money Panel")]
    [SerializeField] private TextMeshProUGUI currencyNameTMP;
    [SerializeField] private TextMeshProUGUI moneyQuantityTMP;
    [SerializeField] private Image moneyImage;
    [Header("References/Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventorySpacer;
    [SerializeField] private GameObject targetSelectPanelItem;
    [SerializeField] private GameObject targetPanelItem;
    [SerializeField] private GameObject targetSpacerItem;
    [Header("References/Skills")]
    [SerializeField] private GameObject skillsMainPanel;
    [SerializeField] private GameObject skillCharacterSpacer;
    [SerializeField] private GameObject skillFamilySpacer;
    [SerializeField] private GameObject skillSpacer;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    [SerializeField] private TextMeshProUGUI manaCostTMP;
    [SerializeField] private TextMeshProUGUI staminaCostTMP;
    [SerializeField] private GameObject userTargetPanelSkill;
    [SerializeField] private GameObject targetPanelSkill;
    [SerializeField] private TextMeshProUGUI skillNameTMP;
    [SerializeField] private TextMeshProUGUI manaCostTargetPanel;
    [SerializeField] private TextMeshProUGUI staminaCostTargetPanel;
    [SerializeField] private GameObject targetSpacerSkill;
    [Header("References/Equip")]
    [SerializeField] private GameObject equipPanel;
    [SerializeField] private GameObject mainEquipPanel;
    [SerializeField] private GameObject characterEquipPanelSpacer;
    [Header("References/Status")]
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private Image statusSprite;
    [SerializeField] private GameObject statusCharacterNameGO;
    [SerializeField] private GameObject statusJobNameGO;
    [SerializeField] private TextMeshProUGUI statusCharacterName;
    [SerializeField] private TextMeshProUGUI statusJobName;
    [SerializeField] private GameObject statusCharacterLevelSpacer;
    [SerializeField] private GameObject statusJobLevelSpacer;
    [SerializeField] private GameObject statusBattleResourcesSpacer;
    [SerializeField] private GameObject statusPrimaryStatsSpacer;
    [SerializeField] private GameObject statusSecundaryStatsSpacer;
    [Header("References/Formation")]
    [SerializeField] private GameObject formationPanel;
    [SerializeField] private GameObject formationPartySpacer;
    [SerializeField] private GameObject formationBenchSpacer;
    [Header("References/Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI audioTitle;
    [SerializeField] private TextMeshProUGUI musicVolumeTMP, ambientVolumeTMP, soundVolumeTMP;
    [SerializeField] private Slider musicSlider, ambientSlider, soundSlider;
    [SerializeField] private TextMeshProUGUI screenTitle;
    [SerializeField] private TextMeshProUGUI brightnessTMP, qualityTMP, resolutionTMP, fullScreenTMP;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown, resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Button closeBtn, defaultBtn, saveBtn;
    [Header("References/Save")]
    [SerializeField] private GameObject savePanel;
    [SerializeField] private GameObject saveSlotSpacer;
    [Header("Prefabs")]
    [SerializeField] private GameObject menuOptionPrefab;
    [SerializeField] private GameObject characterResumenPrefab;
    [SerializeField] private GameObject menuHeroPrefab;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject skillHeroPrefab;
    [SerializeField] private GameObject skillFamilyPrefab;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private GameObject characterEquipPanel;
    [SerializeField] private GameObject nameAndValuePrefab;
    [SerializeField] private GameObject characterInFormationPrefab;
    [SerializeField] private GameObject saveSlotPrefab;

    [Header("Other Components")]
    public GameObject bright;

    public bool MenuOpened => menuGO.activeSelf;

    public GameObject MenuGO => menuGO;

    private Resolution[] resolutions;
    private InputManager IM;
    PartyManager PM;
    SettingsManager STM;

    private new void Awake()
    {
        base.Awake();
        IM = InputManager.Instance;
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SetActiveAllRightPanels(false);
        currencyNameTMP.text = GeneralManager.Instance.currencyName;
        moneyQuantityTMP.text = InventoryManager.Instance.moneyInPosesion.ToString();
        moneyImage.sprite = UIManager.Instance.moneySprite;
        menuGO.SetActive(false);

        STM = SettingsManager.Instance;
        UpdateBright();
    }

    private void Update()
    {
        Inputs();
    }

    private void Inputs()
    {
        if (Input.GetKeyDown(IM.openCloseMenu))
            ToggleMenu();
        if (Input.GetKeyDown(IM.back))
            Back();
    }

    public void Back()
    {
        if (menuPartyResumen.activeSelf)
            menuGO.SetActive(false);
        else if (targetSelectPanelItem.activeSelf)
            ChangeView(targetSelectPanelItem, inventoryPanel);
        else if (inventoryPanel.activeSelf)
            ChangeView(inventoryPanel, menuPartyResumen);
        else if (skillsMainPanel.activeSelf)
            ChangeView(skillsMainPanel, menuPartyResumen);
        else if (targetPanelSkill.activeSelf)
            ChangeView(targetPanelSkill, skillsMainPanel);
        else if (statusPanel.activeSelf)
            ChangeView(statusPanel, menuPartyResumen);
        else if (formationPanel.activeSelf)
            ChangeView(formationPanel, menuPartyResumen);
        else if (settingsPanel.activeSelf)
            ChangeView(settingsPanel, menuPartyResumen);
        else if (savePanel.activeSelf)
            ChangeView(savePanel, menuPartyResumen);
    }

    public void ChangeView(GameObject actualPanel, GameObject nextPanel)
    {
        SetActive(actualPanel, false);
        SetActive(nextPanel, true);
    }

    private void SetActive(GameObject panel, bool enabled)
    {
        if (panel == menuPartyResumen)
            UpdatePartyResumenPanel();
        else if (panel == targetSelectPanelItem)
            UpdateTargetItem();
        else if (panel == inventoryPanel)
            FilterItemCommon();
        else if (panel == skillsMainPanel)
            SetSkillMainPanel(null);
        else if (panel == targetPanelSkill)
            targetPanelSkill.SetActive(enabled);
        else if (panel == mainEquipPanel || panel == equipPanel)
            UpdateMainEquipPanel(enabled);
        else if (panel == statusPanel)
            UpdateStatusPanel(null);
        else if (panel == formationPanel)
            UpdateFormationPanel();
        else if (panel == settingsPanel)
            UpdateSettingsPanel();
        else if (panel == savePanel)
            UpdateSavePanel();


        panel.SetActive(enabled);
    }

    public void ToggleMenu()
    {
        if (MenuGO != null)
            menuGO.SetActive(!menuGO.activeSelf);
        UpdateInterface();
        GoToMainScreen();
    }

    private void UpdateInterface()
    {
        UpdateOptionsPanel();
        UpdatePartyResumenPanel();
    }

    private void UpdatePartyResumenPanel()
    {
        Clear(menuPartyResumen);

        List<GameObject> membersGO = PartyManager.Instance.PartyGameObjects;
        for (int i = 0; i < membersGO.Count; i++)
        {
            GameObject memberResumenPrefab = Instantiate(characterResumenPrefab, menuPartyResumen.transform);
            MenuHeroPrefab menuHero = memberResumenPrefab.GetComponent<MenuHeroPrefab>();
            PersonajeHandler memberHandler = membersGO[i].GetComponent<PersonajeHandler>();
            menuHero.SetData(memberHandler);
            menuHero.UpdateUI();
            memberResumenPrefab.GetComponent<Button>().enabled = false;
        }
    }

    internal void SetEnable(MenuOptions option, bool enable)
    {
        GeneralManager.Instance.SetEnable(option, enable);
    }

    private void UpdateOptionsPanel()
    {
        Clear(menuOptionsSpacer);
        InstanceOptions();

    }
    internal void DeselectAllOptions()
    {
        foreach (Transform children in menuOptionsSpacer.transform)
        {
            if (children.TryGetComponent<MenuOption>(out var menuOption)) menuOption.Deselect();
        }
    }

    private void InstanceOptions()
    {
        foreach (KeyValuePair<MenuOptions, object[]> option in GeneralManager.Instance.menuOptionDict)
        {
            MenuOptions optionKey = option.Key;
            string optionName = (string)option.Value[0];
            bool enableOption = (bool)option.Value[1];
            bool initialEnable = (bool)option.Value[2];
            if (initialEnable)
                NewOptionInstance(optionName, optionKey, enableOption);
        }
    }

    private void NewOptionInstance(string name, MenuOptions option, bool enable)
    {
        GameObject optionPrefab = Instantiate(menuOptionPrefab, menuOptionsSpacer.transform);
        MenuOption menuOption = optionPrefab.GetComponent<MenuOption>();
        menuOption.SetData(name, option, enable);
    }

    public void Clear(GameObject spacer)
    {
        UIManager.Clear(spacer);
    }

    private void GoToMainScreen()
    {
        SetActiveAllRightPanels(false);
        UpdatePartyResumenPanel();
        menuPartyResumen.SetActive(true);
    }

    internal void SetActiveAllRightPanels(bool enabled)
    {
        menuPartyResumen.SetActive(enabled);
        inventoryPanel.SetActive(enabled);
        targetSelectPanelItem.SetActive(enabled);
        skillsMainPanel.SetActive(enabled);
        targetPanelSkill.SetActive(enabled);
        //        mainEquipPanel.SetActive(enabled);
        equipPanel.SetActive(enabled);
        statusPanel.SetActive(enabled);
        formationPanel.SetActive(enabled);
        settingsPanel.SetActive(enabled);
        savePanel.SetActive(enabled);
    }

    internal void SetActive(MenuOptions option, bool enabled)
    {
        switch (option)
        {
            case MenuOptions.Resumen:
                SetActive(menuPartyResumen, enabled);
                break;
            case MenuOptions.ITEMS:
                SetActive(inventoryPanel, enabled);
                break;
            case MenuOptions.SKILLS:
                SetActive(skillsMainPanel, enabled);
                break;
            case MenuOptions.EQUIP:
                SetActive(equipPanel, enabled);
                break;
            case MenuOptions.STATUS:
                SetActive(statusPanel, enabled);
                break;
            case MenuOptions.FORMATION:
                SetActive(formationPanel, enabled);
                break;
            case MenuOptions.SETTINGS:
                SetActive(settingsPanel, enabled);
                break;
            case MenuOptions.SAVE:
                SetActive(savePanel, enabled);
                break;
            case MenuOptions.EXIT:
                ReturnToMainScreen();
                break;
        }
    }


    private void ChangeCharacterMenu(int direction)
    {
        PartyManager PM = PartyManager.Instance;
        List<GameObject> partyMembers = PM.PartyGameObjects;
        MenuHeroPrefab heroPrefab = skillCharacterSpacer.GetComponentInChildren<MenuHeroPrefab>();
        PersonajeHandler heroHandler = heroPrefab.Hero;
        int actualHeroIndex = PM.GetIndex(heroHandler);
        int indexToFind;
        if (direction == 0) //Left
            indexToFind = actualHeroIndex == 0 ? partyMembers.Count - 1 : actualHeroIndex - 1;
        else
            indexToFind = actualHeroIndex == partyMembers.Count - 1 ? 0 : actualHeroIndex + 1;

        PersonajeHandler newHandler = PM.GetPartyMember(indexToFind);

        SetSkillMainPanel(newHandler.gameObject);
        SetEquipMainPanel(newHandler.gameObject);
        UpdateStatusPanel(newHandler.gameObject);
    }

    public void ChangeCharacterMenuLeft()
    {
        ChangeCharacterMenu(0);
    }

    public void ChangeCharacterMenuRight()
    {
        ChangeCharacterMenu(1);
    }

    #region Item Option

    public void FilterItem(ItemSO.ItemImportance itemImportance)
    {
        Dictionary<ItemSO, int> filterItems = InventoryManager.Instance.GetItemSlots(itemImportance);

        UpdateInventorySpacer(filterItems);
    }

    private void UpdateInventorySpacer(Dictionary<ItemSO, int> filterItems)
    {
        Clear(inventorySpacer);
        foreach (KeyValuePair<ItemSO, int> itemEntry in filterItems)
        {
            GameObject itemPrefab = Instantiate(inventoryItemPrefab, inventorySpacer.transform);
            ItemSlot itemSlot = itemPrefab.GetComponent<ItemSlot>();
            itemSlot.SetItem(itemEntry.Key);
            itemSlot.SetAmount(itemEntry.Value);
            itemSlot.UpdateUI(ItemSlot.Context.INVENTORY);
        }
    }

    public void FilterItemCommon()
    {
        FilterItem(ItemSO.ItemImportance.COMMON);
    }
    public void FilterKeyItem()
    {
        FilterItem(ItemSO.ItemImportance.KEY_ITEM);
    }

    internal void ItemChoosed(ItemSO item, int amount)
    {
        Clear(targetPanelItem);

        targetSelectPanelItem.SetActive(true);

        GameObject itemSlotGO = Instantiate(itemSlotPrefab, targetPanelItem.transform);
        ItemSlot itemSlot = itemSlotGO.GetComponent<ItemSlot>();
        itemSlot.SetItem(item);
        itemSlot.SetAmount(amount);
        itemSlot.UpdateUI(ItemSlot.Context.INVENTORY);

        InstancePartyItemTarget(item);
    }

    private void InstancePartyItemTarget(ItemSO item)
    {
        Clear(targetSpacerItem);

        PartyManager PM = PartyManager.Instance;
        foreach (GameObject heroGO in PM.PartyGameObjects)
        {
            GameObject heroGOPrefab = Instantiate(menuHeroPrefab, targetSpacerItem.transform);
            MenuHeroPrefab menuHero = heroGOPrefab.GetComponent<MenuHeroPrefab>();
            menuHero.SetData(heroGO.GetComponent<PersonajeHandler>());
            menuHero.UpdateUI();
            Button btn = heroGOPrefab.GetComponent<Button>();
            btn.onClick.AddListener(() => UseItemOn(item, heroGO.GetComponent<PersonajeHandler>()));
            if (item.Usable) btn.enabled = true;
        }
    }

    private void UseItemOn(ItemSO item, PersonajeHandler heroGOHandler)
    {
        item.Use(heroGOHandler);
        UpdateTargetItem();
        if (InventoryManager.Instance.GetAmount(item) <= 0)
        {
            ChangeView(targetSelectPanelItem, inventoryPanel);
            FilterItem(ItemSO.ItemImportance.COMMON);
        }

    }

    private void UpdateTargetItem()
    {
        ItemSlot itemSlot = targetPanelItem.GetComponentInChildren<ItemSlot>();
        itemSlot.UpdateUI(ItemSlot.Context.INVENTORY);

        foreach (MenuHeroPrefab heroPrefab in targetSpacerItem.GetComponentsInChildren<MenuHeroPrefab>())
        {
            heroPrefab.UpdateUI();
        }
    }

    #endregion
    #region Skills

    private void SetSkillMainPanel(GameObject character)
    {
        GameObject characterToInstance = character != null ? character : PartyManager.Instance.PartyGameObjects[0];
        SetSkillCharacter(characterToInstance, skillCharacterSpacer);
        SetSkillFamilys(characterToInstance);
        descriptionTMP.text = "";
        manaCostTMP.text = "";
        staminaCostTMP.text = "";
    }

    private void SetSkills(PersonajeHandler character, string family)
    {
        Clear(skillSpacer);

        //        Debug.Log(character + " " + family);
        List<BaseActiveSkill> skillsOfFamily = character.Stats.GetSkillsByFamily(family);
        for (int i = 0; i < skillsOfFamily.Count; i++)
        {
            BaseActiveSkill skill = skillsOfFamily[i];
            GameObject skillInstance = Instantiate(skillPrefab, skillSpacer.transform);
            SkillButtonSlot skillButtonSlot = skillInstance.GetComponent<SkillButtonSlot>();
            skillButtonSlot.setData(character, skillsOfFamily[i]);
            skillButtonSlot.SetData(descriptionTMP, manaCostTMP, staminaCostTMP);
            skillButtonSlot.NameTMP.text = skill.Name;
            //            skillButtonSlot.UpdateUI(descriptionTMP, manaCostTMP, staminaCostTMP);
            Button skillInstanceBtn = skillInstance.GetComponent<Button>();
            skillInstanceBtn.onClick.AddListener(() => SkillChoosed(character, skill));
            skillInstanceBtn.enabled = skill.IsUsable(BaseActiveSkill.UsableOn.MENU);
        }
    }

    private void SkillChoosed(PersonajeHandler character, BaseActiveSkill skill)
    {
        SetActiveAllRightPanels(false);
        targetPanelSkill.SetActive(true);
        UpdateTargetPanelSkill(character, skill);
    }

    private void UpdateTargetPanelSkill(PersonajeHandler character, BaseActiveSkill skill)
    {
        skillNameTMP.text = skill.Name;
        manaCostTargetPanel.text = skill.manaCost.ToString();
        staminaCostTargetPanel.text = skill.staminaCost.ToString();

        SetSkillCharacter(character.gameObject, userTargetPanelSkill);

        UpdateTargetSpacerSkill(character, skill);
    }

    private void UpdateTargetSpacerSkill(PersonajeHandler character, BaseActiveSkill skill)
    {
        List<GameObject> members = PartyManager.Instance.PartyGameObjects;
        Clear(targetSpacerSkill);

        for (int i = 0; i < members.Count; i++)
        {
            GameObject memberGO = members[i];
            GameObject memberInstance = Instantiate(menuHeroPrefab, targetSpacerSkill.transform);
            MenuHeroPrefab memberPrefab = memberInstance.GetComponent<MenuHeroPrefab>();
            PersonajeHandler memberHandler = memberGO.GetComponent<PersonajeHandler>();
            if (!character.SufficientResources(skill)) ChangeView(targetPanelSkill, skillsMainPanel);
            memberPrefab.SetData(memberHandler);
            memberPrefab.UpdateUI();

            Button button = memberInstance.GetComponent<Button>();
            button.onClick.AddListener(() => SkillOn(character, skill, memberHandler));
            button.enabled = true;
        }
    }

    private void SkillOn(PersonajeHandler user, BaseActiveSkill skill, PersonajeHandler target)
    {
        if (skill.targetType == TargetType.ALLY || skill.targetType == TargetType.ENEMY)
            skill.Exec(user, target);
        else if (skill.targetType == TargetType.ALL_ALLIES || skill.targetType == TargetType.ALL_ENEMIES || skill.targetType == TargetType.ALL)
            for (int i = 0; i < PartyManager.Instance.PartyGameObjects.Count; i++)
                skill.Exec(user, PartyManager.Instance.PartyGameObjects[i].GetComponent<PersonajeHandler>());
        else if (skill.targetType == TargetType.USER)
            skill.Exec(user, user);

        user.UseMP(skill.manaCost);
        user.UseStamina(skill.staminaCost);
        UpdateTargetPanelSkill(user, skill);
    }

    private void SetSkillFamilys(GameObject character)
    {
        Clear(skillFamilySpacer);
        Clear(skillSpacer);

        PersonajeHandler characterHandler = character.GetComponent<PersonajeHandler>();
        List<string> skillFamilysAvailables = characterHandler.Stats.GetSkillFamilysAvailable();
        //        Debug.Log(skillFamilysAvailables.Count);
        for (int i = 0; i < skillFamilysAvailables.Count; i++)
        {
            GameObject skillFamilyInstance = Instantiate(skillFamilyPrefab, skillFamilySpacer.transform);
            TextMeshProUGUI skillFamilyTMP = skillFamilyInstance.GetComponentInChildren<TextMeshProUGUI>();
            skillFamilyTMP.text = skillFamilysAvailables[i].ToString();
            Button button = skillFamilyInstance.GetComponent<Button>();
            button.onClick.AddListener(() => SetSkills(characterHandler, skillFamilyTMP.text));
        }

    }

    private void SetSkillCharacter(GameObject character, GameObject spacer)
    {
        Clear(spacer);

        GameObject skillCharacter = Instantiate(skillHeroPrefab, spacer.transform);
        MenuHeroPrefab menuHero = skillCharacter.GetComponent<MenuHeroPrefab>();
        menuHero.SetData(character.GetComponent<PersonajeHandler>());
        menuHero.UpdateUI();
    }

    #endregion
    #region Equip
    private void UpdateMainEquipPanel(bool enabled)
    {
        mainEquipPanel.SetActive(enabled);
        SetEquipMainPanel(null);
    }

    private void SetEquipMainPanel(GameObject character)
    {
        GameObject characterToInstance = character != null ? character : PartyManager.Instance.PartyGameObjects[0];

        EquipMainMenu equipMainMenu = characterEquipPanel.GetComponent<EquipMainMenu>();
        equipMainMenu.SetData(characterToInstance);
        equipMainMenu.UpdateUI();
    }

    #endregion

    #region Status
    private void UpdateStatusPanel(GameObject character)
    {
        GeneralManager GM = GeneralManager.Instance;
        GameObject characterToInstance = character != null ? character : PartyManager.Instance.PartyGameObjects[0];
        PersonajeHandler handler = characterToInstance.GetComponent<PersonajeHandler>();
        StatsHandler stats = handler.Stats;

        //        Debug.Log($"{stats.Character.Name} : {handler.Stats.Job.Name} {GM.jobLevelShort} {stats.Job.Level} {GM.jobExpShort} {handler.PEXP.GetParcialActualJobExp()} / {handler.PEXP.GetRelativeJobExpReq()}");

        statusSprite.sprite = stats.Character.SpriteInMenu;
        statusCharacterName.text = stats.Character.Name;
        //Basic info
        //Character
        Clear(statusCharacterLevelSpacer);
        InstanceKeyAndValue(GM.level, GM.levelShort, stats.Character.Level.ToString(), statusCharacterLevelSpacer);
        InstanceKeyAndValue(GM.characterExpShort, GM.characterExpShort, handler.PEXP.GetParcialActualExp() + " / " + handler.PEXP.GetRelativeExpReq(), statusCharacterLevelSpacer);
        //Job
        statusJobNameGO.SetActive(stats.Job != null);
        if (stats.Job != null)
        {
            statusJobName.text = handler.Stats.Job.Name;
            Clear(statusJobLevelSpacer);
            InstanceKeyAndValue(GM.jobLevel, GM.jobLevelShort, stats.Job.Level.ToString(), statusJobLevelSpacer);
            InstanceKeyAndValue(GM.jobExpShort, GM.jobExpShort, handler.PEXP.GetParcialActualJobExp() + " / " + handler.PEXP.GetRelativeJobExpReq(), statusJobLevelSpacer);
        }
        //Stats
        //Battle Resources
        Clear(statusBattleResourcesSpacer);
        InstanceKeyAndValue(GM.maxHPShort, GM.maxHPShort, stats.ActualLife + " / " + stats.MaxLife, statusBattleResourcesSpacer);
        InstanceKeyAndValue(GM.maxMPShort, GM.maxMPShort, stats.ActualMana + " / " + stats.MaxMana, statusBattleResourcesSpacer);
        InstanceKeyAndValue(GM.maxStaminaShort, GM.maxStaminaShort, stats.ActualStamina + " / " + stats.MaxStamina, statusBattleResourcesSpacer);
        //Primary Stats
        Clear(statusPrimaryStatsSpacer);
        InstanceKeyAndValue(GM.strength, GM.strength, stats.MaxStrength.ToString(), statusPrimaryStatsSpacer);
        InstanceKeyAndValue(GM.mind, GM.mind, stats.MaxMind.ToString(), statusPrimaryStatsSpacer);
        InstanceKeyAndValue(GM.dexterity, GM.dexterity, stats.MaxDexterity.ToString(), statusPrimaryStatsSpacer);
        InstanceKeyAndValue(GM.speed, GM.speed, stats.MaxSpeed.ToString(), statusPrimaryStatsSpacer);
        //Secundary Stats
        Clear(statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.attack, GM.attack, stats.MaxAttack.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.defense, GM.defense, stats.MaxDefense.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.magicAttack, GM.magicAttack, stats.MaxMagicAttack.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.magicDefense, GM.magicDefense, stats.MaxMagicDefense.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.precision, GM.precision, stats.MaxPrecision.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.evasion, GM.evasion, stats.MaxEvasion.ToString(), statusSecundaryStatsSpacer);
        InstanceKeyAndValue(GM.critProb, GM.critProb, stats.MaxCritProb.ToString(), statusSecundaryStatsSpacer);

    }

    private void InstanceKeyAndValue(string name, string key, string value, GameObject spacer)
    {
        GameObject instance = Instantiate(nameAndValuePrefab, spacer.transform);
        if (name != null) instance.name = name;
        NameAndValue nameAndValue = instance.GetComponent<NameAndValue>();
        nameAndValue.SetData(key, value);
        nameAndValue.UpdateUI();
    }
    #endregion

    #region Formation

    public void UpdateFormationPanel()
    {
        PM = PartyManager.Instance;
        List<GameObject> party = PM.PartyGameObjects;
        List<GameObject> bench = PM.BenchGameObjects;

        UpdateFormationSpacer(party, formationPartySpacer, true);
        UpdateFormationSpacer(bench, formationBenchSpacer, false);
    }

    private void UpdateFormationSpacer(List<GameObject> list, GameObject spacer, bool mainParty)
    {
        Clear(spacer);

        int count = mainParty ? PM.activePartyCount : PM.BenchGameObjects.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject instance = Instantiate(characterInFormationPrefab, spacer.transform);
            CharacterInFormation characterIF = instance.GetComponent<CharacterInFormation>();
            characterIF.Init();
            if (list.Count >= i + 1 && list[i] != null)
            {
                //                Debug.Log($"{i} {list[i].name}");
                characterIF.SetData(list[i].GetComponent<PersonajeHandler>());
                characterIF.UpdateUI();
            }
        }
    }

    #endregion

    #region Settings
    #region Basic
    private void UpdateSettingsPanel()
    {
        SetSettingsGlosaryValues();
        SetActualSettingsValues();
    }

    private void SetActualSettingsValues()
    {
        musicSlider.value = STM.MasterMusic;
        ambientSlider.value = STM.MasterAmbient;
        soundSlider.value = STM.MasterSounds;

        brightnessSlider.value = STM.Brightness;
        qualityDropdown.value = STM.QualityLevel;
    }

    private void SetSettingsGlosaryValues()
    {
        STM = SettingsManager.Instance;

        SetSettingsTexts();
        SetResolutionsTexts();
    }

    private void SetSettingsTexts()
    {
        SetSettingsTMPS();
        SetImageQualityTexts();
    }

    private void SetResolutionsTexts()
    {
        resolutions = Screen.resolutions;
        List<string> options = new();
        foreach (Resolution res in resolutions)
            options.Add($"{res.width} x {res.height}");

        GeneralManager.AddOptions(resolutionDropdown, options);

        SetActualResolution();

    }

    private void SetActualResolution()
    {
        Resolution actualResolution = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
            if (Screen.fullScreen && actualResolution.width == resolutions[i].width && actualResolution.height == resolutions[i].height)
            {
                SetResolution(i);
            }
        resolutionDropdown.RefreshShownValue();
    }

    private void SetResolution(int i)
    {
        Resolution resolution = resolutions[i];
        STM.ResWidt = resolution.width;
        STM.ResHeight = resolution.height;
        STM.FullScreen = Screen.fullScreen;
        resolutionDropdown.value = i;
        Screen.SetResolution(resolution.width, resolution.height, STM.FullScreen);
    }

    private void SetSettingsTMPS()
    {
        STM = SettingsManager.Instance;
        audioTitle.text = STM.audioTitleText;
        musicVolumeTMP.text = STM.musicVolumeText;
        ambientVolumeTMP.text = STM.ambientVolumeText;
        soundVolumeTMP.text = STM.soundVolumeText;
        brightnessTMP.text = STM.brightnessText;
        qualityTMP.text = STM.qualityText;
        resolutionTMP.text = STM.resolutionText;
        fullScreenTMP.text = STM.fullScreenText;
        GeneralManager.SetText(closeBtn, STM.closeButtonText);
        GeneralManager.SetText(defaultBtn, STM.defaultValuesText);
        GeneralManager.SetText(saveBtn, STM.saveButtonText);
    }

    private void SetImageQualityTexts()
    {
        List<string> options = new() { STM.veryLowQuality, STM.lowQuality, STM.mediumQuality, STM.highQuality, STM.veryHighQuality, STM.UltraQuality };

        GeneralManager.AddOptions(resolutionDropdown, options);
    }
    public void SetSettingsDefaultValues()
    {
        STM.InitialValues();
        UpdateSettingsPanel();
    }
    #endregion
    #region Settings Changes
    public void MusicMasterChange()
    {
        AudioManager.Instance.UpdateVolume(AudioType.MUSIC, musicSlider.value);
    }
    public void AmbientMasterChange()
    {
        AudioManager.Instance.UpdateVolume(AudioType.AMBIENTAL, musicSlider.value);
    }
    public void SoundMasterChange()
    {
        AudioManager.Instance.UpdateVolume(AudioType.SOUND, musicSlider.value);
    }
    public void BrightnessChange()
    {
        STM.Brightness = brightnessSlider.value;
        UpdateBright();
    }

    internal void UpdateBright()
    {
        //        bright.GetComponent<CanvasGroup>().alpha = STM.Brightness;
        bright.GetComponent<Image>().color = new Color(0, 0, 0, STM.Brightness);
    }
    public void QualityChange()
    {
        STM.QualityLevel = qualityDropdown.value;
        QualitySettings.SetQualityLevel(STM.QualityLevel);

    }
    public void ResolutionChange()
    {
        SetResolution(resolutionDropdown.value);
    }
    public void ToogleFullScreen()
    {
        STM.FullScreen = fullScreenToggle.isOn;
        Screen.fullScreen = STM.FullScreen;
    }
    #endregion
    #endregion

    #region Save
    private void UpdateSavePanel()
    {
        UIManager.UpdateSavePanel(saveSlotPrefab, saveSlotSpacer, SaveManager.Operation.SAVE);
    }

    public void InstanceSaveSlot(SaveData saveData, int i)
    {
        GameObject slotPrefab = Instantiate(saveSlotPrefab, saveSlotSpacer.transform);
        Saveslot saveslot = slotPrefab.GetComponent<Saveslot>();
        if (saveData == null)
            saveslot.UpdateUI(i);
        else
            saveslot.UpdateUI(i, saveData.ubication, saveData.GetDateTime(), saveData.GetPlayTime(), saveData.mainPartyData);
        saveslot.GetComponent<Button>().onClick.AddListener(() => Save(i));
    }

    public void Save(int i)
    {
        SaveManager.SaveData(SaveManager.Instance.GetFullPath(i));
        UpdateSavePanel();
    }
    #endregion

    #region Exit
    public void ReturnToMainScreen()
    {
        SceneHandler.Instance.ChangeToScene(SceneHandler.Instance.mainScreenScene, false);
        SceneHandler.DestroyAllEvents();

        //        MainScreenManager.Instance.Initialize();
    }
    #endregion
}