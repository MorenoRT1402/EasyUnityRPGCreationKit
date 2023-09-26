using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum AddRemove
{
    NONE, ADD, REMOVE,
    SET
}
public enum MathOperations { SUM, MINUS, MULT, DIV }
public enum LogicalOperations { GREATER, LESSER, EQUAL }
public class GeneralManager : Singleton<GeneralManager>
{
    //    [Header("Player")]
    //    public float walkingSpeed = 5f;
    [Header("Characters")]
    public AilmentSO DeathState;
    public bool allowPassiveSkillsDuplicates = true;


    public bool UseRecomendedExpFormula = false;
    #region Recomended Exp Formula

    //Formula:
    /*
    exp(level) = (level ^ c1) * (level ^ c2)

    c1 depends on how long you want each level to take
    c2 depends on how close you want to keep the player to the level of the enemies they fight
    */

    public float C1 = 1.1f;
    public float C2 = 0.7f;
    #endregion

    [Header("Glosary")]
    public string currencyName = "Gold";
    [Header("Glosary/Menu Options")]
    public string itemsName = "Items";
    public string skillsName = "Skills";
    public string equipName = "Equip";
    public string statusName = "Status";
    public string formationName = "Formation";
    public string optionsName = "Options";
    public string saveName = "Save";
    public string exitName = "Return To Title";
    [Header("Glosary/Stats")]
    public string level = "Level";
    public string levelShort = "Lv";
    public string characterExpShort = "EXP";
    public string jobLevel = "Job Level";
    public string jobLevelShort = "Lv";
    public string jobExpShort = "JP";
    public string hp = "HP";
    public string maxHP = "MaxHP";
    public string maxHPShort = "HP";
    public string mp = "MP";
    public string maxMP = "MaxMP";
    public string maxMPShort = "MP";
    public string stamina = "Stamina";
    public string MaxStamina = "MaxStamina";
    public string maxStaminaShort = "STA";
    public string attack = "Attack";
    public string strength = "Strength";
    public string defense = "Defense";
    public string magicAttack = "Magic Attack";
    public string mind = "Mind";
    public string magicDefense = "Magic Defense";
    public string dexterity = "Dexterity";
    public string speed = "Speed";
    public string precision = "Precision";
    public string evasion = "Evasion";
    public string critProb = "Crit%";

    [Header("Parameters")]
    [Header("Parameters/Initial Menu Options Available")]

    public bool items = true;
    public bool skills = true;
    public bool equip = true;
    public bool status = true;
    public bool formation = true;
    public bool options = true;
    public bool save = true;
    public bool exit = true;

    public Dictionary<MenuOptions, object[]> menuOptionDict;

    [Header("Parameters/Stats Weight")]
    public float maxHPW = 10;
    public float maxMPW = 5;
    public float maxStaminaW = 3;
    public float maxAttackW = 1;
    public float maxStrengthW = 1;
    public float maxDefenseW = 1;
    public float maxMagicAttackW = 1;
    public float maxMindW = 1;
    public float maxMagicDefenseW = 1;
    public float maxDexterityW = 1;
    public float maxSpeedW = 1;
    public float maxPrecisionW = 1;
    public float maxEvasionW = 1;
    public float maxCritProbW = 1;


    public Dictionary<Stats, string> nameOfStats;

    private bool gameStarted = false;

    public bool GameStarted => gameStarted;

    private new void Awake()
    {
        base.Awake();
        InitDicts();
    }

    private void InitDicts()
    {
        InitStatsDict();
        InitMenuOptionsDict();
    }

    private void InitMenuOptionsDict()
    {
        menuOptionDict = new(){
            {MenuOptions.ITEMS, GetArray(ref itemsName, ref items)},
            {MenuOptions.SKILLS, GetArray(ref skillsName, ref skills)},
            {MenuOptions.EQUIP, GetArray(ref equipName, ref equip)},
            {MenuOptions.STATUS, GetArray(ref statusName, ref status)},
            {MenuOptions.FORMATION, GetArray(ref formationName, ref formation)},
            {MenuOptions.SETTINGS, GetArray(ref optionsName, ref options)},
            {MenuOptions.SAVE, GetArray(ref saveName, ref save)},
            {MenuOptions.EXIT, GetArray(ref exitName, ref exit)},
        };
    }

    private void InitStatsDict()
    {
        nameOfStats = new Dictionary<Stats, string>()
        {
            { Stats.HP, hp },
            { Stats.HP_MAX, maxHP },
            { Stats.MP, mp },
            { Stats.MP_MAX, maxMP },
            { Stats.STAMINA, stamina },
            { Stats.STAMINA_MAX, MaxStamina },
            { Stats.ATTACK, attack },
            { Stats.STRENGTH, strength },
            { Stats.DEFENSE, defense },
            { Stats.MAGIC_ATTACK, magicAttack },
            { Stats.MIND, mind },
            { Stats.MAGIC_DEFENSE, magicDefense },
            { Stats.DEX, dexterity },
            { Stats.SPEED, speed },
            { Stats.PRECISION, precision },
            { Stats.EVASION, evasion },
            { Stats.CRIT_PROB, critProb },
        };
    }

    private void Start()
    {
        gameStarted = true;
    }

    private void OnDisable()
    {
        gameStarted = false;
    }

    internal static GameObject[] GetGOByName(string goName)
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        List<GameObject> gameObjectsByName = new();
        for (int i = 0; i < gameObjects.Length; i++)
            if (gameObjects[i].name == goName)
                gameObjectsByName.Add(gameObjects[i]);
        return gameObjectsByName.ToArray();
    }

    internal object[] GetArray(ref string name, ref bool enable)
    {
        return new object[3] { name, enable, enable }; //1 = actual; 2 = old
    }

    internal void SetEnable(MenuOptions option, bool enable)
    {
        menuOptionDict[option][1] = enable;
    }

    internal static bool Compare(float first, LogicalOperations comparator, float second)
    {
        return comparator switch
        {
            LogicalOperations.GREATER => first > second,
            LogicalOperations.LESSER => first < second,
            LogicalOperations.EQUAL => first == second,
            _ => false,
        };
    }

    #region Converter

    private static string GetRelativePath(string path)
    {
        string relativePath = path.Replace("Assets/Resources/", "");
        relativePath = relativePath.Replace(".asset", "");
        return relativePath;
    }

    public static string ToPath(Sprite sprite)
    {
        if (sprite != null)
            return AssetDatabase.GetAssetPath(sprite.texture);
        return null;
    }

    public static Sprite ToSprite(string path)
    {
        if(path == null) return null; 
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    public static string ToPath(AudioClip audio)
    {
        if (audio != null)
            return AssetDatabase.GetAssetPath(audio);
        return null;
    }

    public static AudioClip ToAudio(string resourcePath)
    {
        if (resourcePath == null) return null;

        string relativePath = GetRelativePath(resourcePath);
        return Resources.Load<AudioClip>(relativePath);
    }

    internal static string ToPath(UnityEngine.Object obj)
    {
        if (obj != null)
            return AssetDatabase.GetAssetPath(obj);
        return null;
    }

    internal static UnityEngine.Object To(string path)
    {
        if(path == null) return null;

        string relativePath = GetRelativePath(path);
        return Resources.Load<UnityEngine.Object>(relativePath);
    }

    internal static ScriptableObject ToSO(string path)
    {
        if(path == null) return null;

        string relativePath = GetRelativePath(path);
        ScriptableObject so = Resources.Load<ScriptableObject>(relativePath);
        return so != null ? so : null;
    }

    internal static Dictionary<string, string> ToPath<TKey, TValue>(Dictionary<TKey, TValue> objDict)
    where TKey : UnityEngine.Object
    where TValue : UnityEngine.Object
    {
        if (objDict == null) return null;
        Dictionary<string, string> pathDict = new();
        foreach (KeyValuePair<TKey, TValue> objPair in objDict){
            pathDict.Add(ToPath(objPair.Key), ToPath(objPair.Value));
        }
        return pathDict;
    }

    internal static Dictionary<EquipPartSO, EquipmentSO> ToEquip(Dictionary<string, string> objDictPaths)
    {
        if (objDictPaths == null) return null;
        Dictionary<EquipPartSO, EquipmentSO> pathDict = new();

        foreach (KeyValuePair<string, string> pathPair in objDictPaths)
        {
            if (pathPair.Key != null)
            {
                EquipPartSO equipPart = (EquipPartSO)ToSO(pathPair.Key);
                EquipmentSO equipment = (EquipmentSO)ToSO(pathPair.Value);

                if (equipPart != null)
                {
                    pathDict.Add(equipPart, equipment);
                }
                else
                {
                    Debug.LogError($"Failed to load objects for key: {pathPair.Key} or value: {pathPair.Value}");
                }
            }
        }

        return pathDict;
    }

    internal static Dictionary<TKey, TValue> To<TKey, TValue>(Dictionary<string, string> objDictPaths)
where TKey : UnityEngine.Object
where TValue : UnityEngine.Object
    {
        if (objDictPaths == null) return null;
        Dictionary<TKey, TValue> pathDict = new();
        foreach (KeyValuePair<string, string> pathPair in objDictPaths)
        {
            Debug.Log($"Follow {pathPair.Key == null} {pathPair.Value == null}");
            if (pathPair.Value != null)
                pathDict.Add((TKey)To(pathPair.Key), (TValue)To(pathPair.Value));
            else
                pathDict.Add((TKey)To(pathPair.Key), null);
        }
        return pathDict;
    }

    internal static List<string> ToPath<T>(List<T> objList)
    where T : UnityEngine.Object
    {
        List<string> paths = new();
        foreach (UnityEngine.Object obj in objList)
            paths.Add(ToPath(obj));
        return paths;
    }

    internal static List<T> To<T>(List<string> paths)
    where T : UnityEngine.Object
    {
        List<T> objList = new();
        foreach (string path in paths)
            if(path != null) objList.Add((T)To(path));
            else objList.Add(default);
        return objList;
    }

        internal static List<SkillBase> ToSkillBase(List<string> paths)
    {
        List<SkillBase> skillList = new();
        foreach (string path in paths)
            if(path != null){ 
                SkillBase skillBase = (SkillBase)To(path);
                skillList.Add(skillBase);
            }else skillList.Add(null);
        return skillList;
    }

    #endregion

    internal static void SetText(Button btn, string text)
    {
        if (btn.GetComponent<TextMeshProUGUI>() != null)
            btn.GetComponent<TextMeshProUGUI>().text = text;
        else if (btn.GetComponentInChildren<TextMeshProUGUI>() != null)
            btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    internal static void AddOptions(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    internal static T GetRandom<T>(List<T> list)
    {
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
