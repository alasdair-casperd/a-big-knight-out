using System.Linq;
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
    public Vector2Int position {get; private set;}

    /// <summary>
    /// Is the player still alive?
    /// </summary>
    public bool Alive;

    private void Start()
    {
        AnimationController = GetComponent<AnimationController>();
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
        
        // TODO: Add death animation
        Debug.Log("Player dies");

        // Show the restart prompt
        GameManager.SetRestartPrompt(true);
    }
}
