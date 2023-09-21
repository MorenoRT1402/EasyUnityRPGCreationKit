using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private List<ItemSO> startItems;
    [HideInInspector] public Dictionary<ItemSO, int> items = new();
    [HideInInspector] public float moneyInPosesion;

    [Header("Glosary")]
    public string equipRemoveOptionName = "Remove";

    private new void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < startItems.Count; i++)
        {
            Add(startItems[i], 1);
        }
    }

    public void Add(ItemSO itemSO, int amount)
    {
        if (itemSO == null) return;
        if (items.ContainsKey(itemSO))
        {
            items[itemSO] += amount;
        }
        else
        {
            items.Add(itemSO, amount);
        }

        if (items[itemSO] >= itemSO.MaxAmount)
            items[itemSO] = itemSO.MaxAmount;
    }

    internal void Remove(ItemSO item, int amount)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= amount;

            if (items[item] <= 0)
            {
                items.Remove(item);
            }
        }

        UpdateInventory();
    }

    private void UpdateInventory()
    {
        // Create a new dictionary to store the items with non-zero amounts.
        Dictionary<ItemSO, int> updatedItemsList = new();

        foreach (KeyValuePair<ItemSO, int> itemEntry in items)
        {
            //            Debug.Log(itemEntry.Key + " x" + itemEntry.Value);
            if (itemEntry.Value > 0)
            {
                updatedItemsList.Add(itemEntry.Key, itemEntry.Value);
            }
        }

        // Clear the original dictionary and add the updated items back.
        items.Clear();
        foreach (KeyValuePair<ItemSO, int> itemEntry in updatedItemsList)
        {
            items.Add(itemEntry.Key, itemEntry.Value);
        }
    }

    internal Dictionary<ItemSO, int> GetItemSlots(ItemSO.ItemImportance itemImportance)
    {
        Dictionary<ItemSO, int> itemSlots = new();

        foreach (KeyValuePair<ItemSO, int> itemEntry in items)
        {
            if (itemEntry.Key.Importance == itemImportance)
            {
                itemSlots.Add(itemEntry.Key, itemEntry.Value);
            }
        }

        return itemSlots;
    }

    internal int GetAmount(ItemSO item)
    {
        if (items.ContainsKey(item))
            return items[item];
        return 0;
    }

    internal int GetTotalAmount(ItemSO item)
    {
        int inInventoryAmount = GetAmount(item);
        int equippedAmount = 0;
        if (item is EquipmentSO equipmentSO)
            equippedAmount = GetEquipped(equipmentSO);

        return inInventoryAmount + equippedAmount;
    }

    internal Dictionary<EquipmentSO, int> GetEquipPieces()
    {
        Dictionary<EquipmentSO, int> equipPieces = new();
        foreach (KeyValuePair<ItemSO, int> keyValuePair in items)
            if (keyValuePair.Key is EquipmentSO equipment) //is EquipmentSO
                equipPieces.Add(equipment, keyValuePair.Value);

        return equipPieces;
    }

    internal int GetEquipped(EquipmentSO equip)
    {
        List<PersonajeHandler> alliesList = PartyManager.Instance.GetAllAllies();
        int count = 0;

        for (int i = 0; i < alliesList.Count; i++)
            count += alliesList[i].GetEquipped(equip);

        return count;

    }

    internal void Trade(ItemSO item, float itemAmount, float moneyAmount, ShopManager.Context context)
    {
        switch (context)
        {
            case ShopManager.Context.BUY:
                Add(item, (int)itemAmount);
                moneyAmount *= -1;
                break;
            case ShopManager.Context.SELL:
                Remove(item, (int)itemAmount);
                break;
        }
        AddMoney(moneyAmount);
    }

    private void AddMoney(float amount)
    {
        moneyInPosesion += amount;
    }
}
