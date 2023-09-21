using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string itemName;
    public string itemDescription;
    public string iconSpritePath;
    public ItemSO.ItemImportance importance;
    public bool usable;
    public bool consumable;
    public float power;
    public string effect;
    public float buyPrice;
    public float sellPrice;

    public int maxAmount;

    public ItemData(){}
    public ItemData(ItemSO item)
    {
        itemName = item.Name;
        itemDescription = item.Description;
        iconSpritePath = GeneralManager.ToPath(item.Icon);
        importance = item.Importance;
        usable = item.Usable;
        consumable = item.Consumable;
        power = item.Power;
        effect = GeneralManager.ToPath(item.Effect);
        buyPrice = item.BuyPrice;
        sellPrice = item.SellPrice;
        maxAmount = item.MaxAmount;
    }
}
