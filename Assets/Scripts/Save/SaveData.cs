using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData
{
    public string id;
    #region Global Variables
    public float[] numericVariables;
    public bool[] switches;
    public string[] strings;
    #endregion

    #region Player Data
    public int hoursPlaying, minutesPlaying, secondsPlaying;
    public float money;
    #endregion

    #region Position
    public EventData[] eventDatas;
    public DungeonData ubication;
    #endregion

    #region Inventory
    public ItemInventoryData[] inventoryData;
    #endregion

    #region Party
    public CharacterData[] mainPartyData, benchPartyData, outMemberDatas;
    #endregion

    #region MetaData
    public string savedScene;
    public int day, month, year;
    public int hour, min, sec;
    #endregion

    public SaveData() { }
    public SaveData(float[] numericVariables, bool[] switches, string[] strings, float money, EventData[] positions, DungeonSO ubication, ItemInventoryData[] inventory, CharacterData[] mainParty, CharacterData[] bench, CharacterData[] outMembers)
    {
        this.numericVariables = numericVariables;
        this.switches = switches;
        this.strings = strings;

        SetPlayTime();
        this.money = money;

        eventDatas = positions;
        this.ubication = new(ubication);

        inventoryData = inventory;

        mainPartyData = mainParty;
        benchPartyData = bench;
        outMemberDatas = outMembers;

        savedScene = SceneHandler.GetCurrentScene();
        SetDateTime(DateTime.Now);
    }

    #region Methods

    internal void Load()
    {
        SaveManager.numericVariables = numericVariables;
        SaveManager.switches = switches;
        SaveManager.strings = strings;
        GameManager.secsPlaying = hoursPlaying * 3600 + minutesPlaying * 60 + secondsPlaying;
        InventoryManager.Instance.moneyInPosesion = money;
        SaveManager.eventDatas = eventDatas.ToList();
        ubication.Load();
        foreach (ItemInventoryData itemInventoryData in inventoryData)
            itemInventoryData.Load();
        LoadCharacterDatas();

        SceneHandler.Instance.ChangeToScene(savedScene, false);
    }

    private void LoadCharacterDatas()
    {
        PartyManager PM = PartyManager.Instance;
        PM.Initialize();

        LoadCharacterData(mainPartyData, PM.PartyGameObjects);
        LoadCharacterData(benchPartyData, PM.BenchGameObjects);
        LoadCharacterData(outMemberDatas, PM.OutMembers);
    }

    private void LoadCharacterData(CharacterData[] partyData, List<PersonajeHandler> party)
    {
        foreach (CharacterData characterData in partyData)
            characterData.Load(party);
    }

    private void LoadCharacterData(CharacterData[] partyData, List<GameObject> party)
    {
        foreach (CharacterData characterData in partyData)
            characterData.Load(party);
            PartyManager.Instance.SetupPlayer();
    }

    internal DateTime GetDateTime()
    {
        return new DateTime(year, month, day, hour, min, sec);
    }
    private void SetDateTime(DateTime dateTime)
    {
        day = dateTime.Day;
        month = dateTime.Month;
        year = dateTime.Year;

        hour = dateTime.Hour;
        min = dateTime.Minute;
        sec = dateTime.Second;
    }

    internal int[] GetPlayTime()
    {
        return new[] { hoursPlaying, minutesPlaying };
    }

    private void SetPlayTime()
    {
        float secsPlaying = GameManager.secsPlaying;
        TimeSpan timeSpan = TimeSpan.FromSeconds(secsPlaying);
        hoursPlaying = timeSpan.Hours;
        minutesPlaying = timeSpan.Minutes;
        secondsPlaying = timeSpan.Seconds;
    }
    public PersonajeHandler GetLeader()
    {
        return mainPartyData[0].ToHandler();
    }
    #endregion
}
