using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : Singleton<SettingsManager>
{
    public enum SettingsOptions{BRIGHTNESS}
    [Header("Glosary")]
    public string audioTitleText = "Audio";
    public string musicVolumeText = "Music Volume";
    public string ambientVolumeText = "Ambient Volume";
    public string soundVolumeText = "Sound Volume";
    public string screenTitleText = "Screen";
    public string brightnessText = "Brightness";
    public string qualityText = "Quality";
    public string resolutionText = "Resolution";
    public string fullScreenText = "Full Screen";
    public string closeButtonText = "Close";
    public string defaultValuesText = "Default";
    public string saveButtonText = "Save";
    [Header("Glosary/Quality")]
    public string veryLowQuality = "Very Low";
    public string lowQuality = "Low";
    public string mediumQuality = "Medium";
    public string highQuality = "High";
    public string veryHighQuality = "Very High";
    public string UltraQuality = "Ultra";


    /*
    [Header("Colors")]
    public Color musicVolumeBackground, musicVolumeValue;
    public Color ambientVolumeBackGround, ambientVolumeValue;
    public Color soundVolumeBackGround, soundVolumeValue;
    public Color brightnessBackGround, brightnessValue;
    */

    [Header("Initial Values")]
    [Range(0, 1)] public float music = 0.5f;
    [Range(0, 1)] public float ambient = 0.5f;
    [Range(0, 1)] public float sounds = 0.5f;
    [Range(0, 1)] public float brightness = 1f;

    public float MasterMusic { get { return PlayerPrefs.GetFloat("music", 0.5f); } set { PlayerPrefs.SetFloat("music", value); } }
    public float MasterAmbient { get { return PlayerPrefs.GetFloat("ambient", 0.5f); } set { PlayerPrefs.SetFloat("ambient", value); } }
    public float MasterSounds { get { return PlayerPrefs.GetFloat("sounds", 0.5f); } set { PlayerPrefs.SetFloat("sounds", value); } }
    public float Brightness { get { return PlayerPrefs.GetFloat("brightness", 0.1f); } set { PlayerPrefs.SetFloat("brightness", value); } }

    public int QualityLevel { get { return QualitySettings.GetQualityLevel(); } set { QualitySettings.SetQualityLevel(value); } }
    public float ResWidt { get { return PlayerPrefs.GetFloat("resW", 1920); } set { PlayerPrefs.SetFloat("resW", value); } }
    public float ResHeight { get { return PlayerPrefs.GetFloat("resH", 1080); } set { PlayerPrefs.SetFloat("resH", value); } }
    public bool FullScreen { get { return PlayerPrefs.GetString("fullScreen", "false") == "true"; } set { PlayerPrefs.SetString("fullScreen", value.ToString()); } }

    public void InitialValues()
    {
        MasterMusic = music;
        MasterAmbient = ambient;
        MasterSounds = sounds;
        Brightness = 1-brightness;
        QualityLevel = QualitySettings.GetQualityLevel();
        ResWidt = Screen.currentResolution.width;
        ResHeight = Screen.currentResolution.height;
        FullScreen = Screen.fullScreen;
    }
}
