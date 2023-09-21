using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatMod : MonoBehaviour
{
    public TextMeshProUGUI statNameTMP, oldValueTMP, newValueTMP;
    public Image arrow;

    public Color upColor = Color.green;
    public Color downColor = Color.red;

    private string statName;
    private float oldValue, newValue;

    internal void SetData(Stats name, float oldValue, float newValue)
    {
        statName = name.ToString();
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    internal void SetData(string name, float oldValue, float newValue)
    {
        statName = name;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    internal void UpdateUI(bool compare)
    {
        UpdateUI();
        Compare(compare);
    }

    private void Compare(bool compare)
    {
        arrow.gameObject.SetActive(compare);
        newValueTMP.gameObject.SetActive(compare);
    }

    private void UpdateUI()
    {
//        Debug.Log($"UpdateUi Name {statName} OldValue {oldValue}");
        statNameTMP.text = statName;
        oldValueTMP.text = oldValue.ToString();
        newValueTMP.text = newValue.ToString();

        Color colorToPaint = Color.white;
        if (oldValue > newValue) colorToPaint = downColor;
        else if (oldValue < newValue) colorToPaint = upColor;

        arrow.color = colorToPaint;
        newValueTMP.color = colorToPaint;
    }
}
