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

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // For background music
    }

    private void Start()
    {
        LoadAudioClips();
        AudioManager.Instance.PlayMusic("BackgroundMusic"); // Can move this at a later point. For now always play music.
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
            audioSource.PlayOneShot(audioClips[soundName], SFXVolume);
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found!");
        }
    }

    public void PlayMusic(string musicName)
    {
        if (audioClips.ContainsKey(musicName))
        {
            musicSource.clip = audioClips[musicName];
            musicSource.volume = MusicVolume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music " + musicName + " not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
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
