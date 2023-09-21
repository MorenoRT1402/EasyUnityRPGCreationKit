using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dungeons/Create new Dungeon")]
public class DungeonSO : ScriptableObject
{
    [SerializeField] private string dungeonName;
    [SerializeField] private Sprite battleBackground;
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip ambiental;
    [SerializeField] private int numberOfSteps=0;
    [SerializeField] private List<AreaSO> randomEncounterAreas;

    public string DungeonName => dungeonName;
    public Sprite BattleBackground => battleBackground;
    public AudioClip Music => music;
    public AudioClip Ambiental => ambiental;
    public List<AreaSO> Areas => randomEncounterAreas;
    public int Steps => numberOfSteps;

    internal static DungeonSO CreateNew(string dungeonName, Sprite bgSprite, AudioClip music, AudioClip ambiental, int numberOfSteps, List<AreaSO> areas)
    {
        DungeonSO newDungeon = CreateInstance<DungeonSO>();

        newDungeon.dungeonName = dungeonName;
        newDungeon.battleBackground = bgSprite;
        newDungeon.music = music;
        newDungeon.ambiental = ambiental;
        newDungeon.numberOfSteps = numberOfSteps;
        newDungeon.randomEncounterAreas = areas;

        return newDungeon;
    }
}
