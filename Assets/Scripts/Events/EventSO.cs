using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Create new Event")]
public class EventSO : ScriptableObject
{
    #region Variables

    public enum GameEventType { NONE, CONVERSATION, CHOICES, CONDITIONS, INVENTORY, SHOP, TELEPORT, BATTLE, HEAL, MODIFY_CHARACTER, CHANGE_PARTY, ROUTE, CHANGE_EVENT_LIST, CHANGE_MENU_OPTIONS, MOD_EVENT, AUDIO, SAVE, STORE_DATA }
    public enum Conditions { NONE, SWITCH, VARIABLE, HAVE_ITEM, ITEM_EQUIPPED }
    public enum CharacterModifications { NONE, TOTAL, NAME, JOB, STATS, AFFINITIES, EQUIPMENT, ANIMATIONS, SKILLS }
    public enum RouteKeys { NONE, HORIZONTAL, VERTICAL, LOOK, WAIT}
    public enum VariableType { NONE, NUMERIC, SWITCH, STRING }
    public enum AudioOptions { NONE, PLAY, CHANGE_VOLUME, PAUSE, SAVE, REPLAY}

    public GameEventType eventType;

    #region Common
    public ItemSO item;
    public float amount;
    #endregion

    #region Conversation
    public List<MonologueSO> conversation;

    #endregion

    #region Choices
    public MonologueSO choicePretext;
    public List<string> choicesText;
    public List<ConsequenceSO> choicesConsequences;
    #endregion

    #region Conditions
    public Conditions condition;
    public ConsequenceSO positiveConsequence;
    public ConsequenceSO negativeConsequence;
    #region Switch || Variables || Store Data
    public int index;
    #region Switch
    public bool switchOn = false;
    #endregion
    #region Variables
    public LogicalOperations logicalOperation;
    public string value;
    #endregion
    #endregion
    #region Item equipped
    public bool equipped;
    #endregion
    #endregion
    #region Inventory
    #endregion
    #region Shop
    public List<ItemSO> itemsInShop;
    public bool canBuy = true;
    public bool canSell = true;
    #endregion
    #region Teleport
    public bool teleportPlayer;
    public bool teleportThis;
    public string objectToTeleportName;
    public string destinationScene;
    public Vector2 targetPosition;
    public DungeonSO destinationDungeon;
    #endregion
    #region Battle
    public List<PersonajeHandlerSO> enemyGroup;
    public bool canEscape = false;
    public bool canBeDefeated = true;
    #endregion
    #region Heal || Modify Character || Change Party
    public PersonajeHandlerSO character;
    public int allyCharacterIndex;
    #region Heal
    public bool healAll = true;
    [Range(0, 100)] public float lifeHealPercent;
    [Range(0, 100)] public float manaHealPercent;
    [Range(0, 100)] public float staminaHealPercent;
    public bool statusHeal = true;
    #endregion
    #region Modify Character
    public CharacterModifications aspectToModify;
    #region Total
    public PersonajeHandlerSO newCharacter;
    #endregion
    #region Name
    public string newName;
    #endregion
    #region Job
    public JobSO newJob;
    #endregion
    #region Stats || Affinities
    public ModScale modScale;
    #region Stats
    public Stats statToModify;
    #endregion
    #region Affinities
    public Affinity affinityToModify;
    #endregion
    #endregion
    #region Equipment
    public EquipmentSO equipItem;
    public bool equip;
    #endregion
    #region Animations
    public RuntimeAnimatorController newAnimations;
    public Sprite newSpriteInMenu;
    #endregion
    #region Skills
    public List<SkillBase> skills;
    #endregion
    #endregion
    #region Change Party
    public AddRemove changeOption;
    #region Add
    public bool resetStats = false;
    #endregion
    #endregion
    #endregion
    #region Route
    public List<RouteKeys> directions;
    [Tooltip("- For horizontal and vertical the values will determine the distance at which they will move (negative values for left and down) \n" + 
    "- The values in look determine the direction in which the event will look clockwise (from 0 to 3). \n" + 
    "- The values for wait will make the event wait for that number of seconds. ")]
    public List<float> values;
    #endregion
    #region Change Event List
    public List<EventSO> newEventList;
    #endregion
    #region Change Menu Options
    public MenuOptions menuOption;
    public bool enable;
    #endregion
    #region Mod Event
    public EventInteraction.MoveRoute eventRoute;
    public EventInteraction.TriggerType eventTrigger;
    public GameObjectComponent component;
    #region Component/Sprite Renderer
    public Sprite newSprite;
    #endregion
    public bool enableComponent;
    #endregion
    #region Audio
    public AudioOptions option;
    public AudioClip audio;
    [Range(0, 1)] public float volume = 0.5f;
    public AudioType audioType;
    #endregion
    #region Save
    #endregion
    #region Store Data
    public VariableType variableType;
    public AddRemove dataOperation;
    #endregion

    #region Methods

        internal static EventSO NewConversation(List<MonologueSO> conversation)
    {
        EventSO newConversation = CreateInstance<EventSO>();
        newConversation.eventType = GameEventType.CONVERSATION;
        newConversation.conversation = conversation;
        return newConversation;
    }

    internal static EventSO NewInventory(ItemSO item, int amount)
    {
        EventSO newInventory = CreateInstance<EventSO>();
        newInventory.eventType = GameEventType.INVENTORY;
        newInventory.item = item;
        newInventory.amount = amount;
        return newInventory;
    }

    internal static EventSO NewChangeEventList(List<EventSO> newEventList)
    {
        EventSO newChangeEventList = CreateInstance<EventSO>();
        newChangeEventList.eventType = GameEventType.CHANGE_EVENT_LIST;
        newChangeEventList.newEventList = newEventList;
        return newChangeEventList;
    }

    internal static EventSO NewRoute(KeyValuePair<RouteKeys, float>[] route)
    {
        EventSO newRoute = CreateInstance<EventSO>();
        newRoute.eventType = GameEventType.ROUTE;
        newRoute.directions = new();
        for(int i = 0; i < route.Length; i++)
        newRoute.directions.Add(route[i].Key);

                newRoute.values = new();
        for(int i = 0; i < route.Length; i++)
        newRoute.values.Add(route[i].Value);
        
        return newRoute;
    }

    internal static EventSO NewAudioPlay(AudioClip audio, AudioType type, int volume)
    {
        EventSO newAudioPlay = CreateInstance<EventSO>();
        newAudioPlay.eventType = GameEventType.AUDIO;
        newAudioPlay.option = AudioOptions.PLAY;
        
        newAudioPlay.audio = audio;
        newAudioPlay.audioType = type;
        newAudioPlay.volume = volume;
        return newAudioPlay;
    }
    #endregion

    #endregion
}
