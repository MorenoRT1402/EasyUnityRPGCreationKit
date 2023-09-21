using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Create Data/Growth/Skills")]
public class SkillsGrowthSO : ScriptableObject
{
    [SerializeField] private int[] learnLevels;
    [Tooltip("Indexes of Skills Per Level are equivalent to Learn Level Indexes")]
    [SerializeField] private List<SkillBase> skillPerLevel;

    internal void Learn(PersonajeStats character, int level)
    {
        List<SkillBase> levelSkillsList = GetSkills(level);
        if (levelSkillsList.Count > 0)
            for (int s = 0; s < levelSkillsList.Count; s++)
                character.AddSkill(levelSkillsList[s]);
    }

    internal void Learn(Job job, int level)
    {
        List<SkillBase> levelSkillsList = GetSkills(level);
        if (levelSkillsList.Count > 0)
            for (int s = 0; s < levelSkillsList.Count; s++)
                job.AddSkill(levelSkillsList[s]);
    }

    private List<SkillBase> GetSkills(int level)
    {
        List<SkillBase> levelSkills = new();
        if (learnLevels.Length <= 0 || skillPerLevel.Count <= 0) return levelSkills;

        for (int i = 0; i < learnLevels.Length; i++)
            if (learnLevels[i] == level && skillPerLevel.Count > i)
                levelSkills.Add(skillPerLevel[i]);
        return levelSkills;
    }
}
