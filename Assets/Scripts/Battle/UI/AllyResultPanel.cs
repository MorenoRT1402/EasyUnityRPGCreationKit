using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AllyResultPanel : MonoBehaviour
{
    [Header("Result Panel")]
    [SerializeField] private TextMeshProUGUI expObtainAllyName;
    [SerializeField] private TextMeshProUGUI expToAddTMP, expNextLevelTMP, levelUpTMP;
    private FighterStateMachine fighter;

    public static Dictionary<int, bool> characterAnimationStates = new Dictionary<int, bool>();

    //    public bool isAnimating = false;
    public static bool StopAllAnimations;

    internal void Run(int expToAdd, GameObject character)
    {
        int IDCharacter = character.GetInstanceID();
        if (characterAnimationStates.TryGetValue(IDCharacter, out _))
        {
            return;
        }

        expObtainAllyName.SetText(fighter.Hero.Stats.Name);
        //        expToAddTMP.SetText(expToAdd.ToString());
        int expNextLevel = (int)fighter.Hero.GetParcialExpNextLevel();
        //        expNextLevelTMP.SetText((expNextLevel).ToString());
        levelUpTMP.gameObject.SetActive(false);
        if (!characterAnimationStates.TryGetValue(IDCharacter, out _) && isActiveAndEnabled)
        {
            characterAnimationStates[IDCharacter] = true;
            StartCoroutine(AnimateExp(expToAdd, expNextLevel, character));
        }
    }

    internal void SetData(FighterStateMachine f)
    {
        _ = f.Hero.Stats.GetDictAtr();
        fighter = f;
    }

    private IEnumerator AnimateExp(int expToAdd, int expNextLevel, GameObject character)
    {
        int IDCharacter = character.GetInstanceID();

        if (expToAdd <= 0)
            yield break;
        StopAllAnimations = false;

        float duration = 5.0f;
        float timer = 0.0f;

        PersonajeHandler fighterHandler = character.GetComponent<PersonajeHandler>();

        int expAdded = 0;
        int startLevel = fighterHandler.Stats.Character.Level;
        int startExpToAdd = expToAdd;

        while (timer < duration && !StopAllAnimations)
        {

            timer += Time.deltaTime;

            float progress = timer / duration;

            int currentExpToAdd = Mathf.RoundToInt(Mathf.Lerp(startExpToAdd, 0, progress));

            expToAddTMP.SetText(currentExpToAdd.ToString());

            int reverseExpToAdd = expToAdd - currentExpToAdd;

            fighterHandler.AddExp(reverseExpToAdd - expAdded);

            expAdded = reverseExpToAdd;

            int currentExpNextLevel = (int)fighterHandler.GetParcialExpNextLevel();

            expNextLevelTMP.SetText(currentExpNextLevel.ToString());


            if (startLevel < fighterHandler.Stats.Character.Level)
            {
                levelUpTMP.gameObject.SetActive(true);
                //                Debug.Log("Animate Exp Level Up Fighter ID " + fighter.GetInstanceID() + " Level " + fighterHandler.Stats.Level);
            }

            if (expToAdd <= 0)
                break;


            yield return null;
        }
        characterAnimationStates[IDCharacter] = false;
        StopAllAnimations = true;
        BattleManagerUI.Instance.SetActive(UIElements.FINISH_BUTTON, true);
        yield break;
    }

}
