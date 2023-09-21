using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new drop")]
public class PersonajeDropSO : ScriptableObject
{
    public ItemSO item;
    [Range(0,100f)] public float probDrop;
    public int quantity;

    public PersonajeDropSO Clone(ItemSO item, float probDrop, int quantity)
{
    PersonajeDropSO clone = ScriptableObject.CreateInstance<PersonajeDropSO>();
    clone.item = item;
    clone.probDrop = probDrop;
    clone.quantity = quantity;
    return clone;
}

}
