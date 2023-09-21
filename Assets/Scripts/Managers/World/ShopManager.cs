using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{

    public enum Context { NONE, BUY, SELL }

    [Header("Glosary")]
    public string sellInputHeaderMessage = "How many are you selling?";
    public string buyInputHeaderMessage = "How many would you like";
    public string confirmButtonText = "Confirm";
    public string cancelButtonText = "Cancel";
    public string dontHaveEnoughMoneyText = "You don't have enough money";
    public string successfullTradeText = "Thanks!";

    [Header("UI")]
    public Sprite upAtributesSprite;
    public Sprite equalAtributesSprite;
    public Sprite downAtributesSprite;

    [Header("Components")]
    [Header("Components/Main")]
    public GameObject shopGO;
    public GameObject buyBtn, sellBtn, exitBtn;
    public TextMeshProUGUI moneyTMP;
    public Image moneyImage;
    public GameObject buyPanel;
    public GameObject equippedGO;
    public TextMeshProUGUI stockTMP, equippedTMP;
    public GameObject characterSpace, equipUpdateCharacterSpace;
    public TextMeshProUGUI descriptionTMP;
    [Header("Components/Input Panel")]
    public GameObject itemInputGO;
    public TextMeshProUGUI headerItemInputTMP;
    public GameObject itemSlotSpace;
    public TextMeshProUGUI inputAmountTMP;
    public Button inputUp, inputDown;
    public TextMeshProUGUI inputTotalTMP;
    public Image inputTotalImage;
    public Button inputConfirmButton, inputCancelButton;

    [Header("Prefab")]
    public GameObject itemPrefab;
    public GameObject equipUpdateCharacterPrefab;

    private List<ItemSO> items;
    private Context context;

    public bool ShopOpened => shopGO.activeSelf;

    private void Start()
    {
        shopGO.SetActive(false);
        itemInputGO.SetActive(false);
    }

    internal void ToogleShop(bool enabled)
    {
        InitializeUI();
        shopGO.SetActive(enabled);
    }

    private void InitializeUI()
    {
        RefreshShop();

        descriptionTMP.text = "";
    }

    private void RefreshShop()
    {
        moneyTMP.text = InventoryManager.Instance.moneyInPosesion.ToString();
        characterSpace.SetActive(false);
        UpdateMainSpace();
        stockTMP.text = "";
        equippedTMP.text = "";
    }

    internal void UpdateUI(ItemSO item)
    {
        int equipped = 0;
        equippedGO.SetActive(false);
        equippedTMP.gameObject.SetActive(false);
        equipUpdateCharacterSpace.SetActive(false);
        if (item is EquipmentSO equip)
        {
            equipped = InventoryManager.Instance.GetEquipped(equip);
            equippedTMP.text = equipped.ToString();
            equippedGO.SetActive(true);
            equippedTMP.gameObject.SetActive(true);
            equipUpdateCharacterSpace.SetActive(true);
            UpdateEquipUpdateCharacterPanel(equip);
        }
        int stock = InventoryManager.Instance.GetAmount(item) + equipped;
        stockTMP.text = stock.ToString();
        descriptionTMP.text = item.Description;
    }

    internal void ItemChoosed(ItemSO item)
    {
        float funds = InventoryManager.Instance.moneyInPosesion;
        bool canBuy = GetAmountBuyable(item, funds) > 0 || item.BuyPrice <= 0;
        bool canSell = InventoryManager.Instance.GetTotalAmount(item) > 0;
        bool canTrade = context == Context.BUY && canBuy || context == Context.SELL && canSell;
        if (canTrade)
            ActiveInputPanel(item, context, true);
    }

    private int GetAmountBuyable(ItemSO item, float funds)
    {
        int absoluteMaxAmount = GetAmountBuyable(item);
        int maxAmountWithThisFunds = item.BuyPrice == 0 ? absoluteMaxAmount : (int)(funds / item.BuyPrice);
        Debug.Log($"Min({absoluteMaxAmount}, {maxAmountWithThisFunds}) = {Mathf.Min(absoluteMaxAmount, maxAmountWithThisFunds)}");
        return Mathf.Min(absoluteMaxAmount, maxAmountWithThisFunds);
    }

    private int GetAmountBuyable(ItemSO item)
    {
        int amountInInventory = InventoryManager.Instance.GetTotalAmount(item);
        int maxStack = item.MaxAmount;
        return maxStack - amountInInventory;
    }

    private void ActiveInputPanel(ItemSO item, Context context, bool enabled)
    {
        ActiveInputPanel(context, enabled);

        InstanceItemSlot(item, itemSlotSpace.transform, false);

        inputUp.onClick.AddListener(() => ChangeInputAmount(item, 1, context));
        inputAmountTMP.text = "1";
        inputDown.onClick.AddListener(() => ChangeInputAmount(item, -1, context));

        InputSetTotalAmount(item, 1);

        inputConfirmButton.onClick.AddListener(() => ConfirmInput(item));

    }

    private void ActiveInputPanel(Context context, bool enabled)
    {
        inputUp.onClick.RemoveAllListeners();
        inputDown.onClick.RemoveAllListeners();
        inputConfirmButton.onClick.RemoveAllListeners();

        itemInputGO.SetActive(enabled);

        headerItemInputTMP.text = context == Context.BUY ? buyInputHeaderMessage : sellInputHeaderMessage;

        UIManager.Clear(itemSlotSpace);

    }

    private void ChangeInputAmount(ItemSO item, int amount, Context context)
    {
        float funds = InventoryManager.Instance.moneyInPosesion;
        int actualAmount = int.Parse(inputAmountTMP.text);
        int nextAmount = actualAmount + amount;
        int maxAmount = context == Context.BUY ? GetAmountBuyable(item, funds) : InventoryManager.Instance.GetTotalAmount(item);

        if (nextAmount <= 0)
            nextAmount = maxAmount;
        if (nextAmount > maxAmount)
            nextAmount = 1;

        inputAmountTMP.text = nextAmount.ToString();
        InputSetTotalAmount(item, nextAmount);
        inputTotalImage.sprite = UIManager.Instance.moneySprite;
    }

    private void InputSetTotalAmount(ItemSO item, int amount)
    {
        float price = context == Context.BUY ? item.BuyPrice : item.SellPrice;
        float totalAmount = price * amount;
        inputTotalTMP.text = totalAmount.ToString();
    }

    public void ConfirmInput(ItemSO item)
    {
        InventoryManager IM = InventoryManager.Instance;
        float itemAmount = int.Parse(inputAmountTMP.text);
        float totalMoneyAmount = int.Parse(inputTotalTMP.text);
        switch (context)
        {
            case Context.BUY:
                if (totalMoneyAmount > IM.moneyInPosesion)
                {
                    descriptionTMP.text = dontHaveEnoughMoneyText;
                    return;
                }
                IM.Trade(item, itemAmount, totalMoneyAmount, context);
                break;
            case Context.SELL:
                IM.Trade(item, itemAmount, totalMoneyAmount, context);
                break;
        }

        CancelInput();

        descriptionTMP.text = successfullTradeText;

    }

    public void CancelInput()
    {
        ActiveInputPanel(context, false);
        RefreshShop();
    }

    private void UpdateMainSpace()
    {
        UIManager.Clear(buyPanel);
        if (context == Context.BUY)
            InstantiateBuyElements();
        if (context == Context.SELL)
            InstantiateSellElements();
    }

    private void InstantiateItemsInShopSpace(List<ItemSO> itemsToInstantiate)
    {
        UIManager.Clear(buyPanel);

        for (int i = 0; i < itemsToInstantiate.Count; i++)
        {
            InstanceItemSlot(itemsToInstantiate[i], buyPanel.transform, true);
        }
    }

    private void InstanceItemSlot(ItemSO itemSO, Transform transform, bool enableButton)
    {
        float customPrice = context == Context.BUY ? itemSO.BuyPrice : itemSO.SellPrice;

        GameObject itemInstance = Instantiate(itemPrefab, transform);
        ItemSlot itemSlot = itemInstance.GetComponent<ItemSlot>();
        itemSlot.SetItem(itemSO);
        itemSlot.SetCustomPrice(customPrice);
        itemSlot.UpdateUI(ItemSlot.Context.SHOP);
        itemInstance.GetComponent<Button>().enabled = enableButton;
    }

    public void Exit()
    {
        ToogleShop(false);
        EventManager.Instance.ComunicateEnd(EventSO.GameEventType.SHOP);
    }

    #region Sell
    public void InstantiateSellElements()
    {
        List<ItemSO> itemsTI = new();
        itemsTI.AddRange(InventoryManager.Instance.items.Keys);

        context = Context.SELL;

        InstantiateItemsInShopSpace(itemsTI);


        characterSpace.SetActive(false);

    }
    #endregion

    #region Buy

    private void UpdateEquipUpdateCharacterPanel(EquipmentSO equip)
    {
        UIManager.Clear(equipUpdateCharacterSpace);
        List<GameObject> partyAllies = PartyManager.Instance.PartyGameObjects;

        for (int i = 0; i < partyAllies.Count; i++)
        {
            GameObject characterInstance = Instantiate(equipUpdateCharacterPrefab, equipUpdateCharacterSpace.transform);
            ShopEquipUpdateCharacter shopEquipUpdateCharacter = characterInstance.GetComponent<ShopEquipUpdateCharacter>();
            shopEquipUpdateCharacter.UpdateUI(partyAllies[i].GetComponent<PersonajeHandler>(), equip);
        }
    }

    public void InstantiateBuyElements()
    {

        context = Context.BUY;

        InstantiateItemsInShopSpace(items);

        characterSpace.SetActive(true);

    }

    internal void SetBuyList(List<ItemSO> itemsInShop)
    {
        items = itemsInShop;
    }
    #endregion
}
