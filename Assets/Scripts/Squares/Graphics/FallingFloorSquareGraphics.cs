using UnityEngine;
using System.Collections.Generic;

public class FallingFloorSquareGraphics : MonoBehaviour
{
    static readonly float fallDuration = 0.1f;
    static readonly float initialAngle = 2;
    static readonly float crackAngle = 5;
    static readonly float fallAngle = 15;

    [SerializeField]
    private GameObject[] fragments;

    [SerializeField]
    private Color fallenColor;

    private void Start()
    {
        transform.eulerAngles += new Vector3(0, Random.Range(0, 4) * 90, 0);
        RotateFragments(initialAngle, 0);
    }

    public void Crack()
    {
        RotateFragments(crackAngle, fallDuration);
    }

    public void Fall()
    {
        LeanTween.scale(gameObject, Vector3.one * 0.8f, fallDuration);
        // LeanTween.moveLocalY(gameObject, -0.01f, fallDuration);
        LeanTween.color(gameObject, fallenColor, fallDuration);
        RotateFragments(fallAngle, fallDuration);
    }

    private void RotateFragments(float angle, float duration)
    {
        Vector3[] initialRotations = new Vector3[fragments.Length];
        Vector3[] rotations = new Vector3[fragments.Length];

        for (int i = 0; i < fragments.Length; i++)
        {
            initialRotations[i] = fragments[i].transform.eulerAngles;
            rotations[i] = new Vector3(Random.Range(-angle, angle), Random.Range(-angle, angle), Random.Range(-angle, angle));
        }

        LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float val) =>
        {   
            // Randomly rotate the fragments
            for (int i = 0; i < fragments.Length; i++)
            {
                if (fragments[i] == null) continue;
                fragments[i].transform.eulerAngles = initialRotations[i] + val * rotations[i];
            }
        });
    }
}