using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillButtonSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTMP;

    public TextMeshProUGUI NameTMP => nameTMP;

    private static PersonajeHandler _ally;
    private BaseActiveSkill _skill;

    private TextMeshProUGUI descriptionTMP, manaCostTMP, staminaCostTMP;

    public void skillChoosed()
    {
        BattleStateMachine BSM = BattleStateMachine.Instance;
        if (BSM != null && _ally.SufficientResources(_skill))
            BattleStateMachine.Instance.SkillInput(_skill);
    }
    public void UpdateUI(PersonajeHandler ally, BaseActiveSkill skill, TextMeshProUGUI descriptionTMP, TextMeshProUGUI allyNameTMP, TextMeshProUGUI allyManaTMP, TextMeshProUGUI allyStaminaTMP, TextMeshProUGUI manaCostTMP, TextMeshProUGUI staminaCostTMP)
    {
        nameTMP.text = skill.Name;
        descriptionTMP.text = skill.Description;
        allyNameTMP.text = ally.Stats.Name;
        allyManaTMP.text = ally.Stats.ActualMana.ToString();
        allyStaminaTMP.text = ally.Stats.ActualStamina.ToString();
        manaCostTMP.text = (skill.manaCost * -1).ToString();
        staminaCostTMP.text = (skill.staminaCost * -1).ToString();
    }
    public void UpdateUI(TextMeshProUGUI descriptionTMP, TextMeshProUGUI manaCostTMP, TextMeshProUGUI staminaCostTMP)
    {
        SetData(descriptionTMP, manaCostTMP, staminaCostTMP);
        updateTexts();

    }

    public void UpdateUI() //When Pointer Enter and Exit
    {
        BattleManagerUI BMUI = BattleManagerUI.Instance;
        nameTMP.text = _skill.Name;
        if (BMUI != null) BMUI.UpdateSkillPanelUI(_ally, _skill);
        else
            updateTexts();
    }

    private void updateTexts()
    {
        nameTMP.text = _skill.Name;
        descriptionTMP.text = _skill.Description;
        manaCostTMP.text = (_skill.manaCost * -1).ToString();
        staminaCostTMP.text = (_skill.staminaCost * -1).ToString();

        /*
        manaCostTMP.gameObject.SetActive(manaCostTMP.text == "0");
        staminaCostTMP.gameObject.SetActive(staminaCostTMP.text == "0");
*/
    }

    public static void SetAlly(PersonajeHandler ally)
    {
        _ally = ally;
    }

    public void setSkill(BaseActiveSkill skill)
    {
        _skill = skill;
    }

    internal void setData(PersonajeHandler character, BaseActiveSkill skill)
    {
        SetAlly(character);
        setSkill(skill);
    }

    internal void SetData(TextMeshProUGUI descriptionTMP, TextMeshProUGUI manaCostTMP, TextMeshProUGUI staminaCostTMP)
    {
        this.descriptionTMP = descriptionTMP;
        this.manaCostTMP = manaCostTMP;
        this.staminaCostTMP = staminaCostTMP;
    }
}
