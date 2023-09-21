using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType { NONE, MUSIC, AMBIENTAL, SOUND }
public class AudioManager : Singleton<AudioManager>
{
    #region Variables
    #region Public
    [Header("Parameters")]
    [Header("Parameters/BGM")]
    public AudioClip initialAudio;
    public AudioClip battleMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;

    [Header("Parameters/Sounds")]
    public AudioClip selectSound;
    public AudioClip cancelSound;
    public AudioClip encounterSound;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambientalSource;
    public AudioSource soundsSource;
    #endregion

    #region Private

    private readonly AudioClip savedMusic;
    private readonly AudioClip savedAmbiental;
    private readonly AudioClip savedSound;
    private Dictionary<AudioSource, AudioClip> audioStorage;

    #endregion
    #endregion

    #region Methods

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        InitDict();
        musicSource.clip = initialAudio;
        Play(initialAudio, AudioType.MUSIC, -1);
    }

    private void InitDict()
    {
        audioStorage = new(){
            { musicSource, savedMusic },
            { ambientalSource, savedAmbiental },
            { soundsSource, savedSound },
        };
    }

    #region Main

    internal void Play(AudioClip audio, AudioType type, float volume)
    {
        AudioSource source = GetSource(type);
        if (source == null) return;

        source.clip = audio;
        if (volume >= 0)
            source.volume = GetRealVolume(type, volume);

        source.Play();
    }

    private float GetRealVolume(AudioType type, float volume)
    {
        float master = GetMaster(type);

        return volume * master;
    }

    private float GetMaster(AudioType type)
    {
        SettingsManager STM = SettingsManager.Instance;

        return type switch
        {
            AudioType.MUSIC => STM.MasterMusic,
            AudioType.AMBIENTAL => STM.MasterAmbient,
            AudioType.SOUND => STM.MasterSounds,
            _ => 0
        };
    }

    internal void ChangeVolume(AudioType audioType, float volume)
    {
        AudioSource source = GetSource(audioType);

        source.volume = GetRealVolume(audioType, volume);
    }

    internal void UpdateVolume(AudioType type, float value)
    {
        AudioSource source = GetSource(type);
        float playVolume = source.volume / GetMaster(type);
        SetMaster(type, value);
        source.volume = GetRealVolume(type, playVolume);
    }

    private void SetMaster(AudioType type, float value)
    {
        SettingsManager STM = SettingsManager.Instance;
        switch (type)
        {
            case AudioType.MUSIC:
                STM.MasterMusic = value;
                break;
            case AudioType.AMBIENTAL:
                STM.MasterAmbient = value;
                break;
            case AudioType.SOUND:
                STM.MasterSounds = value;
                break;
        }
    }

    private AudioSource GetSource(AudioType type)
    {
        return type switch
        {
            AudioType.MUSIC => musicSource,
            AudioType.AMBIENTAL => ambientalSource,
            AudioType.SOUND => soundsSource,
            _ => null
        };

    }

    internal void Pause(AudioType audioType)
    {
        AudioSource source = GetSource(audioType);

        source.Pause();
    }

    internal void Save(AudioType audioType)
    {
        AudioSource source = GetSource(audioType);

        audioStorage[source] = source.clip;
    }

    internal void Replay(AudioType audioType)
    {
        AudioSource source = GetSource(audioType);

        AudioClip audio = audioStorage[source];
        Play(audio, audioType, -1);
    }

    private void ReplayAll()
    {
        Replay(AudioType.MUSIC);
        Replay(AudioType.AMBIENTAL);
        Replay(AudioType.SOUND);
    }

    public void PauseAll()
    {
        Pause(AudioType.MUSIC);
        Pause(AudioType.AMBIENTAL);
        Pause(AudioType.SOUND);
    }

    private void SaveAll()
    {
        Save(AudioType.MUSIC);
        Save(AudioType.AMBIENTAL);
        Save(AudioType.SOUND);
    }

    private void SaveAndPlay(AudioClip audio, AudioType type, int volume)
    {
        Save(type);
        Play(audio, type, volume);
    }

    #endregion

    #region Battle

    internal void PlayEncounterSound()
    {
        if (encounterSound != null)
            Play(encounterSound, AudioType.SOUND, -1);
    }

    internal void PlayLoseMusic()
    {
        if (loseMusic != null)
            musicSource.PlayOneShot(loseMusic);
    }
    internal void PlayWinMusic()
    {
        if (winMusic != null)
            musicSource.PlayOneShot(winMusic);
    }

    internal void Battle(bool battle)
    {
        if (battle)
        {
            SaveAll();
            PauseAll();
            Play(battleMusic, AudioType.MUSIC, -1);
        }
        else
            ReplayAll();
    }

    internal void PlayActualDungeonMusic()
    {
        DungeonManager DM = DungeonManager.Instance;

        Play(DM.actualDungeon.Music, AudioType.MUSIC, -1);
        Play(DM.actualDungeon.Ambiental, AudioType.AMBIENTAL, -1);
    }

    #endregion

    #endregion
}
