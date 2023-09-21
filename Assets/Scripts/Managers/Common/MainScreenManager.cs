using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenManager : Singleton<MainScreenManager>
{
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private AudioClip audioInMenu;

    [Header("Glosary")]
    [SerializeField] private string newGameText = "New Game";
    [SerializeField] private string loadGameText = "Load Game";
    [SerializeField] private string exitGameText = "Return To Desktop";


    [Header("Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private TextMeshProUGUI newGameTMP, loadGameTMP, exitGameTMP;
    [SerializeField] private GameObject loadGameSpace;
    [SerializeField] private GameObject saveSlotSpace;


    [Header("Prefabs")]
    [SerializeField] private GameObject saveSlotPrefab;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        Inputs();
    }

    private void Inputs()
    {
        InputManager IM = InputManager.Instance;
        if (Input.GetKeyDown(IM.back))
            Back();
    }

    private void Back()
    {
        if (loadGameSpace.activeSelf)
            loadGameSpace.SetActive(false);
        else
            Application.Quit();
    }

    public void Initialize()
    {
        UpdateUI();
        AudioManager.Instance.Play(audioInMenu, AudioType.MUSIC, -1);
    }

    public void UpdateUI()
    {
        loadGameSpace.SetActive(false);
        backgroundImage.sprite = backgroundSprite;
        titleTMP.text = GameManager.Instance.GameTitle;
        newGameTMP.text = newGameText;
        loadGameTMP.text = loadGameText;
        exitGameTMP.text = exitGameText;
    }

    public void NewGame()
    {
        GameManager.NewGame();
    }
    public void LoadGame()
    {
        loadGameSpace.SetActive(true);
        UIManager.UpdateSavePanel(saveSlotPrefab, saveSlotSpace, SaveManager.Operation.LOAD);

    }
    internal void Load(int i)
    {
        SaveManager.LoadGame(SaveManager.Instance.GetFullPath(i));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
