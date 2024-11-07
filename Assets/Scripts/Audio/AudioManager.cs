using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The AudioManager class provides static methods for playing sound effects and background sounds
/// </summary>
public class AudioManager : MonoBehaviour
{
    // A current instance of the AudioManager
    private static AudioManager current;

    /// <summary>
    /// A scriptable object all of the game's sound effects
    /// </summary>
    public static SoundEffects SoundEffects {
        get {
            CheckInitialisation();
            return current.soundEffects;
        }
    }

    /// <summary>
    /// A scriptable object containing all of the game's background sounds
    /// </summary>
    public static BackgroundSounds BackgroundSounds {
        get {
            CheckInitialisation();
            return current.backgroundSounds;
        }
    }

    [Header("Sound Effects")]

    // A scriptable object containing all the game's sound effects
    public SoundEffects soundEffects;

    // The maximum number of sound effects that can be played simultaneously
    // When the capacity is exceeded, the oldest sounds will be overridden
    public int maximumConcurrentSoundEffects = 30;

    // An array of the audio sources used for sound effects
    private List<AudioSource> soundEffectSources;

    // A tracker for the current sound effect audio source
    private int currentSoundEffectSource = 0;

    [Header("Background Sounds")]

    // A scriptable object containing all the game's sound effects
    public BackgroundSounds backgroundSounds;

    // The maximum number of sound effects that can be played simultaneously
    // When the capacity is exceeded, the oldest sounds will be overridden
    public int maximumConcurrentBackgroundSounds = 10;

    // An array of the audio sources used for sound effects
    private List<AudioSource> backgroundSoundSources;

    // A tracker for the current sound effect audio source
    private int currentBackgroundSoundSource = 0;

    [Header("Music Tracks")]

    public bool loopMusic = true;

    // Check that the current instance is set, and that it's audio sources have been created
    private static void CheckInitialisation() {
        if (current == null) current = FindAnyObjectByType<AudioManager>();
        if (current == null) Debug.LogError("An attempt to use the AudioManager was made, but no Audio Manager is present");
        if (current.soundEffectSources == null) current.InitialiseSoundEffectSources();
        if (current.backgroundSoundSources == null) current.InitialiseBackgroundSoundSources();
    }

    /// <summary>
    /// Play a sound effect. Sound effects should be used for short sounds which play only once.
    /// </summary>
    /// <param name="soundEffect">The sound effect to be played</param>
    public static void Play(SoundEffect soundEffect) {
        CheckInitialisation();
        current.Perform(soundEffect);
    }

    /// <summary>
    /// Play a background sound. Background sounds should be used for longer sounds or sounds which loop indefinitely.
    /// </summary>
    /// <param name="backgroundSound">The background sound to be played.</param>
    public static void Play(BackgroundSound backgroundSound) {
        CheckInitialisation();
        current.Perform(backgroundSound);
    }

    /// <summary>
    /// Use this function instead of Play(SoundEffect soundEffect) during development when the desired sound effect has yet to be implemented 
    /// </summary>
    /// <param name="description">A description of the sound effect</param>
    public static void LogSoundEffect(string description) {
        Debug.LogWarning($"Sound effect '{description}' needs to be implemented.");
    }

    /// <summary>
    /// Use this function instead of Play(BackgroundSound backgroundSound) during development when the desired background sound has yet to be implemented 
    /// </summary>
    /// <param name="description">A description of the backgroundSound</param>
    public static void LogBackgroundSound(string description) {
        Debug.LogWarning($"Background Sound '{description}' needs to be implemented.");
    }

    // Create the audio sources to be used for playing sound effects
    private void InitialiseSoundEffectSources() {
        soundEffectSources = new List<AudioSource>();
        for (int i = 0; i < maximumConcurrentSoundEffects; i++)
        {
            soundEffectSources.Add(gameObject.AddComponent<AudioSource>());
        }                
    }

    // Create the audio sources to be used for playing background sounds
    private void InitialiseBackgroundSoundSources() {
        backgroundSoundSources = new List<AudioSource>();
        for (int i = 0; i < maximumConcurrentBackgroundSounds; i++)
        {
            backgroundSoundSources.Add(gameObject.AddComponent<AudioSource>());
        }                
    }

    // Play a sound effect
    private void Perform(SoundEffect soundEffect)
    {         
        // Select an AudioSource to use
        AudioSource audioSource = soundEffectSources[currentSoundEffectSource];
        currentSoundEffectSource = (currentSoundEffectSource + 1) % maximumConcurrentSoundEffects;         

        // Set the properties of the AudioSource from SoundEffect data
        audioSource.clip = soundEffect.audioClip;         
        float randomVolumeMultiplier = 1 + Random.Range(-soundEffect.volumeVariance / 2, soundEffect.volumeVariance / 2);
        float randomPitchMultiplier = 1 + Random.Range(-soundEffect.pitchVariance / 2, soundEffect.pitchVariance / 2);
        audioSource.volume = soundEffect.volume * randomVolumeMultiplier;
        audioSource.pitch = soundEffect.pitch * randomPitchMultiplier;

        // Play the sound effect
        audioSource.Play();
    }

    // Play a background sound
    private void Perform(BackgroundSound backgroundSound)
    {         
        // Select an AudioSource to use
        AudioSource audioSource = backgroundSoundSources[currentBackgroundSoundSource];
        currentBackgroundSoundSource = (currentBackgroundSoundSource + 1) % maximumConcurrentBackgroundSounds;         

        // Set the properties of the AudioSource from Background data
        audioSource.clip = backgroundSound.audioClip;         
        audioSource.volume = backgroundSound.volume;
        audioSource.pitch = backgroundSound.pitch;
        audioSource.loop = backgroundSound.looping;

        // Play the background sound
        audioSource.Play();
    }
}
