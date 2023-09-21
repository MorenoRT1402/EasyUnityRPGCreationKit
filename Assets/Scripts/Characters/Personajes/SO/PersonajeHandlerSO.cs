using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Create new Character")]
public class PersonajeHandlerSO : ScriptableObject
{
    public PersonajeStatsSO stats;
    public StatsGrowthSO statsGrowth;
    public SkillsGrowthSO skillsLearn;
    public JobSO job;
    public PersonajeAffinitiesSO affinities;
    public RuntimeAnimatorController animationSheet;
}
