using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Skills/Skills/New Active Skill/Special")]
public class SpecialActiveSkill : AilmentActiveSkill
{
    [SerializeField] private BattleCommands specialCommand;
    private float timeToFinish;
    private float timer;
    NCalcManager NCM;
    TurnHandler actualTurn;

    public BattleCommands SpecialCommand => specialCommand;

    private void Awake()
    {
        initialize();
    }

        private void Start()
    {
        category = Category.SPECIAL;
    }

    private void initialize()
    {
        category = Category.SPECIAL;

        ailmentsHandler = new List<Ailment>();
        ailmentsHealHandler = new List<Ailment>();
        if (ailments != null && ailments.Count > 0)
            foreach (AilmentSO ailment in ailments)
                ailmentsHandler.Add(new Ailment(ailment));
        if (ailmentsHeal != null && ailmentsHeal.Count > 0)
            foreach (AilmentSO ailment in ailmentsHeal)
                ailmentsHealHandler.Add(new Ailment(ailment));
    }

    internal void Run(PersonajeHandler handler)
    {
        NCM = NCalcManager.Instance;
        actualTurn = BattleStateMachine.Instance.PerformList[0];
        FighterStateMachine character = handler.GetComponent<FighterStateMachine>();

        switch (specialCommand)
        {
            case BattleCommands.DEFEND:
                defend(character);
                break;
            case BattleCommands.ITEMS:
                break;
            case BattleCommands.FLEE:
                Flee(character);
                break;
            case BattleCommands.WAIT:
                wait(character);
                break;
        }
        ApplyAilmentsAndHeals(handler);
    }


    private void wait(FighterStateMachine character)
    {
        float result = NCM.ConvertToFloat(NCM.CalculateObject(actualTurn));
        NCM.CalculateObject(actualTurn);

        if (!character.Waiting)
        {
            character.Waiting = true;
            timer = 0;
            timeToFinish = result;
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > timeToFinish)
                character.Waiting = false;
        }
    }

    private void Flee(FighterStateMachine character)
    {

        float result = NCM.ConvertToFloat(NCM.CalculateObject(actualTurn));
//        Debug.Log(this);

        int num = Random.Range(0, 101);
//        Debug.Log($"{num} < {result} ? {num < result}");

        if (num < result)
            BattleStateMachine.Instance.Flee(character);
        else
            BattleManagerUI.Instance.Notify(BattleStateMachine.Instance.failedFleeText);
    }

    private void defend(FighterStateMachine character)
    {
        TurnHandler turnTemp = new TurnHandler(actualTurn.Attacker, actualTurn.Type, actualTurn.AttackersGameObject, actualTurn.AttackersTargets, this);
        float result = NCM.ConvertToFloat(NCM.CalculateObject(turnTemp));

        if (character.OnGuard)
        {
            float inverseResult = 1 / result;
            multAffinities(character, inverseResult);
            character.OnGuard = false;
        }
        else
        {
            multAffinities(character, result);
            character.OnGuard = true;
        }
    }

    private void multAffinities(FighterStateMachine character, float result)
    {
        PersonajeAffinities affinities = character.Hero.Affinities;

        foreach (Affinity affinity in Enum.GetValues(typeof(Affinity)))
        {

            affinities.Mult(affinity, result);

        }
    }

}
