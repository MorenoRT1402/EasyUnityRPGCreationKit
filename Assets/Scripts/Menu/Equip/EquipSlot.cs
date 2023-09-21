using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public TextMeshProUGUI equipPartTMP, equipmentTMP;
    public Image equipmentIcon;
    public Button button;
    public EventTrigger eventTrigger;

    private EquipPartSO part;
    private EquipmentSO equipment;

    internal void SetData(KeyValuePair<EquipPartSO, EquipmentSO> equipSlot)
    {
        part = equipSlot.Key;
        equipment = equipSlot.Value;
    }

    internal void UpdateUI()
    {
        equipPartTMP.text = part.equipPartName;
        if (equipment != null)
        {
            if (equipment.Icon != null) equipmentIcon.sprite = equipment.Icon;

            equipmentTMP.text = equipment.Name;
        }
        else
        {
            equipmentTMP.text = "Empty";
        }
    }
}
