using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    // Reference to the Light component
    private Light pointLight;

    // Parameters for flicker effect
    [Header("Flicker Settings")]
    public float intensityBase = 1f;  // Base intensity of the light
    public float intensityRange = 0.5f;  // Maximum fluctuation amount
    public float flickerSpeed = 1f;  // Speed of the flicker

    private float noiseOffset;  // Offset for unique flicker per light

    void Start()
    {
        // Get the Light component attached to this GameObject
        pointLight = GetComponent<Light>();

        // Set a random offset for Perlin noise to make each light flicker differently
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Calculate new intensity using Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);
        float intensity = intensityBase + (noise - 0.5f) * 2 * intensityRange;

        // Apply the calculated intensity to the light
        pointLight.intensity = Mathf.Clamp(intensity, 0f, intensityBase + intensityRange);
    }
}