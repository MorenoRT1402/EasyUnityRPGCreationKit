#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EventSO))]
public class EventSOEditor : Editor
{
    private SerializedProperty eventSOProperty;
    private EventSO.GameEventType gameEventType;

    public EventSO Target => target as EventSO;

    private void OnEnable()
    {
        eventSOProperty = serializedObject.FindProperty("eventType");
        gameEventType = (EventSO.GameEventType)eventSOProperty.enumValueIndex;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(eventSOProperty);

        gameEventType = (EventSO.GameEventType)eventSOProperty.enumValueIndex;

        ShowEventTypeVariables();

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowEventTypeVariables()
    {
        switch (gameEventType)
        {
            case EventSO.GameEventType.NONE:
                break;
            case EventSO.GameEventType.CONVERSATION:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("conversation"));
                break;
            case EventSO.GameEventType.CHOICES:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("choicePretext"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("choicesText"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("choicesConsequences"));
                break;
            case EventSO.GameEventType.CONDITIONS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"));
                ShowConditions();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("positiveConsequence"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("negativeConsequence"));
                break;
            case EventSO.GameEventType.INVENTORY:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case EventSO.GameEventType.SHOP:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemsInShop"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canBuy"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canSell"));
                break;
            case EventSO.GameEventType.TELEPORT:
                if (Target.objectToTeleportName == "")
                {
                    if (!Target.teleportPlayer)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleportThis"));
                    if (!Target.teleportThis)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleportPlayer"));
                }
                if (!Target.teleportThis && !Target.teleportPlayer)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("objectToTeleportName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("destinationScene"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPosition"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("destinationDungeon"));
                break;
            case EventSO.GameEventType.BATTLE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyGroup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canEscape"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canBeDefeated"));
                break;
            case EventSO.GameEventType.HEAL:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("healAll"));
                if (!Target.healAll)
                    SetCharacterChooseMethods();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lifeHealPercent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("manaHealPercent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("staminaHealPercent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("statusHeal"));
                break;
            case EventSO.GameEventType.MODIFY_CHARACTER:
                SetCharacterChooseMethods();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("aspectToModify"));
                ShowModCharacterOptions();
                break;
            case EventSO.GameEventType.CHANGE_PARTY:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("changeOption"));
                SetCharacterChooseMethods();
                if (Target.changeOption == AddRemove.ADD)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("resetStats"));
                break;
            case EventSO.GameEventType.ROUTE:
                ShowRouteOptions();
                break;
            case EventSO.GameEventType.CHANGE_EVENT_LIST:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newEventList"));
                break;
            case EventSO.GameEventType.CHANGE_MENU_OPTIONS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("menuOption"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enable"));
                break;
            case EventSO.GameEventType.MOD_EVENT:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eventRoute"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eventTrigger"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("component"));
                if (Target.component == GameObjectComponent.SPRITE_RENDERER)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("newSprite"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableComponent"));
                break;
            case EventSO.GameEventType.AUDIO:
                ShowAudioOptions();
                break;
            case EventSO.GameEventType.STORE_DATA:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("variableType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dataOperation"));
                if (Target.dataOperation == AddRemove.ADD || Target.dataOperation == AddRemove.SET)
                {
                    if (Target.variableType == EventSO.VariableType.SWITCH)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("switchOn"));
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
                        if (Target.variableType != EventSO.VariableType.STRING)
                            SetCharacterChooseMethods();
                    }
                }
                break;
        }
    }

    private void ShowConditions()
    {
        switch (Target.condition)
        {
            case EventSO.Conditions.SWITCH:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("switchOn"));
                break;
            case EventSO.Conditions.VARIABLE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("logicalOperation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
                SetCharacterChooseMethods();
                break;
            case EventSO.Conditions.HAVE_ITEM:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("logicalOperation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case EventSO.Conditions.ITEM_EQUIPPED:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("equipped"));
                SetCharacterChooseMethods();
                break;
        }
    }

    private void ShowAudioOptions()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("option"));
        switch (Target.option)
        {
            case EventSO.AudioOptions.PLAY:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("audio"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
                break;

            case EventSO.AudioOptions.CHANGE_VOLUME:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
                break;
            case EventSO.AudioOptions.PAUSE:
            case EventSO.AudioOptions.SAVE:
            case EventSO.AudioOptions.REPLAY:
                break;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("audioType"));

    }

    private void ShowRouteOptions()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("directions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("values"));
        EditorGUILayout.EndHorizontal();
    }

    private void ShowModCharacterOptions()
    {
        switch (Target.aspectToModify)
        {
            case EventSO.CharacterModifications.TOTAL:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newCharacter"));
                break;
            case EventSO.CharacterModifications.NAME:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newName"));
                break;
            case EventSO.CharacterModifications.JOB:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newJob"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("resetStats"));
                break;
            case EventSO.CharacterModifications.STATS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("statToModify"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("modScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case EventSO.CharacterModifications.AFFINITIES:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affinityToModify"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("modScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case EventSO.CharacterModifications.EQUIPMENT:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("equipItem"));
                break;
            case EventSO.CharacterModifications.ANIMATIONS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newAnimations"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("newSpriteInMenu"));
                break;
            case EventSO.CharacterModifications.SKILLS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("changeOption"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skills"));
                break;
        }
    }

    private void SetCharacterChooseMethods()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("character"));
        if (Target.character == null)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allyCharacterIndex"));
    }
}

#endif
