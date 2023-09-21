using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Affinity { SLASH, CRUSH, PIERCE, NO_ELEMENTAL, FIRE, WATER, ELECTRIC, ICE, WIND, EARTH, LIGHT, DARK }


public class PersonajeAffinities
{
    [SerializeField] private PersonajeAffinitiesSO so;

    public PersonajeAffinitiesSO SO => so;

    #region Variables

    private List<Affinity> affinities;

    [Header("Actual Resistances")]
    private float actualSlash = 1;
    private float actualCrush = 1;
    private float actualPierce = 1;
    private float actualNoElemental = 1;
    private float actualFire = 1;
    private float actualWater = 1;
    private float actualElectric = 1;
    private float actualIce = 1;
    private float actualWind = 1;
    private float actualEarth = 1;
    private float actualLight = 1;
    private float actualDark = 1;

    private Dictionary<Affinity, float> actualResistances;

    [Header("Base Resistances")]
    private float slash = 1f;
    private float crush = 1f;
    private float pierce = 1f;
    private float noElemental = 1f;
    private float fire = 1f;
    private float water = 1f;
    private float electric = 1f;
    private float ice = 1f;
    private float wind = 1f;
    private float earth = 1f;
    private float light = 1f;
    private float dark = 1f;

    private Dictionary<Affinity, float> baseResistances;

    public List<Affinity> Affinities => affinities;
    public Dictionary<Affinity, float> ActualResistance => actualResistances;
    public Dictionary<Affinity, float> Resistances => baseResistances;

    public PersonajeAffinities(PersonajeAffinitiesSO affinitiesModel)
    {
        so = affinitiesModel;
        //        Debug.Log($"{so.GetInstanceID()} == {affinitiesModel.GetInstanceID()} ? {so.GetInstanceID()==affinitiesModel.GetInstanceID()}");
        InitResistances();
    }

    public PersonajeAffinities(PersonajeAffinitiesSO affinitiesSO, Dictionary<Affinity, float>[] resistances, Affinity[] elementalAffinities)
    {
        so = affinitiesSO;

        InitResistances();

        foreach (KeyValuePair<Affinity, float> pair in resistances[0])
            actualResistances[pair.Key] = pair.Value;

        foreach (KeyValuePair<Affinity, float> pair in resistances[1])
            baseResistances[pair.Key] = pair.Value;

            affinities = elementalAffinities.ToList();
    }


    #endregion

    #region Methods

    internal void Mod(Affinity affinity, ModScale modScale, float amount)
    {
        if (modScale == ModScale.SUM)
            Up(affinity, amount);
        else
            Mult(affinity, amount);
    }

    internal void Mult(Affinity affinity, float amount)
    {
        if (actualResistances.ContainsKey(affinity))
        {
            actualResistances[affinity] *= amount;
        }
    }

    internal float GetMult(Affinity affinity)
    {
        return actualResistances[affinity];
    }

    public void Up(Affinity affinity, float amount)
    {
        if (baseResistances.ContainsKey(affinity))
        {
            baseResistances[affinity] += amount;
        }
    }

    public void ResetValues()
    {
        if (so == null)
            return;

        else affinities = so.Affinities ?? new();

        slash = so.getInitial(Affinity.SLASH);
        crush = so.getInitial(Affinity.CRUSH);
        pierce = so.getInitial(Affinity.PIERCE);
        noElemental = so.getInitial(Affinity.NO_ELEMENTAL);
        fire = so.getInitial(Affinity.FIRE);
        water = so.getInitial(Affinity.WATER);
        electric = so.getInitial(Affinity.ELECTRIC);
        ice = so.getInitial(Affinity.ICE);
        wind = so.getInitial(Affinity.WIND);
        earth = so.getInitial(Affinity.EARTH);
        light = so.getInitial(Affinity.LIGHT);
        dark = so.getInitial(Affinity.DARK);
    }

    public void Restore()
    {
        foreach (KeyValuePair<Affinity, float> baseRes in baseResistances)
            actualResistances[baseRes.Key] = baseRes.Value;

        /*actualSlash = slash;
        actualCrush = crush;
        actualPierce = pierce;
        actualNoElemental = noElemental;
        actualFire = fire;
        actualWater = water;
        actualElectric = electric;
        actualIce = ice;
        actualWind = wind;
        actualEarth = earth;
        actualLight = light;
        actualDark = dark;*/
    }

    private void InitResistances()
    {
        ResetValues();

        InitBaseResistancesDict();
        InitActualResistancesDict();

        Restore();
    }

    private void InitActualResistancesDict()
    {
        actualResistances = new Dictionary<Affinity, float>()
        {
            { Affinity.SLASH, actualSlash },
            { Affinity.CRUSH, actualCrush },
            { Affinity.PIERCE, actualPierce },
            { Affinity.NO_ELEMENTAL, actualNoElemental },
            { Affinity.FIRE, actualFire },
            { Affinity.WATER, actualWater },
            { Affinity.ELECTRIC, actualElectric },
            { Affinity.ICE, actualIce },
            { Affinity.WIND, actualWind },
            { Affinity.EARTH, actualEarth },
            { Affinity.LIGHT, actualLight },
            { Affinity.DARK, actualDark }
        };
    }

    private void InitBaseResistancesDict()
    {
        baseResistances = new Dictionary<Affinity, float>()
        {
            { Affinity.SLASH, slash },
            { Affinity.CRUSH, crush },
            { Affinity.PIERCE, pierce },
            { Affinity.NO_ELEMENTAL, noElemental },
            { Affinity.FIRE, fire },
            { Affinity.WATER, water },
            { Affinity.ELECTRIC, electric },
            { Affinity.ICE, ice },
            { Affinity.WIND, wind },
            { Affinity.EARTH, earth },
            { Affinity.LIGHT, light },
            { Affinity.DARK, dark }
        };
    }

    internal void Divide(Affinity affinity, float amount)
    {
        if (actualResistances.ContainsKey(affinity))
        {
            actualResistances[affinity] /= amount;
        }
    }

    #endregion

}
