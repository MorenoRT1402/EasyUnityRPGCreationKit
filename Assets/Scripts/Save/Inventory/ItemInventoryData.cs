using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemInventoryData
{
    public string itemPath;
    public float amount;

    public ItemInventoryData(){}
    public ItemInventoryData(ItemSO item, float amount){
        itemPath = GeneralManager.ToPath(item);
        this.amount = amount;
    }

    internal static ItemInventoryData[] GetInventory(Dictionary<ItemSO, int> items)
    {
        List<ItemInventoryData> itemDatas = new();
        foreach(KeyValuePair<ItemSO, int> itemPair in items)
        itemDatas.Add(new(itemPair.Key, itemPair.Value));

        return itemDatas.ToArray();
    }

    internal void Load()
    {
        ItemSO item = (ItemSO)GeneralManager.To(itemPath);
        
        InventoryManager.Instance.Add(item, (int)amount);
    }
}
