using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class EventManager : Singleton<EventManager>
{
    [HideInInspector] public static EventInteraction actualEventInteraction;
    [HideInInspector] public static string actualEventID;
    //    [HideInInspector] public static int actualEventIndex;

    [HideInInspector] public static EventInteraction nearestEventInteraction;

    [HideInInspector] public static List<ConsequenceSO> subEventLists;
    [HideInInspector] public static List<int> subEventIndexes;


    private static readonly float cooldown = 0.4f;
    private static float cooldownTimer = 0;

    private static bool canContinue = true;

    private static ChatManager CM;
    private static PartyManager PM;

    private void OnEnable()
    {
        actualEventInteraction = GetEventInteraction(actualEventID);
        PM = PartyManager.Instance;
    }

    private void Update()
    {

        RunCooldown();

    }

    private void RunCooldown()
    {
        if (cooldownTimer < cooldown && SecurityDistance())
            cooldownTimer += Time.deltaTime;
    }

    private bool SecurityDistance()
    {
        if (nearestEventInteraction != null && nearestEventInteraction.triggerType == EventInteraction.TriggerType.TOUCH)
            return false;

        if (actualEventInteraction != null) return false;

        return true;
    }

    private static EventInteraction GetEventInteraction(string actualEventID)
    {
        if (actualEventInteraction != null) return actualEventInteraction; //There is event interaction, all right
        if (actualEventID == null) return null; //It's free

        List<EventInteraction> eventInteractions = new();
        eventInteractions.AddRange(FindObjectsOfType<EventInteraction>(false));

        for (int i = 0; i < eventInteractions.Count; i++)
            if (eventInteractions[i] != null && eventInteractions[i].id == actualEventID)
                return eventInteractions[i];
        return null;
    }

    internal void Interact(EventInteraction eventInteraction)
    {
//                Debug.Log($"{actualEventID != null} || {cooldownTimer < cooldown} ? {actualEventID != null || cooldownTimer < cooldown}");
        if (actualEventID != null || cooldownTimer < cooldown) return;

        actualEventInteraction = eventInteraction;
        actualEventID = actualEventInteraction.id;
        //        actualEventIndex = index;

        subEventLists = new();
        subEventIndexes = new();

        AddSubEventList(actualEventInteraction.ActionList);

    }

    private void Execute()
    {
        CM = ChatManager.Instance;
        EventSO actualEvent = GetActualEvent();

        switch (actualEvent.eventType)
        {
            case EventSO.GameEventType.CONVERSATION:
                ConversationExecute(actualEvent.conversation, true);
                break;
            case EventSO.GameEventType.CHOICES:
                ChoicesExecute(actualEvent);
                break;
            case EventSO.GameEventType.CONDITIONS:
                ConditionsExecute(actualEvent);
                break;
            case EventSO.GameEventType.INVENTORY:
                InventoryExecute(actualEvent);
                break;
            case EventSO.GameEventType.SHOP:
                ShopExecute(actualEvent);
                break;
            case EventSO.GameEventType.TELEPORT:
                TeleportExecute(actualEvent);
                break;
            case EventSO.GameEventType.BATTLE:
                BattleExecute(actualEvent);
                break;
            case EventSO.GameEventType.HEAL:
                HealExecute(actualEvent);
                break;
            case EventSO.GameEventType.MODIFY_CHARACTER:
                ModifyCharacterExecute(actualEvent);
                break;
            case EventSO.GameEventType.CHANGE_PARTY:
                ChangePartyExecute(actualEvent);
                break;
            case EventSO.GameEventType.ROUTE:
                StartCoroutine(RouteExecute(actualEvent));
                break;
            case EventSO.GameEventType.CHANGE_EVENT_LIST:
                ChangeEventList(actualEvent);
                break;
            case EventSO.GameEventType.CHANGE_MENU_OPTIONS:
                MenuManager.Instance.SetEnable(actualEvent.menuOption, actualEvent.enable);
                NextEvent();
                break;
            case EventSO.GameEventType.MOD_EVENT:
                ModEventExecute(actualEvent);
                break;
            case EventSO.GameEventType.AUDIO:
                AudioExecute(actualEvent);
                break;
            case EventSO.GameEventType.STORE_DATA:
                StoreDataExecute(actualEvent);
                break;
        }
    }

    private void ConditionsExecute(EventSO actualEvent)
    {
        subEventIndexes[^1]++;
        if (CheckCondition(actualEvent.condition, actualEvent))
            AddSubEventList(actualEvent.positiveConsequence.ConsequenceEvents);
        else
            AddSubEventList(actualEvent.negativeConsequence.ConsequenceEvents);
    }

    private bool CheckCondition(EventSO.Conditions condition, EventSO actualEvent)
    {
        return condition switch
        {
            EventSO.Conditions.SWITCH => SaveManager.switches[actualEvent.index] == actualEvent.switchOn,
            EventSO.Conditions.VARIABLE => CheckNumericCondition(actualEvent),
            EventSO.Conditions.HAVE_ITEM => CheckHaveItemCondition(actualEvent),
            EventSO.Conditions.ITEM_EQUIPPED => CheckItemEquippedConditions(actualEvent),
            _ => true,
        };
    }

    private bool CheckItemEquippedConditions(EventSO actualEvent)
    {
        PersonajeHandler character = GetCharacter(actualEvent);
        if (actualEvent.item is EquipmentSO equipment)
            return character.GetEquipped(equipment) > 0;
        return false;
    }

    private bool CheckHaveItemCondition(EventSO actualEvent)
    {
        float savedValue = InventoryManager.Instance.GetAmount(actualEvent.item);
        LogicalOperations comparator = actualEvent.logicalOperation;
        float valueToCompare = actualEvent.amount;

        return GeneralManager.Compare(savedValue, comparator, valueToCompare);
    }

    private bool CheckNumericCondition(EventSO actualEvent)
    {
        float savedValue = SaveManager.numericVariables[actualEvent.index];
        LogicalOperations comparator = actualEvent.logicalOperation;
        PersonajeHandler target = GetCharacter(actualEvent);
        float valueToCompare = NCalcManager.Instance.CalculateObjectToFloat(target.gameObject, actualEvent.value);

        return GeneralManager.Compare(savedValue, comparator, valueToCompare);
    }

    private void AudioExecute(EventSO actualEvent)
    {
        switch (actualEvent.option)
        {
            case EventSO.AudioOptions.PLAY:
                AudioManager.Instance.Play(actualEvent.audio, actualEvent.audioType, actualEvent.volume);
                break;
            case EventSO.AudioOptions.CHANGE_VOLUME:
                AudioManager.Instance.ChangeVolume(actualEvent.audioType, actualEvent.volume);
                break;
            case EventSO.AudioOptions.PAUSE:
                AudioManager.Instance.Pause(actualEvent.audioType);
                break;
            case EventSO.AudioOptions.SAVE:
                AudioManager.Instance.Save(actualEvent.audioType);
                break;
            case EventSO.AudioOptions.REPLAY:
                AudioManager.Instance.Replay(actualEvent.audioType);
                break;
        }
        NextEvent();
    }

    private void StoreDataExecute(EventSO actualEvent)
    {
        PersonajeHandler target = GetCharacter(actualEvent);
        object value = new();
        switch (actualEvent.variableType)
        {
            case EventSO.VariableType.NUMERIC:
                value = NCalcManager.Instance.CalculateObjectToFloat(target.gameObject, actualEvent.value);
                break;
            case EventSO.VariableType.SWITCH:
                value = actualEvent.switchOn;
                break;
            case EventSO.VariableType.STRING:
                value = actualEvent.value;
                break;
        }
        SaveManager.ManageVariables(actualEvent.index, actualEvent.dataOperation, value);
        NextEvent();
    }

    private void ModEventExecute(EventSO actualEvent)
    {
        if (actualEvent.eventRoute != EventInteraction.MoveRoute.ACTUAL)
            actualEventInteraction.moveRoute = actualEvent.eventRoute;
        if (actualEvent.eventTrigger != EventInteraction.TriggerType.ACTUAL)
            actualEventInteraction.triggerType = actualEvent.eventTrigger;

        ObtainAndEnableComponent(actualEvent.component, actualEventInteraction.gameObject, actualEvent.enableComponent);

        if (actualEvent.component == GameObjectComponent.SPRITE_RENDERER)
            actualEventInteraction.GetComponent<SpriteRenderer>().sprite = actualEvent.newSprite;

        NextEvent();
    }

    private void ObtainAndEnableComponent(GameObjectComponent component, GameObject gameObject, bool enableComponent)
    {
        Type componentType = GameManager.Get(component);
        Component componentToMod = gameObject.GetComponent(componentType);

        SimpleEnableComponent(componentToMod, enableComponent);
    }

    private void SimpleEnableComponent(Component componentToMod, bool enableComponent)
    {
        Type type = componentToMod.GetType();
        if (type == typeof(Button))
            ((Button)componentToMod).enabled = enableComponent;
        if (type == typeof(Collider2D))
            ((Collider2D)componentToMod).enabled = enableComponent;
        if (type == typeof(Image))
            ((Image)componentToMod).enabled = enableComponent;
        if (type == typeof(Rigidbody))
            ((Rigidbody2D)componentToMod).simulated = enableComponent;
        if (type == typeof(SpriteRenderer))
            ((SpriteRenderer)componentToMod).enabled = enableComponent;
    }

    private IEnumerator RouteExecute(EventSO actualEvent)
    {
        for (int i = 0; i < actualEvent.directions.Count; i++)
        {
            canContinue = false;
            StartCoroutine(ExecuteEventRoute(actualEvent, i));
            yield return new WaitUntil(() => canContinue == true);
        }
        NextEvent();

    }

    private IEnumerator ExecuteEventRoute(EventSO actualEvent, int i)
    {
        if (actualEventInteraction == null) yield break;
        EventSO.RouteKeys key = actualEvent.directions[i];
        float value = actualEvent.values[i];
        switch (key)
        {
            case EventSO.RouteKeys.HORIZONTAL:
            case EventSO.RouteKeys.VERTICAL:
                StartCoroutine(MoveEventExecute(key, value));
                break;
            case EventSO.RouteKeys.LOOK:
                PersonajeAnimations.Instance.ChangeAnimation(key, value, actualEventInteraction.gameObject);
                new WaitForSeconds(PersonajeAnimations.Instance.GetCurrentAnimationDuration(actualEventInteraction.gameObject));
                canContinue = true;
                break;
            case EventSO.RouteKeys.WAIT:
                yield return new WaitForSecondsRealtime(value);
                canContinue = true;
                break;
        }
    }

    private IEnumerator MoveEventExecute(EventSO.RouteKeys key, float value)
    {
        canContinue = false;
        Vector3 displacement = Vector3.zero;
        displacement.x = key == EventSO.RouteKeys.HORIZONTAL ? value : 0;
        displacement.y = key == EventSO.RouteKeys.VERTICAL ? value : 0;
        actualEventInteraction.Move(displacement);
        yield return new WaitUntil(() => actualEventInteraction.IsMoving == false);
        canContinue = true;
        yield break;
    }

    private void InventoryExecute(EventSO actualEvent)
    {
        InventoryManager IM = InventoryManager.Instance;
        int itemAmount = (int)actualEvent.amount;
        if (actualEvent.amount >= 0)
            IM.Add(actualEvent.item, itemAmount);
        else
            IM.Remove(actualEvent.item, Mathf.Abs(itemAmount));
        NextEvent();
    }

    private void ModifyCharacterExecute(EventSO actualEvent)
    {
        PersonajeHandler character = GetAlly(actualEvent);

        switch (actualEvent.aspectToModify)
        {
            case EventSO.CharacterModifications.TOTAL:
                character.Init(actualEvent.newCharacter);
                break;
            case EventSO.CharacterModifications.NAME:
                string newName = NCalcManager.LocateVariables(actualEvent.newName);
                character.Stats.Name = newName;
                break;
            case EventSO.CharacterModifications.JOB:
                character.SetJob(actualEvent.newJob, actualEvent.resetStats);
                break;
            case EventSO.CharacterModifications.STATS:
                character.ModStats(actualEvent.statToModify, actualEvent.modScale, actualEvent.amount); // In the future I could change amount (fixed float) for value (string formula with NCalc) 
                break;
            case EventSO.CharacterModifications.AFFINITIES:
                character.ModAffinities(actualEvent.affinityToModify, actualEvent.modScale, actualEvent.amount); // ''
                break;
            case EventSO.CharacterModifications.EQUIPMENT:
                character.ChangeEquipment(actualEvent.equipItem);
                break;
            case EventSO.CharacterModifications.ANIMATIONS:
                character.SetMenuSprite(actualEvent.newSpriteInMenu);
                character.ChangeAnimations(actualEvent.newAnimations);
                break;
            case EventSO.CharacterModifications.SKILLS:
                character.ChangeSkills(actualEvent.changeOption, actualEvent.skills);
                break;
        }

        NextEvent();
    }
    private static PersonajeHandler GetCharacter(EventSO actualEvent)
    {
        PersonajeHandlerSO characterModel = actualEvent.character;

        if (characterModel == null)
            return PM.GetAlly(actualEvent.allyCharacterIndex);

        return PM.GetCharacter(characterModel);
    }


    private static PersonajeHandler GetAlly(EventSO actualEvent)
    {
        PersonajeHandlerSO characterModel = actualEvent.character;

        if (characterModel == null)
            return PM.GetAlly(actualEvent.allyCharacterIndex);

        return PM.GetAlly(characterModel);
    }

    private void ChangePartyExecute(EventSO actualEvent)
    {
        PersonajeHandlerSO characterModel = actualEvent.character != null ? actualEvent.character : PM.GetAlly(actualEvent.allyCharacterIndex).SO;
        GameObject newCharacterGO = PM.CreateCharacterGO(characterModel);

        switch (actualEvent.changeOption)
        {
            case AddRemove.ADD:
                PM.AddToParty(newCharacterGO, actualEvent.resetStats);
                break;
            case AddRemove.REMOVE:
                PM.Remove(characterModel, true);
                break;
        }

        NextEvent();
    }

    private void HealExecute(EventSO actualEvent)
    {
        //Get Characters To Heal
        List<PersonajeHandler> charactersToHeal = new();
        if (actualEvent.healAll)
            charactersToHeal.AddRange(PM.GetAllAllies());
        else if (actualEvent.character != null)
            charactersToHeal.Add(PM.GetAlly(actualEvent.character));
        else
            charactersToHeal.Add(PM.GetAlly(actualEvent.allyCharacterIndex));

        //Heal
        if (charactersToHeal.Count > 0)
            for (int i = 0; i < charactersToHeal.Count; i++)
                if (charactersToHeal[i] != null)
                {
                    float life = actualEvent.lifeHealPercent / 100;
                    float mana = actualEvent.manaHealPercent / 100;
                    float stamina = actualEvent.staminaHealPercent / 100;
                    bool status = actualEvent.statusHeal;
                    charactersToHeal[i].Heal(life, mana, stamina, status);
                }

        NextEvent();
    }

    private void ConversationExecute(List<MonologueSO> conversation, bool nextEvent)
    {
        if (CM == null) return;

        if (conversation.Count > 0)
            CM.Run(conversation);
        else if (nextEvent)
            NextEvent();
    }

    private void ChoicesExecute(EventSO actualEvent)
    {

        MonologueSO pretext = actualEvent.choicePretext;

        if (pretext != null)
            ConversationExecute(new List<MonologueSO>() { pretext }, false);

        CM.ShowChoices(actualEvent.choicesText, actualEvent.choicesConsequences);

        subEventIndexes[subEventIndexes.Count - 1]++;
    }

    private static void ShopExecute(EventSO actualEvent)
    {
        ShopManager SM = ShopManager.Instance;

        SM.SetBuyList(actualEvent.itemsInShop);
        SM.ToogleShop(true);
        ShopManager.Instance.buyBtn.SetActive(actualEvent.canBuy);
        ShopManager.Instance.sellBtn.SetActive(actualEvent.canSell);
    }

    private void TeleportExecute(EventSO actualEvent)
    {
        SceneHandler SH = SceneHandler.Instance;

        if (actualEvent.teleportPlayer)
        {
            if (!actualEvent.destinationScene.Equals(SceneManager.GetActiveScene().ToString())) SH.ChangeToScene(actualEvent.destinationScene, true);
            PM.GetLeader().transform.position = actualEvent.targetPosition;
        }
        if (actualEvent.teleportThis)
            SH.Move(actualEventInteraction.gameObject, actualEvent.destinationScene,  actualEvent.destinationDungeon, actualEvent.targetPosition);

        else SH.Move(actualEvent.objectToTeleportName, actualEvent.destinationScene, actualEvent.destinationDungeon, actualEvent.targetPosition);

        NextEvent();
    }

    private static void BattleExecute(EventSO actualEvent)
    {
        List<GameObject> allies = PM.PartyGameObjects;
        DungeonManager.Instance.InitializeBattle(allies, actualEvent.enemyGroup, actualEvent.canEscape, actualEvent.canBeDefeated);
    }

    internal void ComunicateEnd(EventSO.GameEventType gameEventType)
    {
        actualEventInteraction = GetEventInteraction(actualEventID);
        if (actualEventInteraction != null)
            if (GetActualEvent().eventType == gameEventType)
                NextEvent();
    }

    private void ChangeEventList(EventSO actualEvent)
    {
        EventInteraction eI = actualEventInteraction;
        if (eI.changeEventListMark || subEventIndexes.First() >= eI.ActionList.Count - 1)
        {
            eI.SetNewEventList(actualEvent.newEventList);
            eI.changeEventListMark = false;
            EndEvent();
        }
        else
        {
            eI.changeEventListMark = true;
            eI.ActionList.Add(actualEvent);
            NextEvent();
        }
    }
    public static EventSO GetActualEvent()
    {
        ConsequenceSO actualEventList = subEventLists.Last();
        int actualIndex = subEventIndexes.Last();
        EventSO actualEvent = actualEventList.GetEvent(actualIndex);

        if (DebugManager.Instance.keyDebugs) Debug.Log($"Executing event from {actualEventInteraction.id} number {actualIndex} of {subEventLists.Count - 1} : {actualEvent.eventType}");


        return actualEvent;
    }


    internal void AddSubEventList(List<EventSO> eventList)
    {
        ConsequenceSO consequenceSO = ScriptableObject.CreateInstance<ConsequenceSO>();
        consequenceSO.SetData(eventList);
        subEventLists.Add(consequenceSO);

        subEventIndexes.Add(0);

        Execute();
    }

    public void NextEvent()
    {
        subEventIndexes[^1]++;

        if (subEventLists.Last().GetEvent(subEventIndexes[^1]) != null)
            Execute();
        else
            EndEvent();

    }

    private void EndEvent()
    {
        ConsequenceSO actualEventList = subEventLists.Last();
        subEventLists.Remove(actualEventList);
        subEventIndexes.RemoveAt(subEventIndexes.Count - 1);

        if (subEventLists.Count > 0)
            Execute();

        else
        {
            actualEventInteraction = null;
            actualEventID = null;
            //        actualEventIndex = 0;
            cooldownTimer = 0;
        }
    }
}
