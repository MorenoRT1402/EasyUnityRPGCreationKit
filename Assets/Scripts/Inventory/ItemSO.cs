using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new item")]
public class ItemSO : ScriptableObject
{
    public enum ItemImportance { COMMON, KEY_ITEM }
    [SerializeField] private string itemName;
    [SerializeField][TextArea] private string itemDescription;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemImportance importance = ItemImportance.COMMON;
    [SerializeField] private bool usable = true;
    [SerializeField] private bool consumable = true;
    [SerializeField] private float power = 0;
    [SerializeField] private BaseActiveSkill effect;
    [SerializeField] private float buyPrice;
    [SerializeField] private float sellPrice;

    [SerializeField] private int maxAmount = 99;

    public string Name => itemName;
    public string Description => itemDescription;
    public Sprite Icon => icon;
    public ItemImportance Importance => importance;
    public bool Usable => usable;
    public bool Consumable => consumable;
    public float Power => power;
    public BaseActiveSkill Effect => effect;
    public float BuyPrice => buyPrice;
    public float SellPrice => sellPrice;
    public int MaxAmount => maxAmount;

    private void Start() {
        effect.power = power;
    }

    internal void Use(PersonajeHandler user, PersonajeHandler target)
    {
        effect.power = power;
        effect.Exec(user, target);
        if(consumable) InventoryManager.Instance.Remove(this, 1);
    }

        internal void Use(PersonajeHandler hero)
    {
        effect.power = power;
        effect.Exec(hero, hero);
        if(consumable) InventoryManager.Instance.Remove(this, 1);
    }
}
