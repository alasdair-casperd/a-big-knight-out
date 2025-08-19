using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component used to give a game object a simple idle floating animation
/// </summary>
public class FloatingAnimator : MonoBehaviour
{
    /// <summary>
    /// The time for a full rotation.
    /// </summary>
    public float RotationTime = 3f;

    /// <summary>
    /// The time take for a loop of the bobbing animation.
    /// </summary>
    public float BobTime = 2f;

    /// <summary>
    /// The height of the bob animation.
    /// </summary>
    public float BobMagnitude = 1f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // Handle spin
        transform.rotation = initialRotation * Quaternion.Euler(0, (float)(Time.time * 360 / RotationTime), 0);

        // Handle bob
        transform.position = initialPosition + BobMagnitude * new Vector3(0, (float)Math.Sin(Time.time * 2 * Math.PI / BobTime), 0);
    }
}
