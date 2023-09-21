#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(AilmentSO))]
public class AilmentEditor : Editor
{
    AilmentSO AilmentSO => target as AilmentSO;
    private SerializedProperty ailmentTypeProperty;
    private AilmentType ailmentType;


    private void OnEnable()
    {
        ailmentTypeProperty = serializedObject.FindProperty("effect");
        ailmentType = (AilmentType)ailmentTypeProperty.enumValueIndex;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ailmentName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animation"));

        EditorGUILayout.PropertyField(ailmentTypeProperty);

        ShowEffectVariables();

        EditorGUILayout.Space();

        ailmentType = (AilmentType)ailmentTypeProperty.enumValueIndex;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("accurate"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeInSeconds"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("afterTakeDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("healProb"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("afterBattle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("afterDeath"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("secundaryAilments"));

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowEffectVariables()
    {
        switch (ailmentType)
        {
            case AilmentType.DAMAGE_PER_TIME:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePerInterval"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("interval"));
                break;
            case AilmentType.AFFINITY_MOD:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("affinityToModify"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaknessMult"));
                break;
            case AilmentType.STATS_MOD:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("statMult"));
                break;
            case AilmentType.CANT_MOVE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("actionProb"));
                break;
            case AilmentType.CONFUSION:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("faction"));
                break;
            case AilmentType.ACTION_INVALID:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("actionToInvalid"));
                break;
            case AilmentType.OUT_OF_BATTLE:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hpTo0"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leavesCombat"));
                break;
            case AilmentType.COUNTER:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("typeThatCounter"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("counterSkill"));
                if (AilmentSO.SkillOfCounter == AilmentSO.CounterSkill.CUSTOM)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("customSkill"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("counterProb"));
                break;
        }
    }
}

#endif
