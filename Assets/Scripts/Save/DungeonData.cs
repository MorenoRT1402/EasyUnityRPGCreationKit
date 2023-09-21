using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonData
{
    public string dungeonName;
    public string battleBackgroundSpritePath;
    public string musicPath;
    public string ambientalPath;
    public int numberOfSteps=0;
    public List<string> randomEncounterAreaPaths;

    public DungeonData(){}

    public DungeonData(DungeonSO ubication)
    {
        dungeonName = ubication.DungeonName;
        battleBackgroundSpritePath = GeneralManager.ToPath(ubication.BattleBackground);
        musicPath = GeneralManager.ToPath(ubication.Music);
        ambientalPath = GeneralManager.ToPath(ubication.Ambiental);
        numberOfSteps = ubication.Steps;
        randomEncounterAreaPaths = GetAreasPaths(ubication.Areas);
    }

    
    internal void Load()
    {
        Sprite bgSprite = GeneralManager.ToSprite(battleBackgroundSpritePath);
        AudioClip music = GeneralManager.ToAudio(musicPath);
        AudioClip ambiental = GeneralManager.ToAudio(ambientalPath);
        List<AreaSO> areas = GeneralManager.To<AreaSO>(randomEncounterAreaPaths);

        DungeonSO dungeon = DungeonSO.CreateNew(dungeonName, bgSprite, music, ambiental, numberOfSteps, areas);
        DungeonManager.Instance.actualDungeon = dungeon;
    }

        private List<string> GetAreasPaths(List<AreaSO> areasList)
    {
        if(areasList == null) return new();
        List<string> areasPaths = new();
        for (int i = 0; i < areasList.Count; i++)
            areasPaths.Add(GeneralManager.ToPath(areasList[i]));
        return areasPaths;
    }

        public List<AreaSO> GetAreas()
    {
        List<AreaSO> areas = new();
        for (int i = 0; i < areas.Count; i++)
            areas.Add((AreaSO)GeneralManager.To(randomEncounterAreaPaths[i]));
        return areas;
    }
}
