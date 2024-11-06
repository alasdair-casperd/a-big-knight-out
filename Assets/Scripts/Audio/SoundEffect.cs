using UnityEngine;

[System.Serializable]
public class SoundEffect {

    [Header("Source")]
    public AudioClip audioClip;

    [Header("Pitch and Volume")]
    [Range(.1f, 3f)] public float pitch = 1f;
    [Range(0f, 2f)] public float volume = 1f;

    [Header("Random Variation")]
    [Range(0f, 1f)]public float volumeVariance = .1f;
    [Range(0f, 1f)] public float pitchVariance = .1f;
}