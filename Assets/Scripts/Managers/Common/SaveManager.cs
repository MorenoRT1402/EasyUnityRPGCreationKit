using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public enum Operation { NONE, SAVE, LOAD }
    [Header("Config")]
    public int saveSlots = 20;
    public string savePath = /*Application.persistentDataPath + */"saves/";
    public string saveFileName = "File ";
    public string saveFormat = ".sav";
    public int maxVariablesNumber = 256;
    public static List<EventData> eventDatas;
    public static float[] numericVariables;
    public static bool[] switches;
    public static string[] strings;

    private string completePath;

    private new void Awake()
    {
        base.Awake();
        completePath = Application.persistentDataPath + savePath;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        eventDatas ??= new();
        numericVariables ??= new float[maxVariablesNumber];
        switches ??= new bool[maxVariablesNumber];
        strings ??= new string[maxVariablesNumber];
    }

    internal string GetFullPath(int i)
    {
        return completePath + saveFileName + i + saveFormat;
    }

    public static void SaveData(SaveData data, string filePath)
    {
        BinaryFormatter formatter = new();

        string directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        FileStream stream = new(filePath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    internal static void SaveData(string filePath)
    {
        SaveData data = GetSaveData();

        SaveData(data, filePath);
    }

    internal static void LoadGame(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(filePath, FileMode.Open);

            SaveData loadedData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            LoadGame(loadedData);
        }
        else
        {
            if (DebugManager.Instance.keyDebugs) Debug.Log($"File in {filePath} it`s empty.");
        }
    }

    private static void LoadGame(SaveData loadedData)
    {
        GameManager.LoadGame(loadedData);
    }

    public static SaveData LoadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(filePath, FileMode.Open);

            SaveData loadedData = (SaveData)formatter.Deserialize(stream);
            stream.Close();

            return loadedData;
        }
        else
        {
            if (DebugManager.Instance.keyDebugs) Debug.Log($"Save file in {filePath} not found.");
            return null;
        }
    }

    internal SaveData[] GetSaveDatas()
    {
        SaveData[] saveDatas = new SaveData[saveSlots];
        for (int i = 0; i < saveSlots; i++)
            saveDatas[i] = LoadData(completePath + saveFileName + (i + 1) + saveFormat);
        return saveDatas;
    }

    internal static void ManageVariables(int index, AddRemove dataOperation, object value)
    {
        switch (dataOperation)
        {
            case AddRemove.ADD:
                if (value is bool boolValue)
                    AddVariableOperation(switches, index, boolValue);
                if (value is float floatValue)
                    AddVariableOperation(numericVariables, index, floatValue);
                if (value is string stringValue)
                    AddVariableOperation(strings, index, stringValue);
                break;
            case AddRemove.SET:
                if (value is bool boolSetValue)
                    SetVariableOperation(switches, index, boolSetValue);
                if (value is float floatSetValue)
                    SetVariableOperation(numericVariables, index, floatSetValue);
                if (value is string stringSetValue)
                    AddVariableOperation(strings, index, stringSetValue);
                break;
            case AddRemove.REMOVE:
                if (value is bool)
                    RemoveVariableOperation(switches, index);
                if (value is float)
                    RemoveVariableOperation(numericVariables, index);
                if (value is string)
                    RemoveVariableOperation(strings, index);
                break;
        }
    }

    private static void AddVariableOperation(float[] list, int index, float value)
    {
        list[index] += value;
    }
    private static void AddVariableOperation(string[] list, int index, string value)
    {
        list[index] += value;
    }
    private static void AddVariableOperation(bool[] list, int index, bool value)
    {
        list[index] = value ? list[index] : !list[index];
    }

    private static void SetVariableOperation<T>(T[] list, int index, T value)
    {
        list[index] = value;
    }

    private static void RemoveVariableOperation<T>(T[] list, int index)
    {
        list[index] = default;
    }

    public static void SaveEvents(bool playerInclude)
    {

        if (GameManager.InBattle || GameManager.InMainScreen || PartyManager.Instance.PartyGameObjects == null) return;

        //Locate all characterMovements in Scene
        List<CharacterMovement> characterMovements = new();
        characterMovements.AddRange(FindObjectsOfType<CharacterMovement>(true));

        //Save each of them
        if (characterMovements.Count > 0)
            for (int i = 0; i < characterMovements.Count; i++)
//                if (characterMovements[i].enabled)
                {
                    CharacterMovement cM = characterMovements[i];
                    EventData eventData = GetNewEventData(cM);
                    eventDatas.Add(eventData);
                }

        if (!playerInclude)
        {
            CharacterMovement leaderMovement = PartyManager.Instance.GetLeader().GetComponent<CharacterMovement>();
            EventData playerEventData = GetEventData(leaderMovement);
            playerEventData.SetPosition(SceneHandler.ActualPosition, 0);
        }

    }

    private static EventData GetEventData(CharacterMovement cM)
    {
        for (int i = 0; i < eventDatas.Count; i++){
            if (eventDatas[i].id == cM.id)
                return eventDatas[i];
        }
        return null;
    }

    private static EventData GetNewEventData(CharacterMovement cM)
    {
        Vector3 position = cM.transform.position;
        int routeIndex = 0;
        List<EventSO> eventList = new();
        if (cM is EventInteraction ei)
        {
            routeIndex = ei.DefaultRouteActualIndex;
            eventList = ei.ActionList;
        }
        KeyValuePair<Vector3, int> positionsDict = new(position, routeIndex);
        return EventData.CreateNew(cM.id, eventList, positionsDict);
    }

    public static void LoadEvents()
    {
        if (GameManager.InBattle) return;
        if (eventDatas.Count <= 0) return;

        List<CharacterMovement> characterMovements = new();
        characterMovements.AddRange(FindObjectsOfType<CharacterMovement>(true));

        if (characterMovements.Count <= 0) return;
        for (int i = 0; i < characterMovements.Count; i++)
        {
            CharacterMovement cM = characterMovements[i];
            if (cM.enabled)
            {
                for (int j = 0; j < eventDatas.Count; j++)
                {
                    EventData actualEventData = eventDatas[j];
                    if (actualEventData.id == cM.id)
                        LoadEventData(actualEventData, cM);
                }
            }
        }
    }

    private static void LoadEventData(EventData actualEventData, CharacterMovement cM)
    {
        float[] pos = actualEventData.position.Key;
        if (pos.Length < 2) return;
        Vector3 posVector = new(pos[0], pos[1], pos[2]);
        cM.transform.position = posVector;
        if (cM is EventInteraction ei)
        {
            ei.SetNewEventList(actualEventData.GetEventList());
            ei.DefaultRouteActualIndex = actualEventData.position.Value;
        }
    }

    internal static SaveData GetSaveData()
    {
        float[] numericVariables = SaveManager.numericVariables;
        bool[] switches = SaveManager.switches;
        string[] strings = SaveManager.strings;
        float money = InventoryManager.Instance.moneyInPosesion;
        SaveEvents(true);
        EventData[] positions = eventDatas.ToArray();
        DungeonSO actualDungeon = DungeonManager.Instance.actualDungeon;
        ItemInventoryData[] inventory = ItemInventoryData.GetInventory(InventoryManager.Instance.items);
        CharacterData[] mainPartyData = CharacterData.GetParty(PartyManager.Instance.PartyGameObjects);
        CharacterData[] benchPartyData = CharacterData.GetParty(PartyManager.Instance.BenchGameObjects);
        CharacterData[] outMembersData = CharacterData.GetParty(PartyManager.Instance.OutMembers);
        return new(numericVariables, switches, strings, money, positions, actualDungeon, inventory, mainPartyData, benchPartyData, outMembersData);
    }
}
