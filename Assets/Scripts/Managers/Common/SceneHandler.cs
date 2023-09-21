using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneHandler : Singleton<SceneHandler>
{
    [Header("Basic")]
    public string mainScreenScene = "MainScreenScene";
    public string startScene;
    public Vector3 startPosition;
    public string battleScene;

    [Header("Transitions")]
    public AnimatorController sceneChangeTransition;
    public AnimatorController battleTransition;
    public float transitionTime = 1f;

    private static Vector3 actualPosition;
    private string previousScene;
    private bool hasReturnedOfBattle = false;
    private bool battleTransitionMark = false;

    public static Vector3 ActualPosition => actualPosition;

    public static bool SceneChanged;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        ChangeToScene(mainScreenScene, false);
    }

    public void NewGame()
    {
        actualPosition = startPosition;
        ChangeToScene(startScene, false);
        //        actualPosition = PartyManager.Instance.GetLeader().transform.position;
    }
    public void ChangeToScene(string scene, bool saveScene)
    {
        if (InThisScene(scene)) return;


        if (saveScene)
        {
            actualPosition = PartyManager.Instance.PartyGameObjects[0].transform.position;
            previousScene = SceneManager.GetActiveScene().name;
        }
        SaveManager.SaveEvents(false);

        StartCoroutine(SceneTransition(scene));

        StartCoroutine(OnSceneCharged());

    }

    private IEnumerator SceneTransition(string scene)
    {
        AnimatorController controller = battleTransitionMark ? battleTransition : sceneChangeTransition;
        Transitions transitions = FindObjectOfType<Transitions>();
        if (transitions != null)
        {
            transitions.StartTransition(controller);
            yield return new WaitForSecondsRealtime(transitionTime);
        }
        battleTransitionMark = false;
        SceneManager.LoadScene(scene);
        yield break;
    }

    private IEnumerator OnSceneCharged()
    {
        yield return new WaitForSeconds(0.05f);
        SceneChanged = true;
        //        Debug.Log("Waited");
        if (!GameManager.InBattle)
        {
            SaveManager.LoadEvents();

            if (hasReturnedOfBattle) EventManager.Instance.ComunicateEnd(EventSO.GameEventType.BATTLE);
        }
    }

    internal void ReturnToPreviousScene(bool saveScene)
    {
        ChangeToScene(previousScene, saveScene);

        DungeonManager.Instance.SetupPlayer();
    }

    internal void ChangeToBattleScene()
    {
        hasReturnedOfBattle = false;
        battleTransitionMark = true;
        PartyManager.Instance.DeactivateAllMovement(false);
        AudioManager.Instance.PlayEncounterSound();
        ChangeToScene(battleScene, true);
    }

    internal void ReturnToWorld()
    {
        hasReturnedOfBattle = true;
        battleTransitionMark = true;
        ReturnToPreviousScene(false);
        DungeonManager.inBattle = false;
    }

    internal void Move(GameObject objectToTeleport, string destinationScene, DungeonSO destinationDungeon, Vector3 targetPosition)
    {
        if (!InThisScene(destinationScene))
            SceneManager.MoveGameObjectToScene(objectToTeleport, SceneManager.GetSceneByName(destinationScene));
        objectToTeleport.transform.position = targetPosition;

        DungeonManager.Instance.SetDungeon(destinationDungeon);
    }

    private bool InThisScene(string destinationScene)
    {
        //        DebugManager.CompareEquals(destinationScene, SceneManager.GetActiveScene().name);
        return destinationScene.Equals(SceneManager.GetActiveScene().name);
    }

    internal void Move(string objectToTeleportName, string destinationScene, DungeonSO destinationDungeon, Vector2 targetPosition)
    {
        GameObject[] gameObjectsWithThisName = GeneralManager.GetGOByName(objectToTeleportName);
        if (gameObjectsWithThisName.Length <= 0) return;

        GameObject firstGO = gameObjectsWithThisName[0];
        Move(firstGO, destinationScene, destinationDungeon, targetPosition);
    }

    internal static void DestroyAllEvents()
    {
        CharacterMovement[] characterMovements = FindObjectsOfType<CharacterMovement>();
        foreach (CharacterMovement cM in characterMovements)
            Destroy(cM.gameObject);
    }

    internal static string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
}

