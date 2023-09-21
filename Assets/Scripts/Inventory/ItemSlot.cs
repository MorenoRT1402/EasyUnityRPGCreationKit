using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public enum Context { INVENTORY, EQUIP, BATTLE, SHOP }

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameTMP, amountTMP;
    [SerializeField] GameObject priceGO;
    [SerializeField] private TextMeshProUGUI priceTMP;
    [SerializeField] private Image moneyIconImage;

    private Context context;

    private ItemSO item;
    private int amount;
    private float price;

    public void ItemChoosed()
    {
        if (!Chooseable(context)) return;

        BattleStateMachine BSM = BattleStateMachine.Instance;
        MenuManager MM = MenuManager.Instance;
        ShopManager SM = ShopManager.Instance;

        switch (context)
        {
            case Context.INVENTORY:
                MM.ItemChoosed(item, amount);
                break;
            case Context.BATTLE:
                BSM.ItemInput(item);
                break;
            case Context.SHOP:
                SM.ItemChoosed(item);
                break;
            case Context.EQUIP:
                break;
        }
    }

    private bool Chooseable(Context context)
    {
        return item.Usable || context == Context.EQUIP || context == Context.SHOP;
    }

    public void SetItem(ItemSO item)
    {
        this.item = item;
    }

    public void SetAmount(int amount)
    {
        this.amount = amount;
    }

    internal void SetCustomPrice(float customPrice)
    {
        price = customPrice;
    }

    public void UpdateUI(Context context)
    {
        //        if (!ShopManager.Instance.ItemSlotInteractuable) return;

        BattleManagerUI BMUI = BattleManagerUI.Instance;
        Button button = GetComponent<Button>();
        if (!item.Usable) button.enabled = false;
        ActiveComponents(context);

        switch (context)
        {
            case Context.INVENTORY:
                break;
            case Context.BATTLE:
                if (BMUI != null) BMUI.UpdateItemPanelUI(item);
                break;
            case Context.SHOP:
                button.enabled = true;
                ShopManager.Instance.UpdateUI(item);
                break;
            case Context.EQUIP:
                button.enabled = true;
                break;
        }

        float amount = InventoryManager.Instance.GetTotalAmount(item);
        iconImage.sprite = item.Icon;
        nameTMP.text = item.Name;
        amountTMP.text = "x" + amount;
        priceTMP.text = price.ToString();
        moneyIconImage.sprite = UIManager.Instance.moneySprite;
        this.context = context;
    }

    public void AutoUpdateUI()
    { //For OnClick
        UpdateUI(context);
    }

    private void ActiveComponents(Context context)
    {
        switch (context)
        {
            case Context.INVENTORY:
            case Context.BATTLE:
            case Context.EQUIP:
                if (priceGO != null) priceGO.SetActive(false);
                amountTMP.gameObject.SetActive(true);
                break;
            case Context.SHOP:
                priceGO.SetActive(true);
                amountTMP.gameObject.SetActive(amount > 0);
                break;
        }
    }

    public void AutoUpdateUI(KeyValuePair<EquipmentSO, int> equip, Context context)
    {
        item = equip.Key;
        amount = equip.Value;

        UpdateUI(context);
    }

    internal void SetRemove()
    {
        InventoryManager IM = InventoryManager.Instance;

        nameTMP.text = IM.equipRemoveOptionName;
        amountTMP.text = "";
    }
}
