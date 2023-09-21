using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeons/Create new Area")]
public class AreaSO : ScriptableObject
{
    [SerializeField] private string areaID;
    [SerializeField] private LayerMask randomEncounterMask;
    [SerializeField] private List<EnemyGroupSO> enemyGroupsList;

    public LayerMask Layer => randomEncounterMask;
    public List<EnemyGroupSO> Enemys => enemyGroupsList;
}
