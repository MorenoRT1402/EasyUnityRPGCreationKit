using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : Singleton<PartyManager>
{
    public int activePartyCount = 4;
    public List<PersonajeHandlerSO> startParty;
    public GameObject heroPrefab;

    private List<GameObject> partyGameObjects;
    private List<GameObject> benchGameObjects;
    private List<PersonajeHandler> outMembers;

    public List<GameObject> PartyGameObjects => partyGameObjects;
    public List<GameObject> BenchGameObjects => benchGameObjects;
    public List<PersonajeHandler> OutMembers => outMembers;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        partyGameObjects = new List<GameObject>();
        benchGameObjects = new List<GameObject>();
        outMembers = new();

    }

        private void SetDontDestroy(GameObject memberToAdd)
    {
        
        /*CharacterMovement[] cMs = FindObjectsOfType<CharacterMovement>();
        foreach (CharacterMovement cm in cMs)
        {
            Debug.Log($"{cm.name} == {memberToAdd.name} ? {cm.gameObject.name == memberToAdd.name}");
            if (cm.gameObject.name == memberToAdd.name && cm.)
                Destroy(cm.gameObject);
        }*/
        DontDestroyOnLoad(memberToAdd);
    }

    public void InitParty()
    {
        Initialize();
        List<GameObject> allMembersGO;
        if (partyGameObjects == null || partyGameObjects.Count <= 0 || partyGameObjects[0] == null)
        {
            allMembersGO = GetGameObjects(startParty, heroPrefab);

            AddToParty(allMembersGO, true);

        }

    }

    private void AddToParty(List<GameObject> members, bool reset)
    {
        for (int i = 0; i < members.Count; i++)
        {
            GameObject member = members[i];
            AddToParty(member, reset);
        }
    }

    public void AddToParty(GameObject memberToAdd, bool reset)
    {
        PersonajeHandler newMember = memberToAdd.GetComponent<PersonajeHandler>();
        PersonajeHandlerSO newMemberModel = newMember.SO;

        if (!reset)
        {
            newMember = GetSavedStats(newMember);
            memberToAdd = CreateSavedCharacterGO(newMember, heroPrefab);
        }
        else
            memberToAdd = CreateCharacterGO(newMemberModel);


        if (memberToAdd == null) return;

        if (partyGameObjects.Count < activePartyCount)
            partyGameObjects.Add(memberToAdd);
        else
            benchGameObjects.Add(memberToAdd);

        SetDontDestroy(memberToAdd);

        memberToAdd.GetComponent<SpriteRenderer>().enabled = false;
    }

    private PersonajeHandler GetSavedStats(PersonajeHandler character)
    {
        PersonajeHandler personajeHandler = character.GetComponent<PersonajeHandler>();
        for (int i = 0; i < outMembers.Count; i++)
        {
            PersonajeHandler savedStats = outMembers[i].GetComponent<PersonajeHandler>();
            if (savedStats.SO.Equals(personajeHandler.SO))
                return savedStats;
        }
        return null;
    }

    internal void Remove(PersonajeHandlerSO character, bool save)
    {
        PersonajeHandler characterToRemove = GetAlly(character);
        if (characterToRemove == null) return;
        GameObject characterTRGO = characterToRemove.gameObject;

        if (save)
        {
            OutMember(characterToRemove);
            Remove(character, false);
        }

        FindAndRemove(characterTRGO, partyGameObjects);
        FindAndRemove(characterTRGO, benchGameObjects);
    }

    private void FindAndRemove(GameObject characterGO, List<GameObject> list)
    {
        if (list.Contains(characterGO))
            list.Remove(characterGO);
    }

    private void OutMember(PersonajeHandler character)
    {
        for (int i = 0; i < outMembers.Count; i++)
        {
            PersonajeHandler outMember = outMembers[i];
            if (outMember.SO == character.SO)
            {
                outMembers[i] = character;
                return;
            }
        }
        outMembers.Add(character);
    }

    public List<GameObject> GetGameObjects(List<PersonajeHandlerSO> list, GameObject prefab)
    {
        List<GameObject> listGameObjects = new();

        for (int i = 0; i < list.Count; i++)
        {
            GameObject characterGO = CreateCharacterGO(list[i]);

            listGameObjects.Add(characterGO);

            if (SceneHandler.ActualPosition != null)
                characterGO.transform.position = SceneHandler.ActualPosition;
        }
        return listGameObjects;
    }

    public GameObject CreateCharacterGO(PersonajeHandlerSO handlerModel)
    {
        GameObject characterGO = Instantiate(heroPrefab, Vector3.zero, Quaternion.identity);
        characterGO.name = handlerModel.stats.name;

        PersonajeHandler characterHandler = characterGO.GetComponent<PersonajeHandler>();
        characterHandler.Init(handlerModel);

        return characterGO;
    }

    public GameObject CreateCharacterGO(PersonajeHandler handler)
    {
        GameObject characterGO = Instantiate(heroPrefab, Vector3.zero, Quaternion.identity);
        characterGO.name = handler.Stats.Character.Name;

        PersonajeHandler characterHandler = characterGO.GetComponent<PersonajeHandler>();
        characterHandler.CopyFrom(handler);

        return characterGO;
    }

    private GameObject CreateSavedCharacterGO(PersonajeHandler newMember, GameObject prefab)
    {
        if (prefab == null) prefab = heroPrefab;

        GameObject characterGO = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        characterGO.name = newMember.Stats.Name;

        PersonajeHandler characterHandler = characterGO.GetComponent<PersonajeHandler>();
        characterHandler.CopyFrom(newMember);

        return characterGO;
    }

    internal void UpdateParty(List<GameObject> fighters)
    {
        partyGameObjects.RemoveAll(GameObject => true);
        partyGameObjects.Clear();

        for (int i = 0; i < fighters.Count; i++)
        {
            //            Debug.Log($"Update Party Fighter ID {fighters[i].GetInstanceID()} Level {fighters[i].GetComponent<PersonajeHandler>().Stats.Level}");

            partyGameObjects.Add(fighters[i]); //Try with addToParty(fighters[i])
        }
    }

    internal List<GameObject> GetHeros(List<GameObject> heroesInBattle) //Get HeroPrefab from HeroInBattle
    {
        List<GameObject> heroes = new();

        for (int i = 0; i < heroesInBattle.Count; i++)
        {
            GameObject heroIB = heroesInBattle[i];
            GameObject prefab = Instantiate(heroPrefab);
            prefab.name = heroIB.name;
            prefab.transform.position = SceneHandler.ActualPosition;
            PersonajeHandler prefabHandler = prefab.GetComponent<PersonajeHandler>();
            prefabHandler.CopyFrom(heroIB.GetComponent<PersonajeHandler>());
            prefab.GetComponent<SpriteRenderer>().enabled = false;
            SetDontDestroy(prefab);
            heroes.Add(prefab);
        }
        return heroes;
    }

    internal int GetIndex(PersonajeHandler heroHandler)
    {
        for (int i = 0; i < PartyGameObjects.Count; i++)
            if (partyGameObjects[i].GetComponent<PersonajeHandler>() == heroHandler)
                return i;
        return -1;
    }

    internal PersonajeHandler GetPartyMember(int indexToFind)
    {
        return PartyGameObjects[indexToFind].GetComponent<PersonajeHandler>();
    }
    internal PersonajeHandler GetAlly(int index)
    {
        List<GameObject> allies = new();
        allies.AddRange(PartyGameObjects);
        allies.AddRange(BenchGameObjects);

        if (allies.Count > index)
            return allies[index].GetComponent<PersonajeHandler>();
        return null;
    }

    internal PersonajeHandler GetAlly(PersonajeHandlerSO ally)
    {
        PersonajeHandler allyToReturn;
        allyToReturn = GetAlly(ally, PartyGameObjects);
        if (allyToReturn == null)
            allyToReturn = GetAlly(ally, BenchGameObjects);


        return allyToReturn;
    }

    private PersonajeHandler GetAlly(PersonajeHandlerSO ally, List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            PersonajeHandler character = list[i].GetComponent<PersonajeHandler>();
            if (character.SO == ally)
                return character;
        }
        return null;
    }

    internal PersonajeHandler GetCharacter(PersonajeHandlerSO characterModel)
    {
        List<GameObject> allEverList = GetAllEverAlliesListGO();
        PersonajeHandler character = GetAlly(characterModel, allEverList);
        if (character == null)
        {
            GameObject simulatedCharacter = CreateCharacterGO(characterModel);
            return simulatedCharacter.GetComponent<PersonajeHandler>();
        }
        return character;
    }

    internal void SetupPlayer()
    {
        if (PartyGameObjects.Count <= 0) return;
        DeactivateAllMovement(true);

        PersonajeHandler leader = GetLeader();

        if (leader == null) { Debug.Log("There is not posible leader"); return; }

        leader.transform.position = SceneHandler.ActualPosition;
        leader.GetComponent<PlayerMovement>().enabled = true;
        leader.GetComponent<SpriteRenderer>().enabled = true;
    }

    internal PersonajeHandler GetLeader()
    {
        if(PartyGameObjects == null) return null;
        if (PartyGameObjects.Count > 0)
            for (int i = 0; i < PartyGameObjects.Count; i++)
            {
                GameObject playerLeader = PartyGameObjects[i];
                if (playerLeader != null && !playerLeader.GetComponent<PersonajeHandler>().IsDead())
                    return playerLeader.GetComponent<PersonajeHandler>();
            }
        return null;
    }

    internal void DeactivateAllMovement(bool full)
    {
        if (PartyGameObjects.Count <= 0) return;
        for (int i = 0; i < PartyGameObjects.Count; i++)
        {
            PlayerMovement playerMovement = PartyGameObjects[i].GetComponent<PlayerMovement>();
            if (playerMovement != null) playerMovement.Deactivate(full);
        }
    }

    internal void SwitchParty(GameObject character)
    {
        KeyValuePair<List<GameObject>, int> location = Search(character);
        if (location.Key == partyGameObjects && partyGameObjects.Count > 1)
        { //If main party & has more than 1 character
            ListChange(benchGameObjects, partyGameObjects, character);
        }
        else if (location.Key == benchGameObjects && partyGameObjects.Count < activePartyCount)
        { //If bench party & has less than active party count
            ListChange(partyGameObjects, benchGameObjects, character);
        }
    }

    private void ListChange(List<GameObject> origin, List<GameObject> destiny, GameObject character)
    {
        origin.Add(character);
        destiny.Remove(character);
    }

    internal void ChangePositions(GameObject characterSelected, PersonajeHandler character)
    {
        GameObject characterGO = character.gameObject;

        KeyValuePair<List<GameObject>, int> locationCHSelected = Search(characterSelected);
        KeyValuePair<List<GameObject>, int> location = Search(characterGO);

        GameObject aux = characterGO;

        location.Key[location.Value] = characterSelected;
        locationCHSelected.Key[locationCHSelected.Value] = aux;
    }

    private KeyValuePair<List<GameObject>, int> Search(GameObject character)
    {
        for (int i = 0; i < partyGameObjects.Count; i++)
            if (character == partyGameObjects[i]) return new KeyValuePair<List<GameObject>, int>(partyGameObjects, i);
        for (int i = 0; i < benchGameObjects.Count; i++)
            if (character == benchGameObjects[i]) return new KeyValuePair<List<GameObject>, int>(benchGameObjects, i);
        return new KeyValuePair<List<GameObject>, int>();

    }

    internal List<PersonajeHandler> GetAllAllies()
    {
        List<GameObject> allAllies = GetAllAlliesGO();

        List<PersonajeHandler> allAlliesHandler = new();

        for (int i = 0; i < allAllies.Count; i++)
            allAlliesHandler.Add(allAllies[i].GetComponent<PersonajeHandler>());

        return allAlliesHandler;
    }

    private List<PersonajeHandler> GetAllEverAlliesList()
    {
        List<PersonajeHandler> allAllies = GetAllAllies();
        allAllies.AddRange(outMembers);

        return allAllies;
    }

    private List<GameObject> GetAllAlliesGO()
    {
        List<GameObject> allAllies = new();
        allAllies.AddRange(partyGameObjects);
        allAllies.AddRange(benchGameObjects);

        return allAllies;
    }

    private List<GameObject> GetAllEverAlliesListGO()
    {
        List<PersonajeHandler> allEverAllies = GetAllEverAlliesList();
        List<GameObject> allEverAlliesGO = new();
        foreach (PersonajeHandler character in allEverAllies)
            allEverAlliesGO.Add(character.gameObject);
        return allEverAlliesGO;
    }
}

