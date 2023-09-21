using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Saveslot : MonoBehaviour
{
    [Header("Glosary")]
    [SerializeField] private string playTimeText = "Time";

    [Header("Refereneces")]
    [SerializeField] private TextMeshProUGUI fileTMP;
    [SerializeField] private TextMeshProUGUI ubicationTMP;
    [SerializeField] private TextMeshProUGUI dayTMP;
    [SerializeField] private TextMeshProUGUI hourTMP;
    [SerializeField] private TextMeshProUGUI timeKeyTMP, timeValueTMP;
    [SerializeField] private TextMeshProUGUI leaderNameTMP;
    [SerializeField] private TextMeshProUGUI levelKeyTMP, levelValueTMP;
    [SerializeField] private GameObject membersSpace;
    [SerializeField] private GameObject spritePrefab;

    internal void UpdateUI(int i, DungeonData ubication, DateTime saveDate, int[] playTime, CharacterData[] mainPartyData)
    {
        UpdateUI(i);
        ubicationTMP.text = ubication.dungeonName;
        dayTMP.text = $"{saveDate.Day}/{saveDate.Month}/{saveDate.Year}";
        hourTMP.text = $"{saveDate.Hour}:{saveDate.Minute}";
        timeValueTMP.text = $"{playTime[0]}:{playTime[1]}";
        leaderNameTMP.text = mainPartyData[0].characterName;
        levelValueTMP.text = mainPartyData[0].level.ToString();
        InstanceMemberSprites(mainPartyData);
    }

    private void InstanceMemberSprites(CharacterData[] mainPartyData)
    {
        UIManager.Clear(membersSpace);
        for (int i = 0; i < mainPartyData.Length; i++)
            InstantiateSprite(mainPartyData[i]);
    }

    private void InstantiateSprite(CharacterData characterData)
    {
        GameObject prefab = Instantiate(spritePrefab, membersSpace.transform);
        prefab.GetComponent<Image>().sprite = characterData.GetSprite();
    }

    internal void UpdateUI(int i)
    {
        fileTMP.text = $"{SaveManager.Instance.saveFileName} {i}";
        SetEmpty();
    }

    private void SetEmpty()
    {
        ubicationTMP.text = "";
        dayTMP.text = "";
        hourTMP.text = "";
        timeKeyTMP.text = playTimeText;
        timeValueTMP.text = "";
        leaderNameTMP.text = "";
        levelKeyTMP.text = GeneralManager.Instance.levelShort;
        levelValueTMP.text = "";
        UIManager.Clear(membersSpace);
    }
}
