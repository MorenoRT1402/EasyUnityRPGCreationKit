using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersonajeAffinitiesSO))]
public class PersonajeAffinitiesEditor : Editor
{
    bool showInitial, showBase;

    private enum ButtonFunction
    {
        RESTAURAR, RESETEAR_VALORES,
        DEFAULT,
        RANDOM
    }

    public PersonajeAffinitiesSO Target => target as PersonajeAffinitiesSO;

    // initial
    private SerializedProperty initialSlashProperty, initialCrushProperty, initialPierceProperty,
    initialNoElementalProperty, initialFireProperty, initialWaterProperty, initialElectricProperty, initialIceProperty,
    initialWindProperty, initialEarthProperty, initialLightProperty, initialDarkProperty;

    // base
    private SerializedProperty SlashProperty, CrushProperty, PierceProperty,
    NoElementalProperty, FireProperty, WaterProperty, ElectricProperty, IceProperty,
    WindProperty, EarthProperty, LightProperty, DarkProperty;

    private void OnEnable()
    {
        InitialValues();

    }

    public override void OnInspectorGUI()
    {
        InitialValues();
        serializedObject.Update();
        DrawDefaultInspector();

        PersonajeAffinitiesSO personajeAffinitiesSO = (PersonajeAffinitiesSO)target;

//        showInitial = EditorGUILayout.Toggle("Mostrar afinidades iniciales", showInitial);

//        showElementsInitial(showInitial);
        
        EditorGUILayout.Space();

        CrearBoton(ButtonFunction.DEFAULT, "Default");

        CrearBoton(ButtonFunction.RANDOM, "Random");

        serializedObject.ApplyModifiedProperties();
    }

    private void showElementsInitial(bool show)
    {
        if (show)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(initialSlashProperty);
            EditorGUILayout.PropertyField(initialCrushProperty);
            EditorGUILayout.PropertyField(initialPierceProperty);
            EditorGUILayout.PropertyField(initialNoElementalProperty);
            EditorGUILayout.PropertyField(initialFireProperty);
            EditorGUILayout.PropertyField(initialWaterProperty);
            EditorGUILayout.PropertyField(initialElectricProperty);
            EditorGUILayout.PropertyField(initialIceProperty);
            EditorGUILayout.PropertyField(initialWindProperty);
            EditorGUILayout.PropertyField(initialEarthProperty);
            EditorGUILayout.PropertyField(initialLightProperty);
            EditorGUILayout.PropertyField(initialDarkProperty);

            // Resto de los campos de Stats Iniciales...
            EditorGUI.indentLevel--;
        }
    }

    private void showElementsBase(bool show)
    {
        if (show)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(SlashProperty);
            EditorGUILayout.PropertyField(CrushProperty);
            EditorGUILayout.PropertyField(PierceProperty);
            EditorGUILayout.PropertyField(NoElementalProperty);
            EditorGUILayout.PropertyField(FireProperty);
            EditorGUILayout.PropertyField(WaterProperty);
            EditorGUILayout.PropertyField(ElectricProperty);
            EditorGUILayout.PropertyField(IceProperty);
            EditorGUILayout.PropertyField(WindProperty);
            EditorGUILayout.PropertyField(EarthProperty);
            EditorGUILayout.PropertyField(LightProperty);
            EditorGUILayout.PropertyField(DarkProperty);

            // Resto de los campos de Stats Iniciales...
            EditorGUI.indentLevel--;
        }
    }

    private void valoresBase()
    {
        SlashProperty = serializedObject.FindProperty("slash");
        CrushProperty = serializedObject.FindProperty("crush");
        PierceProperty = serializedObject.FindProperty("pierce");
        NoElementalProperty = serializedObject.FindProperty("noElemental");
        FireProperty = serializedObject.FindProperty("fire");
        WaterProperty = serializedObject.FindProperty("water");
        ElectricProperty = serializedObject.FindProperty("electric");
        IceProperty = serializedObject.FindProperty("ice");
        WindProperty = serializedObject.FindProperty("wind");
        EarthProperty = serializedObject.FindProperty("earth");
        LightProperty = serializedObject.FindProperty("light");
        DarkProperty = serializedObject.FindProperty("dark");

    }

    private void InitialValues()
    {
        initialSlashProperty = serializedObject.FindProperty("initialSlash");
        initialCrushProperty = serializedObject.FindProperty("initialCrush");
        initialPierceProperty = serializedObject.FindProperty("initialPierce");
        initialNoElementalProperty = serializedObject.FindProperty("initialNoElemental");
        initialFireProperty = serializedObject.FindProperty("initialFire");
        initialWaterProperty = serializedObject.FindProperty("initialWater");
        initialElectricProperty = serializedObject.FindProperty("initialElectric");
        initialIceProperty = serializedObject.FindProperty("initialIce");
        initialWindProperty = serializedObject.FindProperty("initialWind");
        initialEarthProperty = serializedObject.FindProperty("initialEarth");
        initialLightProperty = serializedObject.FindProperty("initialLight");
        initialDarkProperty = serializedObject.FindProperty("initialDark");
    }

    private void CrearBoton(ButtonFunction function, string buttonText)
    {
        if (GUILayout.Button(buttonText))
        {
            switch (function)
            {
                case ButtonFunction.DEFAULT:
                    Target.DefaultValues();
                    break;
                case ButtonFunction.RANDOM:
                    Target.RandomValues();
                    break;
            }
        }
    }
}
