using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class EquipMainMenu : MonoBehaviour
{
    public PersonajeHandler character;

    public GameObject parentPanel;

    [Header("Main Panel")]
    public GameObject equipPanel;
    public TextMeshProUGUI nameTMP, jobTMP;
    public Image sprite;
    public GameObject equipSlotPrefab;
    public GameObject equipPartsSpacer;

    [Header("Change Equip Panel")]
    public GameObject changeEquipPanel;
    public GameObject cEEquipmentPrefab;
    public GameObject equipmentSpacer;
    public TextMeshProUGUI cENameTMP, cEJobTMP, cEPartNameTMP, cEActualEquipmentNameTMP;
    public Image cESprite;
    public GameObject iconAndTMPPrefab;
    public GameObject cEActualEquipmentSpacer;

    [Header("Common")]
    public TextMeshProUGUI descriptionTMP;
    public GameObject statModPrefab;
    public GameObject statsSpacer;

    private List<Stats> defaultStatsToShow;

    private InputManager IM;

    internal void SetData(GameObject character)
    {
        this.character = character.GetComponent<PersonajeHandler>();
    }

    private void Update()
    {
        inputs();
    }

    private void inputs()
    {
        IM = InputManager.Instance;

        if (Input.GetKeyDown(IM.openCloseMenu))
            MenuManager.Instance.ToggleMenu();
        if (Input.GetKeyDown(IM.back))
            back();
    }

    private void back()
    {
        if (equipPanel.activeSelf)
            back(equipPanel);
        else if (changeEquipPanel.activeSelf)
            back(changeEquipPanel);
    }

    internal void UpdateUI()
    {
        changeEquipPanel.SetActive(false);
        equipPanel.SetActive(true);
        initDefaultStats();
        nameTMP.text = character.Stats.Name;
        jobTMP.text = character.Stats.Job.Name;
        descriptionTMP.text = "";
        sprite.sprite = character.Stats.Character.SpriteInMenu;
        setEquipParts();
        setDefaultStats();
    }

    private void initDefaultStats()
    {
        defaultStatsToShow = new List<Stats>(){
            Stats.MAX_ATTACK, Stats.MAX_PRECISION,
            Stats.MAX_DEFENSE, Stats.MAX_EVASION,
            Stats.MAX_MAGIC_ATTACK, Stats.MAX_MAGIC_DEFENSE
        };
    }

    public void Optimize(){
        character.Optimize();
        UpdateUI();
    }

    public void RemoveAll(){
        character.Stats.RemoveAll();
        UpdateUI();
    }

    private void setStats(EquipmentSO oldEquipmentSO, EquipmentSO newEquipmentSO)
    {
        Clear(statsSpacer);

        if (newEquipmentSO == null && oldEquipmentSO == null)
            setDefaultStats();
        else{
            Dictionary<Stats, float> statModDict = oldEquipmentSO != null ? oldEquipmentSO.GetChangingStats(newEquipmentSO, true) : newEquipmentSO.GetChangingStats(oldEquipmentSO, false);
            foreach (KeyValuePair<Stats, float> statMod in statModDict)
            {
                if (GeneralManager.Instance.nameOfStats[statMod.Key] != "")
                {
                    GameObject statModInstance = Instantiate(statModPrefab, statsSpacer.transform);
                    StatMod statModHandler = statModInstance.GetComponent<StatMod>();
                    string name = GeneralManager.Instance.nameOfStats[statMod.Key];
                    float oldValue = character.Stats.Get(statMod.Key);
                    float newValue = oldValue + statMod.Value;
                    statModHandler.SetData(name, oldValue, newValue);
                    statModHandler.UpdateUI(true);
                }
            }
        }
    }

    private void setDefaultStats()
    {
        Clear(statsSpacer);

        GeneralManager GM = GeneralManager.Instance;

        foreach (Stats statToShow in defaultStatsToShow)
        {
            if (GM.nameOfStats[statToShow] != "")
            {
                //                Debug.Log("SetDefaultStats " + statToShow + " called " + GM.nameOfStats[statToShow] + " value " + character.Stats.Get(statToShow));
                GameObject statModInstance = Instantiate(statModPrefab, statsSpacer.transform);
                StatMod statModHandler = statModInstance.GetComponent<StatMod>();
                string statName = GM.nameOfStats[statToShow];
                float oldValue = character.Stats.Get(statToShow);
                float newValue = character.Stats.Get(statToShow);
                statModHandler.SetData(statName, oldValue, newValue);
                statModHandler.UpdateUI(false);
            }
        }
    }

    private void Clear(GameObject spacer)
    {
        MenuManager.Instance.Clear(spacer);
    }


    private void setEquipParts()
    {
        Clear(equipPartsSpacer);

        Dictionary<EquipPartSO, EquipmentSO> equipDict = character.Stats.EquipDict;

        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in equipDict)
        {
            GameObject equipSlotInstance = Instantiate(equipSlotPrefab, equipPartsSpacer.transform);
            EquipSlot equipSlotHandler = equipSlotInstance.GetComponent<EquipSlot>();
            equipSlotHandler.SetData(equipSlot);
            equipSlotHandler.UpdateUI();
            Button button = equipSlotHandler.button;
            button.onClick.AddListener(() => openChangeEquipment(character, equipSlot));
            button.enabled = true;
        }
    }

    private void openChangeEquipment(PersonajeHandler character, KeyValuePair<EquipPartSO, EquipmentSO> equipSlot)
    {
        MenuManager.Instance.ChangeView(equipPanel, changeEquipPanel);
        SetChangeEquipments(character, equipSlot);
        setRightPanel(character, equipSlot.Key, equipSlot.Value);
        //Set this Right Panel (sprite & equip)
    }

    private void setRightPanel(PersonajeHandler character, EquipPartSO part, EquipmentSO equipment)
    {
        cENameTMP.text = character.Stats.Name;
        cEJobTMP.text = character.Stats.Job.Name;
        cEPartNameTMP.text = part.equipPartName;
        string equipmentName = equipment == null ? "Empty" : equipment.Name;
        cEActualEquipmentNameTMP.text = equipmentName;
        cESprite.sprite = character.Stats.Character.SpriteInMenu;

        Clear(cEActualEquipmentSpacer);
        foreach (KeyValuePair<EquipPartSO, EquipmentSO> equipSlot in character.Stats.EquipDict)
        {
            GameObject equipSlotInstance = Instantiate(iconAndTMPPrefab, cEActualEquipmentSpacer.transform);
            if (equipSlotInstance.TryGetComponent<IconAndTMP>(out var iconAndTMP))
            {
                Sprite icon = equipSlot.Value != null ? equipSlot.Value.Icon : null;
                string equipmentNameToWrite = equipSlot.Value == null ? "Empty" : equipSlot.Value.Name;
                iconAndTMP.SetData(icon, equipmentNameToWrite);
                iconAndTMP.UpdateUI(IconAndTMP.FORMAT.EQUIPMENT);
            }
        }
    }

    private void SetChangeEquipments(PersonajeHandler character, KeyValuePair<EquipPartSO, EquipmentSO> equipSlot)
    {
        Clear(equipmentSpacer);

        Dictionary<EquipmentSO, int> equipPieces = InventoryManager.Instance.GetEquipPieces();

        InstanceRemoveOption(equipSlot);

        foreach (KeyValuePair<EquipmentSO, int> keyValuePair in equipPieces)
        {
            EquipmentSO equipment = keyValuePair.Key;
            if (equipment.IsEquipable(character.Stats, equipSlot.Key))
            {
                GameObject equipSlotInstance = Instantiate(cEEquipmentPrefab, equipmentSpacer.transform);
                ItemSlot equipmentSlot = equipSlotInstance.GetComponent<ItemSlot>();
                equipmentSlot.AutoUpdateUI(keyValuePair, ItemSlot.Context.EQUIP);

                EquipPartSO part = equipSlot.Key;

                EventTrigger eventButton = equipSlotInstance.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new(){
                    eventID = EventTriggerType.PointerEnter
                };
                entry.callback.AddListener((eventData) => showEquipInfo(part, equipment));
                eventButton.triggers.Add(entry);

                Button button = equipSlotInstance.GetComponent<Button>();
                button.onClick.AddListener(() => equip(keyValuePair.Key, equipSlot.Key));
            }
        }
    }

    private void InstanceRemoveOption(KeyValuePair<EquipPartSO, EquipmentSO> equipSlot)
    {
        GameObject equipSlotInstance = Instantiate(cEEquipmentPrefab, equipmentSpacer.transform);
        ItemSlot equipmentSlot = equipSlotInstance.GetComponent<ItemSlot>();
        equipmentSlot.SetRemove();

        EquipPartSO part = equipSlot.Key;

        EventTrigger eventButton = equipSlotInstance.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new(){
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => showEquipInfo(part, null));
        eventButton.triggers.Add(entry);

        Button button = equipSlotInstance.GetComponent<Button>();
        button.onClick.AddListener(() => unequip(equipSlot.Key));
    }

    private void unequip(EquipPartSO part)
    {
        character.Unequip(part);

        back(changeEquipPanel);
    }

    private void equip(EquipmentSO equipment, EquipPartSO part)
    {
        character.Equip(equipment, part);

        back(changeEquipPanel);

    }

    private void back(GameObject panel)
    {
        panel.SetActive(false);
        if (panel == changeEquipPanel)
        {
            equipPanel.SetActive(true);
            UpdateUI();
        }
        else if (panel == equipPanel)
        {
            parentPanel.SetActive(false);
            MenuManager.Instance.ToggleMenu();
            MenuManager.Instance.ToggleMenu();
        }
    }

    public void showEquipInfo(EquipPartSO part, EquipmentSO equip)
    {
        descriptionTMP.text = equip != null ? equip.Description : "";

        setStats(part, equip);
    }

    private void setStats(EquipPartSO part, EquipmentSO equip)
    {
        EquipmentSO actualEquip = character.GetEquipment(part);
        EquipmentSO newEquip = equip;

        setStats(actualEquip, newEquip);
    }
}
