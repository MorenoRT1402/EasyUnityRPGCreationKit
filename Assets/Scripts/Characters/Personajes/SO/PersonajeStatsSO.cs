using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Characters/Create Data/Stats")]
[Serializable]
public class PersonajeStatsSO : ScriptableObject
{
    private readonly PersonajeStats handler;

    [HideInInspector]
    public bool showInitialFields, showBaseFields;

    public Sprite spriteInMenu;

    public List<string> equipmentTypeEquipable;
    public List<EquipPartSO> equipParts;

    public List<EquipmentSO> initialEquipment;

    [Header("Stats Iniciales")]
    public string initialName;
    public int initialLevel, initialMaxLevel;
    public string LevelUpFormula;
    public float initialLife, initialMana, initialStamina, initialAttack, initialStrength, initialDexterity, initialMagicAttack, initialMind, initialDefense, initialMagicDefense, initialSpeed;

    public float initialCritProb, initialPrecision, initialEvasion;

    public int InitialExp;


    public BaseActiveSkill initialBasicAttack;

    public List<SkillBase> initialSkills;

    public SpecialActiveSkill InitialGuardSkill;
    public SpecialActiveSkill InitialWaitSkill;
    public SpecialActiveSkill InitialFleeSkill;

    public string ExpFormula = "0.0";
    public string MoneyFormula = "0.0";
    public string JobPointsFormula = "0.0";
    public List<PersonajeDropSO> drops;

    #region Getters

    #endregion

    public void ResetearValores()
    {
        handler.DefaultValues();
    }

}
