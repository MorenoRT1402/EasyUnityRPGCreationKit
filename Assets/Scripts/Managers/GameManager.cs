using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameObjectComponent { NONE, SPRITE_RENDERER, RIGIDBODY2D, COLLIDER2D, IMAGE, BUTTON }

public class GameManager : Singleton<GameManager>
{
    public string GameTitle = "Easy Unity RPG Creation Kit";
    public static bool InMainScreen => MainScreenManager.Instance != null;
    public static bool InBattle => BattleStateMachine.Instance != null;

    public static Dictionary<GameObjectComponent, Type> gameComponentsDict;

    public static float secsPlaying;
    public static bool InWorld => MenuManager.Instance != null && ChatManager.Instance != null && ShopManager.Instance != null;

    private void Start()
    {
        InitDict();
    }

    private void Update()
    {
        if(!InMainScreen) secsPlaying += Time.deltaTime;
    }

    private static void InitDict()
    {
        gameComponentsDict = new(){
            { GameObjectComponent.BUTTON, typeof(Button)},
            { GameObjectComponent.COLLIDER2D, typeof(Collider2D)},
            { GameObjectComponent.IMAGE, typeof(Image)},
            { GameObjectComponent.RIGIDBODY2D, typeof(Rigidbody2D)},
            { GameObjectComponent.SPRITE_RENDERER, typeof(SpriteRenderer)},
        };
    }
    public static void NewGame()
    {
        SceneHandler.DestroyAllEvents();
        AudioManager.Instance.Initialize();
        PartyManager.Instance.InitParty();
        SceneHandler.Instance.NewGame();
        DungeonManager.Instance.SetupPlayer();
    }
    internal static void LoadGame(SaveData loadedData)
    {
        SceneHandler.DestroyAllEvents();
        loadedData.Load();
        AudioManager.Instance.PlayActualDungeonMusic();
    }

    #region Essential

    public static bool InPriorityUI()
    {
        if (InBattle) return true;
        if (!InWorld) return false;
        if (MenuManager.Instance.MenuOpened || ChatManager.Instance.Chatting || ShopManager.Instance.ShopOpened)
            return true;
        return false;
    }

    internal static void Wait(float waitTime)
    {
        float timer = 0;
        while (timer < waitTime)
            timer += Time.deltaTime;
    }

    #endregion

    #region Components

    internal static Type Get(GameObjectComponent component)
    {
        return gameComponentsDict[component];
    }

    public static T GetComponent<T>(GameObject gameObject, GameObjectComponent component)
    {
        if (gameComponentsDict.TryGetValue(component, out Type componentType))
        {
            Component componentGetted = gameObject.GetComponent(componentType);
            if (componentGetted is T)
            {
                return (T)(object)componentGetted;
            }
            else
            {
                Debug.LogError("Component type does not match the expected type.");
                return default;
            }
        }
        else
        {
            Debug.LogError("Component type not found in gameComponentsDict.");
            return default;
        }
    }

    #endregion


    #region Events

    internal static void StartEventInteraction(EventInteraction eventInteraction)
    {
        //    Debug.Log($"{eventInteraction.eventsList.Count > 0} && {!InPriorityUI()} ? {eventInteraction.eventsList.Count > 0 && !InPriorityUI()}");
        if (eventInteraction.ActionList.Count > 0 && !InPriorityUI())
            EventManager.Instance.Interact(eventInteraction);
    }

    internal static void StartBattle(List<GameObject> allies, EnemyGroupSO group, DungeonSO actualDungeon, bool canEscape, bool canBeDefeated)
    {
        SceneHandler.SceneChanged = false;
        AudioManager.Instance.Battle(true);
        DungeonManager.Instance.InitializeBattle(allies, group, actualDungeon, true, true);
    }

    internal static void EndBattle()
    {
        SceneHandler.Instance.ReturnToWorld();
        AudioManager.Instance.Battle(false);
        SceneHandler.SceneChanged = false;
    }

    internal static void AddChoiceConsequences(List<EventSO> eventList)
    {
        ChatManager.Instance.SetMainPanels(false);
        ChatManager.Choosing = false;
        EventManager.Instance.AddSubEventList(eventList);
    }
    #endregion
}
