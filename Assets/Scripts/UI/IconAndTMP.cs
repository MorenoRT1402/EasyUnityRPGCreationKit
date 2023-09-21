using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconAndTMP : MonoBehaviour
{
    public enum FORMAT { EQUIPMENT }

    public Image icon;
    public TextMeshProUGUI tmp;

    private Sprite sprite;
    private string text;

    internal void SetData(Sprite sprite, string text)
    {
        if (sprite != null)
            this.sprite = sprite;
        this.text = text;
    }

    internal void SetData(string text)
    {
        this.text = text;

    }

    internal void UpdateUI(FORMAT format)
    {
        switch (format)
        {
            case FORMAT.EQUIPMENT:
                equipFormat();
                break;
        }

        UpdateUI();

    }

    private void UpdateUI()
    {
        if (sprite != null)
            icon.sprite = sprite;
        tmp.text = text;
    }

    private void equipFormat()
    {
        text = text == null || text == "" ? "Empty" : text;
    }
}
