using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The AudioManager class provides static methods for playing audio
/// </summary>
public class AudioManager : MonoBehaviour
{
    // A current instance of the AudioManager
    private static AudioManager current;

    /// <summary>
    /// A class containing all of the game's sound effects
    /// </summary>
    public static SoundEffects SoundEffects {
        get {
            CheckInitialisation();
            return current.soundEffects;
        }
    }

    // A scriptable object containing all the game's sound effects
    public SoundEffects soundEffects;

    // The maximum number of sound effects that can be played simultaneously
    // When the capacity is exceeded, the oldest sounds will be overridden
    public int maximumConcurrentSoundEffects = 30;

    // An array of the audio sources used for sound effects
    private List<AudioSource> soundEffectSources;

    // A tracker for the current sound effect audio source
    private int currentSoundEffectSource = 0;

    // Check that the current instance is set, and that it's audio sources have been created
    private static void CheckInitialisation() {
        if (current == null) current = FindAnyObjectByType<AudioManager>();
        if (current == null) Debug.LogError("An attempt to use the AudioManager was made, but no Audio Manager is present");
        if (current.soundEffectSources == null) current.InitialiseSoundEffectSources();
    }

    /// <summary>
    /// Play a sound effect
    /// </summary>
    /// <param name="soundEffect">The sound effect to be played</param>
    public static void Play(SoundEffect soundEffect) {
        CheckInitialisation();
        current.Perform(soundEffect);
    }

    /// <summary>
    /// Use this function instead of <see cref="Play"/> during development when the desired sound effect has yet to be implemented 
    /// </summary>
    /// <param name="description">A description of the sound effect</param>
    public static void LogSoundEffect(string description) {
        Debug.LogWarning($"Sound effect '{description}' needs to be implemented.");
    }

    // Create the audio sources to be used for playing sound effects
    private void InitialiseSoundEffectSources() {
        soundEffectSources = new List<AudioSource>();
        for (int i = 0; i < maximumConcurrentSoundEffects; i++)
        {
            soundEffectSources.Add(gameObject.AddComponent<AudioSource>());
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

        // Play the sound
        audioSource.Play();
    }
}
