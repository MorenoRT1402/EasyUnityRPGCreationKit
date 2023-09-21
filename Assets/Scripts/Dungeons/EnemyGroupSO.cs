using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeons/Create new Enemy Group")]
public class EnemyGroupSO : ScriptableObject
{
    [SerializeField] private string groupID;
    [SerializeField] private List<PersonajeHandlerSO> members;
    [SerializeField] [Range(0, 100)] private float appearenceProb;

    private static int cont = 0;

    public float AppearenceProb => appearenceProb;
    public List<PersonajeHandlerSO> Members => members;

    public static EnemyGroupSO CreateInstance(List<PersonajeHandlerSO> group){
        EnemyGroupSO instance = CreateInstance<EnemyGroupSO>();
        instance.groupID = $"INS {cont++}";
        instance.members = group;
        instance.appearenceProb = 0;

        return instance;
    }
}
