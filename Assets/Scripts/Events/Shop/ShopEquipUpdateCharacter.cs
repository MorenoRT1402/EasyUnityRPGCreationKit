using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopEquipUpdateCharacter : MonoBehaviour
{
    public Image upImage, equalsImage, downImage, characterImage;

    internal void UpdateUI(PersonajeHandler character, EquipmentSO equip)
    {
        characterImage.sprite = character.Stats.SpriteInMenu;

        float totalBalance = character.CompareTotalAtributes(equip);

        upImage.gameObject.SetActive(totalBalance > 0);
        equalsImage.gameObject.SetActive(totalBalance == 0);
        downImage.gameObject.SetActive(totalBalance < 0 && totalBalance > float.MinValue);
    }
}
