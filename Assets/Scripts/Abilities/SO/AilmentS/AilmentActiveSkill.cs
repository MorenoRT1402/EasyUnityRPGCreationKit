using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Skills/Skills/New Active Skill/Ailment")]
public class AilmentActiveSkill : DamageActiveSkill
{
    [SerializeField] protected List<AilmentSO> ailments, ailmentsHeal;
    protected List<Ailment> ailmentsHandler, ailmentsHealHandler;

    private void OnEnable()
    {
        Initialize();
    }
    private void Start()
    {
        category = Category.AILMENT;
    }

    private void Initialize()
    {
        ailmentsHandler = new List<Ailment>();
        ailmentsHealHandler = new List<Ailment>();
        if (ailments != null && ailments.Count > 0)
            foreach (AilmentSO ailment in ailments)
                ailmentsHandler.Add(new Ailment(ailment));
        if (ailmentsHeal != null && ailmentsHeal.Count > 0)
            foreach (AilmentSO ailment in ailmentsHeal)
                ailmentsHealHandler.Add(new Ailment(ailment));
    }

    public void ApplyAilmentsAndHeals(PersonajeHandler handler)
    {
        if (ailmentsHandler != null)
            foreach (Ailment ailment in ailmentsHandler)
                Apply(ailment, handler);
        if (ailmentsHealHandler != null)
            foreach (Ailment ailmentHeal in ailmentsHealHandler)
                Heal(ailmentHeal, handler);
    }

    protected void Heal(Ailment ailmentHeal, PersonajeHandler handler)
    {
        List<Ailment> ailments = handler.Stats.Ailments;
        for (int a = 0; a < ailments.Count; a++)
            if (ailments[a] != null && ailmentHeal.Name == ailments[a].Name)
            {
                if (Random.Range(0, 101f) < ailmentHeal.Accurate)
                    ailments[a].EndOfAilment(handler);
            }
    }

    protected void Apply(Ailment ailment, PersonajeHandler handler)
    {
        List<Ailment> ailmentsOnFighter = handler.Stats.Ailments;

        if (Random.Range(0, 101f) < ailment.Accurate)
        {

            for (int a = 0; a < ailmentsOnFighter.Count; a++)
            {
                if (ailmentsOnFighter[a] != null && ailmentsOnFighter[a].Name == ailment.Name)
                {
                    ailmentsOnFighter[a].EndOfAilment(handler);
                    break;
                }
            }

            Ailment newAilment = new(ailment.SO);
            ailmentsOnFighter.Add(newAilment);
            newAilment.Initialize();
        }
    }

    public override void Exec(PersonajeHandler user, PersonajeHandler target)
    {
        base.Exec(user, target);
        ApplyAilmentsAndHealsEx(target);
    }

    private void ApplyAilmentsAndHealsEx(PersonajeHandler handler)
    {
        List<Ailment> ailmentsOnFighter = handler.Stats.Ailments;
        if (ailments.Count > 0)
        {
            for (int a = 0; a < ailments.Count; a++)
            {
                Ailment ailment = new Ailment(ailments[a]);

                if (Random.Range(0, 101f) < ailment.Accurate)
                {

                    for (int i = 0; i < ailmentsOnFighter.Count; i++)
                    {
                        if (ailmentsOnFighter[i] != null && ailmentsOnFighter[i].Name == ailment.Name)
                        {
                            ailmentsOnFighter[i].EndOfAilment(handler);
                            break;
                        }
                    }

                    Ailment newAilment = new Ailment(ailment.SO);
                    ailmentsOnFighter.Add(newAilment);
                    newAilment.Initialize();
                }
            }
        }
    }
}
