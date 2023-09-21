using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : FighterStateMachine
{

    new void Start()
    {
        base.Start();
        Faction = FighterTeam.ENEMY;
        //        animSpeed = hero.Velocidad;
        moveMarginX = 1.7f;
    }

    void Update()
    {
        TurnUpdate();
    }

    private new void TurnUpdate()
    {
        base.TurnUpdate();
        switch (currentState)
        {
            case TurnState.PROCESSING:
                UpgradeProgressBar();
                break;
            case TurnState.CHOOSE_ACTION:
                SetRandomTarget();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:

                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                UpdateDeadState();
                break;
        }
    }

    protected override void UpgradeProgressBar()
    {
        base.UpgradeProgressBar();


        if (ATB >= MaxATB && CanMove)
        {
            currentState = CanMove ? TurnState.CHOOSE_ACTION : TurnState.PROCESSING;
            ATB = 0;
        }
    }

    public override void TakeDamage(float damage)
    {
        Hero.TakeDamage(damage);
        if (Hero.IsDead())
        {
            Dead();
        }

    }

}
