using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component used to animate a gameObject
/// </summary>
public class AnimationController : MonoBehaviour
{
    /// <summary>
    /// The duration of the jump animation
    /// </summary>
    public float JumpDuration = 0.2f;

    /// <summary>
    /// The duration of the teleportation animation
    /// </summary>
    public float TeleportDuration = 0.2f;

    /// <summary>
    /// The duration of the slide animation
    /// </summary>
    public float SlideDuration = 0.2f;

    /// <summary>
    /// The duration of the move along path animation
    /// </summary>
    public float MoveAlongPathDuration = 0.15f;

    /// <summary>
    /// The height of the  jump animation
    /// </summary>
    public float JumpHeight = 2;

    /// <summary>
    /// The animation types available for movement
    /// </summary>
    public enum MovementType
    {
        None, Jump, Slide, Teleport
    }

    /// <summary>
    /// Animates movement to a given position with a jump
    /// </summary>
    public void JumpTo(Vector3 endPosition, float duration, bool gameBlocking = true)
    {
        // Applies default duration
        if (duration < 0)
        {
            duration = JumpDuration;
        }

        GameObject tweenTarget = GetTweenTarget(gameBlocking);

        // Stores the initial position and rotation
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // Animates the jump
        LeanTween.value(tweenTarget, 0, 1, duration)
        .setOnUpdate(t =>
        {
            // Translates
            Vector3 outputPosition = t * endPosition + (1 - t) * startPosition;
            outputPosition.y += JumpHeight * t * (1 - t) * 4;
            transform.position = outputPosition;

            // Rotates about the y-axis towards the end position
            Vector3 direction = endPosition - startPosition;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
        });
    }

    /// <summary>
    /// Animates teleportation via a portal to a given position
    /// </summary>
    public void TeleportTo(Vector3 endPosition, float duration, bool gameBlocking = true)
    {
        // Applies default duration
        if (duration < 0)
        {
            duration = TeleportDuration;
        }

        GameObject tweenTarget = GetTweenTarget(gameBlocking);

        // Stores the initial position and scale
        Vector3 startPosition = transform.position;
        Vector3 startLocalScale = transform.localScale;

        // Animates the teleportation
        LeanTween.value(tweenTarget, 0, 1, duration)
        .setOnUpdate(t =>
        {
            // Disappearance
            if (t < 0.5)
            {
                transform.localScale = startLocalScale * (1 - 2 * t);
            }
            // Reappearance
            else
            {
                transform.localScale = startLocalScale * (2 * t - 1);
                transform.position = endPosition;
            }
        });
    }

    /// <summary>
    /// Animates a simple linear slide to a given position
    /// </summary>
    public void SlideTo(Vector3 endPosition, float duration, bool gameBlocking = true)
    {
        // Applies default duration
        if (duration < 0)
        {
            duration = SlideDuration;
        }

        GameObject tweenTarget = GetTweenTarget(gameBlocking);

        // Stores the initial position and rotation
        Vector3 startPosition = transform.position;

        // Animates the slide
        LeanTween.value(tweenTarget, 0, 1, duration)
        .setOnUpdate(t =>
        {
            Vector3 outputPosition = t * endPosition + (1 - t) * startPosition;
            transform.position = outputPosition;
        });
    }

    private GameObject GetTweenTarget(bool gameBlocking)
    {
        return gameBlocking ? ActionQueue.GameBlockingAnimationsContainer : gameObject;
    }

    /// <summary>
    /// Animates movement along a given path
    /// </summary>
    public void MoveAlongPath(Vector2Int[] path, float duration, bool gameBlocking = true)
    {
        // Check if the path is empty
        if (path.Length == 0) return;

        // Applies default duration
        if (duration < 0)
        {
            duration = MoveAlongPathDuration;
        }

        GameObject tweenTarget = GetTweenTarget(gameBlocking);

        Vector2Int startPosition = GridUtilities.WorldToGridPos(transform.position);
        Vector2Int previousPosition = startPosition;

        // Calculate step lengths and total path length
        float totalPathLength = 0;
        float[] stepLengths = new float[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            stepLengths[i] = (path[i] - previousPosition).magnitude;
            totalPathLength += stepLengths[i];
            previousPosition = path[i];
        }

        // Determines the step of the animation
        (int stepIndex, float stepProgress) currentState(float t)
        {
            float x = 0;
            for (int i = 0; i < path.Length; i++)
            {
                x += stepLengths[i] / totalPathLength;
                if (x > t) return (i, (t - (x - stepLengths[i] / totalPathLength)) * totalPathLength / stepLengths[i]);
            }
            return (path.Length - 1, 1);
        }

        // Animates the path following
        LeanTween.value(tweenTarget, 0, 1, duration)
        .setOnUpdate(t =>
        {
            (int i, float x) = currentState(t);
            Vector2Int previousPosition = i > 0 ? path[i - 1] : startPosition;
            transform.position = Vector3.Lerp(GridUtilities.GridToWorldPos(previousPosition), GridUtilities.GridToWorldPos(path[i]), x);
        });
    }
}
