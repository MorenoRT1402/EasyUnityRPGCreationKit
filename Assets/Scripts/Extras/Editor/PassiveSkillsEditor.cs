#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BasePassiveSkill))]
public class PassiveSkillsEditor : Editor
{
    private SerializedProperty Property;
    private BasePassiveSkill.Type type;

    private void OnEnable()
    {
        Property = serializedObject.FindProperty("type");
        type = (BasePassiveSkill.Type)Property.enumValueIndex;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(Property);

        ShowTypeVariables();

        EditorGUILayout.Space();

        type = (BasePassiveSkill.Type)Property.enumValueIndex;

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowTypeVariables()
    {
        switch (type)
        {
            case BasePassiveSkill.Type.NONE:
                break;
            case BasePassiveSkill.Type.STATS:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("modScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case BasePassiveSkill.Type.RESISTANCES:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affinities"));
//                EditorGUILayout.PropertyField(serializedObject.FindProperty("modScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
                break;
            case BasePassiveSkill.Type.AFFINITIES:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affinities"));
                break;
            case BasePassiveSkill.Type.SPECIAL:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("effect"));
                break;
        }
    }
}

#endif
