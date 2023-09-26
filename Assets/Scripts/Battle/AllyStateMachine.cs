using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AllyStateMachine : FighterStateMachine
{

    public AllyPanelStats PanelStats;

    //for the progressBar
    private Image atbBar;

    public Image ATBBar => atbBar;

    new void Start()
    {
        BMUI = BattleManagerUI.Instance;
        BSM = BattleStateMachine.Instance;

        base.Start();
        Faction = FighterTeam.ALLY;
        //create panel
        //        BMUI.setInterfaceElements(this);
        moveMarginX = -1.5f;
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
            case TurnState.ADD_TO_LIST:
                BSM.HerosToManage.Add(gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.SELECTING:
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
        BMUI.UpdateAllyPanel(this);

        if (ATB >= MaxATB)
        {
            if (CanMove)
                if (Confusion)
                    SetRandomTarget();
                else
                {//confusion == false
                    currentState = TurnState.ADD_TO_LIST;
                }
            else //canMove == false
            {
                ATB = 0;
                currentState = TurnState.PROCESSING;
            }

        }

    }


    public override void TakeDamage(float damage)
    {
        Hero.TakeDamage(damage);
        if (Hero.IsDead())
            Dead();
        BMUI.UpdateAllyPanel(this);
        //        BMUI.UpdateAllyPanel(hero);

    }

    internal void SetATBBar(Image atbBar)
    {
        this.atbBar = atbBar;
    }
}
