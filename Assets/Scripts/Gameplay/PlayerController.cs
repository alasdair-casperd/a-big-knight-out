using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// The current square manager
    /// </summary> 
    public SquareManager SquareManager;

    /// <summary>
    /// The attached animation controller
    /// </summary> 
    private AnimationController AnimationController;

    private void Start()
    {
        AnimationController = GetComponent<AnimationController>();
    }

    /// <summary>
    /// Move the player to a given position with the specified animation style
    /// </summary>
    /// <param name="endPosition"> The world position to move the player to </param>
    public void MoveTo(Vector2Int to, AnimationController.MovementType movementType, float duration = -1)
    {
        // Update the square manager
        SquareManager.PlayerPos = to;

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
}
