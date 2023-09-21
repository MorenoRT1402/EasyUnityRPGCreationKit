/*
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(PersonajeStats))]
public class PersonajeStatsEditor : Editor
{
    private enum ButtonFunction { CURAR_TODO, RESETEAR_VALORES }

    public PersonajeStatsSO StatsTarget => target as PersonajeStatsSO;

    // stats iniciales
    private SerializedProperty showInitialStatsProperty;
    private SerializedProperty nivelInicialProperty;
    private SerializedProperty nivelMaximoInicialProperty;
    private SerializedProperty vidaInicialProperty;
    private SerializedProperty manaInicialProperty;
    private SerializedProperty staminaInicialProperty;
    private SerializedProperty fuerzaInicialProperty;
    private SerializedProperty destrezaInicialProperty;
    private SerializedProperty menteInicialProperty;
    private SerializedProperty velocidadInicialProperty;
    private SerializedProperty probCritInicialProperty;
    private SerializedProperty precisionInicialProperty;
    private SerializedProperty experienciaInicialProperty;
    private SerializedProperty expRequeridaSiguienteNivelInicialProperty;
    private SerializedProperty multiplicadorExpReqInicialProperty;
    private SerializedProperty nombreInicialProperty;
    private SerializedProperty ataqueBasicoInicialProperty;
    private SerializedProperty habilidadesInicialesProperty;

    // stats base
    private SerializedProperty showBaseStatsProperty;
    private SerializedProperty nivelProperty;
    private SerializedProperty nivelMaximoProperty;
    private SerializedProperty vidaProperty;
    private SerializedProperty manaProperty;
    private SerializedProperty staminaProperty;
    private SerializedProperty fuerzaProperty;
    private SerializedProperty destrezaProperty;
    private SerializedProperty menteProperty;
    private SerializedProperty velocidadProperty;
    private SerializedProperty precisionProperty;
    private SerializedProperty probCritProperty;
    private SerializedProperty experienciaProperty;
    private SerializedProperty expRequeridaSiguienteNivelProperty;
    private SerializedProperty multiplicadorExpReqProperty;
    private SerializedProperty nombreProperty;
    private SerializedProperty ataqueBasicoProperty;
    private SerializedProperty habilidadesProperty;
    // Resto de las propiedades de los campos de Stats Iniciales...

    private void OnEnable()
    {
        valoresIniciales();
        valoresBase();

        // Resto de las asignaciones de propiedades de los campos de Stats Iniciales...
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        PersonajeStatsSO personajeStats = (PersonajeStatsSO)target;

        CrearBoton(ButtonFunction.CURAR_TODO, "Curar");

        showBaseStatsProperty.boolValue = EditorGUILayout.Toggle("Mostrar stats base", showBaseStatsProperty.boolValue);

        if (showBaseStatsProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nombreProperty);
            EditorGUILayout.PropertyField(nivelProperty);
            EditorGUILayout.PropertyField(nivelMaximoProperty);
            EditorGUILayout.PropertyField(vidaProperty);
            EditorGUILayout.PropertyField(manaProperty);
            EditorGUILayout.PropertyField(staminaProperty);
            EditorGUILayout.PropertyField(fuerzaProperty);
            EditorGUILayout.PropertyField(destrezaProperty);
            EditorGUILayout.PropertyField(menteProperty);
            EditorGUILayout.PropertyField(velocidadProperty);
            EditorGUILayout.PropertyField(precisionProperty);
            EditorGUILayout.PropertyField(probCritProperty);
            EditorGUILayout.PropertyField(experienciaProperty);
            EditorGUILayout.PropertyField(expRequeridaSiguienteNivelProperty);
            EditorGUILayout.PropertyField(multiplicadorExpReqProperty);
            EditorGUILayout.PropertyField(ataqueBasicoProperty);
            EditorGUILayout.PropertyField(habilidadesProperty);

            // Resto de los campos de Stats Iniciales...
            EditorGUI.indentLevel--;
        }

        CrearBoton(ButtonFunction.RESETEAR_VALORES, "Resetear valores");

        showInitialStatsProperty.boolValue = EditorGUILayout.Toggle("Mostrar stats iniciales", showInitialStatsProperty.boolValue);

        if (showInitialStatsProperty.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nombreInicialProperty);
            EditorGUILayout.PropertyField(nivelInicialProperty);
            EditorGUILayout.PropertyField(nivelMaximoInicialProperty);
            EditorGUILayout.PropertyField(vidaInicialProperty);
            EditorGUILayout.PropertyField(manaInicialProperty);
            EditorGUILayout.PropertyField(staminaInicialProperty);
            EditorGUILayout.PropertyField(fuerzaInicialProperty);
            EditorGUILayout.PropertyField(destrezaInicialProperty);
            EditorGUILayout.PropertyField(menteInicialProperty);
            EditorGUILayout.PropertyField(velocidadInicialProperty);
            EditorGUILayout.PropertyField(precisionInicialProperty);
            EditorGUILayout.PropertyField(probCritInicialProperty);
            EditorGUILayout.PropertyField(experienciaInicialProperty);
            EditorGUILayout.PropertyField(expRequeridaSiguienteNivelInicialProperty);
            EditorGUILayout.PropertyField(multiplicadorExpReqInicialProperty);
            EditorGUILayout.PropertyField(ataqueBasicoInicialProperty);
            EditorGUILayout.PropertyField(habilidadesInicialesProperty);

            // Resto de los campos de Stats Iniciales...
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void valoresBase()
    {
        showBaseStatsProperty = serializedObject.FindProperty("showBaseFields");
        nivelProperty = serializedObject.FindProperty("Nivel");
        nivelMaximoProperty = serializedObject.FindProperty("NivelMaximo");
        vidaProperty = serializedObject.FindProperty("Vida");
        manaProperty = serializedObject.FindProperty("Mana");
        staminaProperty = serializedObject.FindProperty("Stamina");
        fuerzaProperty = serializedObject.FindProperty("Fuerza");
        destrezaProperty = serializedObject.FindProperty("Destreza");
        menteProperty = serializedObject.FindProperty("Mente");
        velocidadProperty = serializedObject.FindProperty("Velocidad");
        precisionProperty = serializedObject.FindProperty("Precision");
        probCritProperty = serializedObject.FindProperty("CritProb");
        experienciaProperty = serializedObject.FindProperty("Experiencia");
        expRequeridaSiguienteNivelProperty = serializedObject.FindProperty("ExpRequeridaSiguienteNivel");
        multiplicadorExpReqProperty = serializedObject.FindProperty("MultiplicadorExpReq");
        nombreProperty = serializedObject.FindProperty("Nombre");
        ataqueBasicoProperty = serializedObject.FindProperty("AtaqueBasico");
        habilidadesProperty = serializedObject.FindProperty("Habilidades");
    }

    private void valoresIniciales()
    {
        showInitialStatsProperty = serializedObject.FindProperty("showInitialFields");
        nivelInicialProperty = serializedObject.FindProperty("NivelInicial");
        nivelMaximoInicialProperty = serializedObject.FindProperty("NivelMaximoInicial");
        vidaInicialProperty = serializedObject.FindProperty("VidaInicial");
        manaInicialProperty = serializedObject.FindProperty("ManaInicial");
        staminaInicialProperty = serializedObject.FindProperty("StaminaInicial");
        fuerzaInicialProperty = serializedObject.FindProperty("FuerzaInicial");
        destrezaInicialProperty = serializedObject.FindProperty("DestrezaInicial");
        menteInicialProperty = serializedObject.FindProperty("MenteInicial");
        velocidadInicialProperty = serializedObject.FindProperty("VelocidadInicial");
        precisionInicialProperty = serializedObject.FindProperty("PrecisionInicial");
        probCritInicialProperty = serializedObject.FindProperty("CritProbInicial");
        experienciaInicialProperty = serializedObject.FindProperty("ExperienciaInicial");
        expRequeridaSiguienteNivelInicialProperty = serializedObject.FindProperty("ExpRequeridaSiguienteNivelInicial");
        multiplicadorExpReqInicialProperty = serializedObject.FindProperty("MultiplicadorExpReqInicial");
        nombreInicialProperty = serializedObject.FindProperty("NombreInicial");
        ataqueBasicoInicialProperty = serializedObject.FindProperty("AtaqueBasicoInicial");
        habilidadesInicialesProperty = serializedObject.FindProperty("HabilidadesIniciales");
    }

    private void CrearBoton(ButtonFunction function, string buttonText)
    {
        if (GUILayout.Button(buttonText))
        {
            switch (function)
            {
                case ButtonFunction.CURAR_TODO:
                    StatsTarget.CurarTodo();
                    break;
                case ButtonFunction.RESETEAR_VALORES:
                    StatsTarget.ResetearValores();
                    break;
            }
        }
    }
}
*/
