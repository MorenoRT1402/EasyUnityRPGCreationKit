using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public TextMeshProUGUI TargetName;
    private GameObject Selector;

    public void SelectEnemy()
    {
        BattleStateMachine.Instance.Input2(EnemyPrefab); //Save input enemy prefab
    }

    public void ToggleSelector()
    {
        Selector = EnemyPrefab.transform.Find("Selector").gameObject;
        Selector.SetActive(!Selector.activeSelf);
    }

}
