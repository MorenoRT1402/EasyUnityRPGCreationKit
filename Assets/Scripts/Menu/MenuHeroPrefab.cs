using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHeroPrefab : MonoBehaviour
{
    [SerializeField] private PersonajeHandler hero;
    [SerializeField] private Image sprite;
    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI jobTMP;
    [SerializeField] private TextMeshProUGUI hpTMP, mpTMP, staminaTMP;
    [SerializeField] private Image hpBar, mpBar, staminaBar;

    public PersonajeHandler Hero => hero;

    internal void SetData(PersonajeHandler handler)
    {
        sprite.sprite = handler.Stats.SpriteInMenu;
        hero = handler;
    }

    internal void UpdateUI()
    {
        StatsHandler stats = hero.Stats;
        int levelShowed = MenuManager.Instance.LevelShowedInMenu == MenuManager.LevelShowed.CHARACTER_LEVEL ? stats.Character.Level : stats.Job.Level;

        nameTMP.text = stats.Name;
        if (levelTMP != null) levelTMP.text = levelShowed.ToString();
        jobTMP.text = stats.Job.Name;
        if (hpTMP != null) hpTMP.text = $"{stats.ActualLife}/{stats.MaxLife}";
        mpTMP.text = $"{stats.ActualMana}/{stats.MaxMana}";
        staminaTMP.text = $"{stats.ActualStamina}/{stats.MaxStamina}";

        float hpFill = (float)stats.ActualLife / stats.MaxLife;
        float mpFill = (float)stats.ActualMana / stats.MaxMana;
        float staminaFill = (float)stats.ActualStamina / stats.MaxStamina;

        // Set fillAmount for each Image (bar)
        if (hpBar != null) hpBar.fillAmount = hpFill;
        if (mpBar != null) mpBar.fillAmount = mpFill;
        if (staminaBar != null) staminaBar.fillAmount = staminaFill;
    }
}
