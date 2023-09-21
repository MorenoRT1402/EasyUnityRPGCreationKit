using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInFormation : MonoBehaviour
{
    private static GameObject characterSelected;
    public Image spriteImage;
    public Sprite emptySprite;

    private PersonajeHandler character;

    internal void Init()
    {
        spriteImage.sprite = emptySprite;
    }

    internal void SetData(PersonajeHandler character)
    {
        this.character = character;
    }

    internal void UpdateUI()
    {
        spriteImage.sprite = character.Stats.Character.SpriteInMenu;
    }

    public void ClickInFormation()
    {
        if (characterSelected == null)
            characterSelected = character.gameObject;
        else
        {
            if (characterSelected == character.gameObject)
                changeParty();
            else
                changePositions();
            characterSelected = null;
        }

        MenuManager.Instance.UpdateFormationPanel();
    }

    private void changePositions()
    {
        PartyManager.Instance.ChangePositions(characterSelected, character);
    }

    private void changeParty()
    {
        PartyManager.Instance.SwitchParty(character.gameObject);
    }
}
