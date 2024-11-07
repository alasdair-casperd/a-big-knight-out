using UnityEngine;

[System.Serializable]
public class BackgroundSound {

    [Header("Source")]
    public AudioClip audioClip;

    [Header("Properties")]
    [Range(0f, 2f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;
    public bool looping = true;
}