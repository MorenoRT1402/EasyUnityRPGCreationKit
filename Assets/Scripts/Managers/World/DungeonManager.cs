using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonManager : Singleton<DungeonManager>
{
    public DungeonSO actualDungeon;

    [Header("Player Movement")]
    public float moveSpeed;
    public bool diagonalMove = false;
    public List<LayerMask> solidObjectsLayer;
    public float solidCollisionRadio = 0.4f;
    public List<LayerMask> interactuableObjectsLayer;
    public float interactuableCollisionRadio = 0.1f;

    private static float stepsCount, numberOfStepsToCheck;

    public static bool sceneChanged;
    public static bool inBattle;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        numberOfStepsToCheck = actualDungeon.Steps;
    }

    public void SetupPlayer()
    {
        if (actualDungeon != null && PartyManager.Instance.PartyGameObjects.Count > 0)
            PartyManager.Instance.SetupPlayer();
    }

    public void CheckForEncounters(PlayerMovement playerMovement)
    {
        if (actualDungeon == null)
            return;

        List<AreaSO> areas = actualDungeon.Areas;
        if (areas != null && areas.Count > 0)
            foreach (AreaSO area in areas)
                CheckForEncounters(area, playerMovement);

    }

    private void CheckForEncounters(AreaSO area, PlayerMovement playerMovement)
    {
        if (area.Enemys == null || area.Enemys.Count <= 0)
            return;
        GameObject playerGO = playerMovement.gameObject;
        Vector3 position = playerGO.transform.position;
        float radio = solidCollisionRadio;
        LayerMask layer = area.Layer;

        if (Physics2D.OverlapCircle(position, radio, layer) != null)
        {
            if (stepsCount >= numberOfStepsToCheck)
            {
                float randomNumber = Random.Range(0, 100);
                foreach (EnemyGroupSO group in area.Enemys)
                    if (randomNumber <= group.AppearenceProb)
                    {
                        List<GameObject> allies = PartyManager.Instance.PartyGameObjects;
                        GameManager.StartBattle(allies, group, actualDungeon, true, true);
                        //                        BattleStateMachine.Instance.StartBattle(allies, group, actualDungeon);
                        //                        Debug.Log("Battle vs " + group);
                        stepsCount = 0;
                    }
            }
            else stepsCount++;
        }
    }


    internal void InitializeBattle(List<GameObject> allies, List<PersonajeHandlerSO> enemyGroup, bool canEscape, bool canBeDefeated)
    {
        EnemyGroupSO enemyGroupSO = EnemyGroupSO.CreateInstance(enemyGroup);

        GameManager.StartBattle(allies, enemyGroupSO, actualDungeon, canEscape, canBeDefeated);
        //InitializeBattle(allies, enemyGroupSO, actualDungeon, canEscape, canBeDefeated);
    }

    public void InitializeBattle(List<GameObject> allies, EnemyGroupSO enemyGroup, DungeonSO dungeon, bool canEscape, bool canBeDefeated)
    {
        StartCoroutine(InitializeBattleCoroutine(allies, enemyGroup, dungeon, canEscape, canBeDefeated));
    }

    private IEnumerator InitializeBattleCoroutine(List<GameObject> allies, EnemyGroupSO enemyGroup, DungeonSO dungeon, bool canEscape, bool canBeDefeated) //Its coroutine due to SceneHandler need time to charge
    {
        //        Debug.Log($"InitBattleCoroutine" + sceneChanged);
        if (!SceneHandler.SceneChanged)
        {
            SceneHandler.Instance.ChangeToBattleScene();
            //            yield return null;
            yield return new WaitForSeconds(0.15f);

            while (SceneHandler.SceneChanged)
            {
                yield return new WaitForSeconds(0.015f);
                if (BattleStateMachine.Instance != null)
                {
                    BattleStateMachine.Instance.StartBattle(allies, enemyGroup, dungeon, canEscape, canBeDefeated);
                    SceneHandler.SceneChanged = false;
                    yield break;
                }
            }
        }
    }

    private void InitializeBattlewithoutCoRou(List<GameObject> allies, EnemyGroupSO enemyGroup, DungeonSO dungeon, bool canEscape, bool canBeDefeated)
    {// not works
        SceneHandler.Instance.ChangeToBattleScene();

        float timer = 0;


        while (true)
        {

            //        Debug.Log($"InitBattleCoroutine" + sceneChanged);
            Debug.Log($"{SceneHandler.Instance} {BattleStateMachine.Instance == null}");
            Debug.Log(BattleStateMachine.Instance);

            if (BattleStateMachine.Instance != null)
            {
                Debug.Log($"Enter");
                BattleStateMachine.Instance.StartBattle(allies, enemyGroup, dungeon, canEscape, canBeDefeated);

                SceneHandler.SceneChanged = true;

            }
            timer += Time.deltaTime;
            if (timer > 10)
                break;
        }
        SceneHandler.SceneChanged = false;

    }

    internal void SetDungeon(DungeonSO destinationDungeon)
    {
        actualDungeon = destinationDungeon;
        AudioManager.Instance.PauseAll();
        if(actualDungeon.Music != null) AudioManager.Instance.Play(actualDungeon.Music, AudioType.MUSIC, -1f);
        if(actualDungeon.Ambiental != null) AudioManager.Instance.Play(actualDungeon.Ambiental, AudioType.AMBIENTAL, -1f);
    }
}
