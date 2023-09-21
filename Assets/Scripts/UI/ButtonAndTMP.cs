using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAndTMP : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI tmp;

    internal void UpdateUI(string text, List<EventSO> eventActions)
    {
        tmp.text = text;
        if (eventActions.Count > 0)
            button.onClick.AddListener(() => AddChoiceConsequence(eventActions));
    }

    internal void AddChoiceConsequence(List<EventSO> eventList)
    {
        GameManager.AddChoiceConsequences(eventList);
    }
}
