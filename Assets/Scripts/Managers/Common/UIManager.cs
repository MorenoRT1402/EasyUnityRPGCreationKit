using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("General")]
    public Sprite generalTheme;
    public Sprite moneySprite;

    public Color defaultTextColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;

    [Header("NPC")]
    [Header("NPC/Interaction")]
    public Sprite npcInteractionButtonSprite;
    public bool showNPCInteractKey = false;
    public Color npcInteractKeyColor = Color.black;
    [Header("NPC/Routes")]
    public Color routeSpheresColor = Color.blue;
    public Color routeSphereHandleColor = Color.red;
    public Color routeLinesColor = Color.gray;
    public float routeSpheresRadius = 0.5f;
    public float routeSphereHandleRadius = 0.7f;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    internal void SetGeneralTheme(GameObject container)
    {
        if (container.TryGetComponent<Image>(out var image)) image.sprite = generalTheme;
        else throw new Exception($"Image component not found in {container.name}: {container.GetInstanceID()}");
    }

    public static void Clear(GameObject parent)
    {
        // Destroy all child objects of the spacer GameObject.
        foreach (Transform child in parent.transform)
        {
            // Check if the child object has a RectMask2D component.
            if (!LayoutGroupElement(child))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private static bool LayoutGroupElement(Transform child)
    {
        if (child.GetComponent<RectMask2D>() != null || child.GetComponent<ContentSizeFitter>() != null)
            return true;
        return false;
    }
    #region Save
    internal static void UpdateSavePanel(GameObject saveSlotPrefab, GameObject saveSlotSpacer, SaveManager.Operation operation)
    {
        Clear(saveSlotSpacer);

        SaveData[] saveDatas = SaveManager.Instance.GetSaveDatas();
        for (int i = 0; i < saveDatas.Length; i++)
        {
            InstanceSaveSlot(saveDatas[i], i + 1, saveSlotPrefab, saveSlotSpacer, operation);
        }
    }

    private static void InstanceSaveSlot(SaveData saveData, int i, GameObject saveSlotPrefab, GameObject saveSlotSpacer, SaveManager.Operation operation)
    {
        GameObject slotPrefab = Instantiate(saveSlotPrefab, saveSlotSpacer.transform);
        Saveslot saveslot = slotPrefab.GetComponent<Saveslot>();
        if (saveData == null)
            saveslot.UpdateUI(i);
        else
            saveslot.UpdateUI(i, saveData.ubication, saveData.GetDateTime(), saveData.GetPlayTime(), saveData.mainPartyData);
        Button slotBtn = saveslot.GetComponent<Button>();
        if (operation == SaveManager.Operation.SAVE)
            slotBtn.onClick.AddListener(() => MenuManager.Instance.Save(i));
        else if (operation == SaveManager.Operation.LOAD)
            slotBtn.onClick.AddListener(() => MainScreenManager.Instance.Load(i));

    }
    #endregion
}
