using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : ScriptableObject
{
    [SerializeField] protected string skillName;
    [SerializeField] protected string description;

    public string Name => skillName;
    public string Description => description;
}
