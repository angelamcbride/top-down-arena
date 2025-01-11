using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonPersistant<AudioManager>
{
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;
    private AudioSource musicSource;

    public float SFXVolume { get; set; } = 1f;
    public float MusicVolume { get; set; } = 0.4f;
    public float hoverSoundCooldown = 0.05f;
    public bool musicEnabledPref;
    //public bool musicEnabled = true;
    public bool musicPlaying;
    [SerializeField] private const string backgroundMusicTrack = "BackgroundMusic";

    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();

    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // For background music

        LoadAudioClips();
    }

    public void StartAudioManager()
    {
        if (musicEnabledPref && !musicPlaying)
        {
            PlayMusic();
            //Debug.Log("Music was enabled in the player pref.");
            //musicPlaying = true;
        }
    }

    public void ToggleMusic()
    {
        musicEnabledPref = !musicEnabledPref;
        //Debug.Log("musicEnabled was " + musicEnabled + ". now it's " + !musicEnabled);
        //AudioManager.Instance.musicEnabled = !AudioManager.Instance.musicEnabled;
        if (musicEnabledPref)
        {
            Instance.PlayMusic();
            PlayerPrefs.SetInt("Music", 0);
            Debug.Log("Music enabled.");
            //musicEnabled = true;
        }
        else
        {
            Instance.StopMusic();
            PlayerPrefs.SetInt("Music", 1);
            Debug.Log("Music disabled.");
            //musicEnabled = false;
        }
        UIManager.Instance.SetMusicCheckbox(musicEnabledPref);
    }

    private void LoadAudioClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
        foreach (AudioClip clip in clips)
        {
            audioClips[clip.name] = clip;
        }
        ValidateRequiredClips();
    }

    private void ValidateRequiredClips()
    {
        string[] requiredClips = { "Gunshot", "PlayerHit", "MobHit", "Death", "BackgroundMusic" };
        foreach (var clipName in requiredClips)
        {
            if (!audioClips.ContainsKey(clipName))
            {
                Debug.LogWarning($"Required audio clip '{clipName}' is missing!");
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if (audioClips.ContainsKey(soundName))
        {
            float currentTime = Time.time;
            if (soundCooldowns.ContainsKey(soundName) && currentTime - soundCooldowns[soundName] < hoverSoundCooldown)
            {
                return;
            }
            soundCooldowns[soundName] = currentTime;
            audioSource.PlayOneShot(audioClips[soundName], SFXVolume);
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
        }
    }

    public void PlayMusic(string musicName=backgroundMusicTrack)
    {
        if (audioClips.ContainsKey(musicName) && !musicPlaying)
        {
            musicSource.clip = audioClips[musicName];
            musicSource.volume = MusicVolume;
            musicSource.Play();
            musicPlaying = true;
        }
        else
        {
            Debug.LogWarning("Music " + musicName + " not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicPlaying = false;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        audioSource.volume = SFXVolume;
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        musicSource.volume = MusicVolume;
    }

    public void CrossfadeMusic(string newMusicName, float fadeDuration)
    {
        StartCoroutine(FadeOutAndPlayNewMusic(newMusicName, fadeDuration));
    }

    private IEnumerator FadeOutAndPlayNewMusic(string newMusicName, float fadeDuration)
    {
        float startVolume = musicSource.volume;

        // Fade out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();

        // Play new music
        if (audioClips.ContainsKey(newMusicName))
        {
            musicSource.clip = audioClips[newMusicName];
            musicSource.Play();
        }

        // Fade in
        while (musicSource.volume < MusicVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}
