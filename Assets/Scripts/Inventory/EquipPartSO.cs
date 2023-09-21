using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equip/Create Equip Part")]
public class EquipPartSO : ScriptableObject
{
    public string equipPartName;
    public List<string> equipAvailables;
}
