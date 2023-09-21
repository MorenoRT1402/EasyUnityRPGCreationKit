using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Characters/Create Data/Affinities")]
public class PersonajeAffinitiesSO : ScriptableObject
{
    #region Variables

    private readonly float defaultValues = 1f;

    //    [Header("Affinities")]
    [SerializeField] List<Affinity> affinities;

    //    [HideInInspector]
    [Header("Initial Resistances")]

    private Dictionary<Affinity, float> initialResistances;

    [SerializeField] private float initialSlash;
    [SerializeField] private float initialCrush;
    [SerializeField] private float initialPierce;
    [SerializeField] private float initialNoElemental;
    [SerializeField] private float initialFire;
    [SerializeField] private float initialWater;
    [SerializeField] private float initialElectric;
    [SerializeField] private float initialIce;
    [SerializeField] private float initialWind;
    [SerializeField] private float initialEarth;
    [SerializeField] private float initialLight;
    [SerializeField] private float initialDark;

    #endregion

    public List<Affinity> Affinities => affinities;

    private Dictionary<Affinity, float> resistances;

    private void OnEnable()
    {
        InitResistances();
    }

    internal float getInitial(Affinity affinity)
    {
        if (initialResistances == null)
            InitResistances();
        return initialResistances[affinity];
    }

    private void InitResistances()
    {

        initialResistances = new Dictionary<Affinity, float>()
        {
            { Affinity.SLASH, initialSlash },
            { Affinity.CRUSH, initialCrush },
            { Affinity.PIERCE, initialPierce },
            { Affinity.NO_ELEMENTAL, initialNoElemental },
            { Affinity.FIRE, initialFire },
            { Affinity.WATER, initialWater },
            { Affinity.ELECTRIC, initialElectric },
            { Affinity.ICE, initialIce },
            { Affinity.WIND, initialWind },
            { Affinity.EARTH, initialEarth },
            { Affinity.LIGHT, initialLight },
            { Affinity.DARK, initialDark }
        };
    }

    /*
        public void DefaultValues()
        {
            //    InitResistances();
            List<Affinity> keys = new List<Affinity>(initialResistances.Keys);
            foreach (Affinity affinity in keys)
            {
                initialResistances[affinity] = defaultValues;
                Debug.Log(initialResistances[affinity]);
            }
        }
        */

    public void DefaultValues()
    {

        initialSlash = defaultValues;
        initialCrush = defaultValues;
        initialPierce = defaultValues;
        initialNoElemental = defaultValues;
        initialFire = defaultValues;
        initialWater = defaultValues;
        initialElectric = defaultValues;
        initialIce = defaultValues;
        initialWind = defaultValues;
        initialEarth = defaultValues;
        initialLight = defaultValues;
        initialDark = defaultValues;

    }

    /*
        public void RandomValues()
        {
            List<Affinity> keys = new List<Affinity>(initialResistances.Keys);

            foreach (Affinity affinity in keys)
            {
                RandomValues(affinity);
            }
        }
        */

    public void RandomValues()
    {

        initialSlash = GetRandomValues();
        initialCrush = GetRandomValues();
        initialPierce = GetRandomValues();
        initialNoElemental = GetRandomValues();
        initialFire = GetRandomValues();
        initialWater = GetRandomValues();
        initialElectric = GetRandomValues();
        initialIce = GetRandomValues();
        initialWind = GetRandomValues();
        initialEarth = GetRandomValues();
        initialLight = GetRandomValues();
        initialDark = GetRandomValues();

    }

    public float GetRandomValues()
    {
        float randomValue = Random.Range(0f, 2f);
        float roundedValue = Mathf.Round(randomValue * 100f) / 100f; // Round to 2 decimals
        return roundedValue;
    }

    public void SetRandomValues(Affinity affinity)
    {

        if (initialResistances.ContainsKey(affinity))
        {
            float randomValue = Random.Range(0f, 2f);
            float roundedValue = Mathf.Round(randomValue * 100f) / 100f; // Round to 2 decimals

            initialResistances[affinity] *= roundedValue;
        }
    }
}
