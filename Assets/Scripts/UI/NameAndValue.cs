using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameAndValue : MonoBehaviour
{

    private string key;
    private string value;

    public TextMeshProUGUI keyTMP;
    public TextMeshProUGUI valueTMP;

    internal void SetData(string key, string value)
    {
        this.key = key;
        this.value = value;
    }

    internal void UpdateUI()
    {
        keyTMP.text = key;
        valueTMP.text = value;
    }
}
