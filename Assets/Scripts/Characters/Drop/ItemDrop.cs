using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI quantity;
    private PersonajeDropSO drop;

    internal void SetData(PersonajeDropSO drop)
    {
        this.drop = drop;
        itemName.SetText(drop.item.Name);
        quantity.SetText(drop.quantity.ToString());
    }
}
