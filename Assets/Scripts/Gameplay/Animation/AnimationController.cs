using System;
using System.Collections.Generic;
using UnityEngine;

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
    /// Animates the player's movement to a given position with a jump
    /// </summary>
    /// <param name="endPosition">The world position to move the player to</param>
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
            // Moves the player
            Vector3 outputPosition = t * endPosition + (1 - t) * startPosition;
            outputPosition.y += JumpHeight * t * (1 - t) * 4;
            transform.position = outputPosition;

            // Rotates the player about the y-axis towards the end position
            Vector3 direction = endPosition - startPosition;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
        });
    }

    /// <summary>
    /// Animates teleportation via a portal to a given position
    /// </summary>
    /// <param name="endPosition">The world position to teleport the player to</param>
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
    /// <param name="endPosition"></param>
    /// <param name="duration"></param>
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
}
