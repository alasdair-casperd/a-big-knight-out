using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// The current game manager
    /// </summary>
    public GameManager GameManager;

    /// <summary>
    /// The current square manager
    /// </summary> 
    public SquareManager SquareManager;

    /// <summary>
    /// The attached animation controller
    /// </summary> 
    private AnimationController AnimationController;

    /// <summary>
    /// The player's current grid position
    /// </summary>
    public Vector2Int position { get; private set; }

    /// <summary>
    /// Is the player still alive?
    /// </summary>
    public bool Alive;

    /// <summary>
    /// Has the player moved since starting the level?
    /// </summary>
    public bool HasMoved;

    /// <summary>
    /// The knight graphics for the player.
    /// </summary>
    [SerializeField] private GameObject graphics;

    /// <summary>
    /// Rubble to show after the player has died.
    /// </summary>
    [SerializeField] private GameObject rubble;

    private void Start()
    {
        AnimationController = GetComponent<AnimationController>();

        // Hide the rubble shown on death
        rubble.SetActive(false);
    }

    public void SetInitialPosition(Vector2Int startPos)
    {
        position = startPos;
        transform.position = GridUtilities.GridToWorldPos(position);
    }

    /// <summary>
    /// Move the player to a given position with the specified animation style
    /// </summary>
    /// <param name="endPosition"> The world position to move the player to </param>
    public void MoveTo(Vector2Int to, AnimationController.MovementType movementType, float duration = -1)
    {
        // Update the player controller position
        position = to;

        // Record that the player has moved
        HasMoved = true;

        // Convert grid position to world position
        Vector3 endPosition = GridUtilities.GridToWorldPos(to);

        // Animate the movement
        switch (movementType)
        {
            case AnimationController.MovementType.Jump:
                AnimationController.JumpTo(endPosition, duration);
                break;
            case AnimationController.MovementType.Slide:
                AnimationController.SlideTo(endPosition, duration);
                break;
            case AnimationController.MovementType.Teleport:
                AnimationController.TeleportTo(endPosition, duration);
                break;
        }
    }

    public void MoveAlongPath(Vector2Int[] path, float duration = -1)
    {
        // Update the player controller position
        if (path.Length > 0) position = path.Last();

        // Animate
        AnimationController.MoveAlongPath(path, duration);
    }

    /// <summary>
    /// Kill the player.
    /// </summary>
    public void Die()
    {
        Alive = false;

        // Hide the player's usual graphics
        graphics.SetActive(false);

        // Show the death animation (rubble)
        rubble.SetActive(true);

        // Show the restart prompt
        if (GameManager.gameplayUIManager != null) GameManager.gameplayUIManager.SetRestartPrompt(true);
    }
}
