using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Dungeons/Create new Enemy Group")]
public class EnemyGroupSO : ScriptableObject
{
    [SerializeField] private string groupID;
    [SerializeField] private List<PersonajeHandlerSO> fixedMembers;
    [SerializeField] private List<PersonajeHandlerSO> randomMembers;
    [SerializeField] private List<int> partyMembersCount;
    [SerializeField][Range(0, 100)] private float appearenceProb;

    private static int cont = 0;

    public float AppearenceProb => appearenceProb;
    public List<PersonajeHandlerSO> FinalMembers => GetFinalMembers();

    public static EnemyGroupSO CreateInstance(List<PersonajeHandlerSO> group)
    {
        EnemyGroupSO instance = CreateInstance<EnemyGroupSO>();
        instance.groupID = $"INS {cont++}";
        instance.fixedMembers = group;
        instance.appearenceProb = 0;

        return instance;
    }

    private List<PersonajeHandlerSO> GetFinalMembers()
    {
        List<PersonajeHandlerSO> finalMembers = new();
        if (partyMembersCount.Count <= 0) return fixedMembers;
        int count = GeneralManager.GetRandom(partyMembersCount);
        
        AddFixedMembers(finalMembers, count);
        AddRandomMembers(finalMembers, count);

        return finalMembers;
    }

    private void AddRandomMembers(List<PersonajeHandlerSO> list, int count)
    {
            for (int i = 0; i < count; i++)
            if (i > list.Count -1 || list[i] == null)
                list.Add(GeneralManager.GetRandom(randomMembers));
    }

    private void AddFixedMembers(List<PersonajeHandlerSO> list, int count)
    {
        for (int i = 0; i < count; i++)
            if (fixedMembers.Count > i)
                list.Add(fixedMembers[i]);
    }
}
