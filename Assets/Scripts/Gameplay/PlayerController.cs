using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// The square manager, to allow the horse to broadcast its position to the square manager
    /// if it has been moved.
    /// </summary> 
    public SquareManager SquareManager;

    /// <summary>
    /// Moves the player to a given position.
    /// </summary>
    /// <param name="position"> The world position to move the player to </param>
    public void MoveTo(Vector3 position)
    {
        transform.position = position;

        // Once the movement has been performed, the knight proudly reports to its manager that it has landed.
        SquareManager.OnPlayerLand();
    }

    /// <summary>
    /// Moves the horse given specific grid coordinates. To be used by squares.
    /// </summary>
    /// <param name="position">The place where the horse is going.</param>
    public void MoveToVector2(Vector2Int position)
    {

        Vector3 endPosition = SquareManager.GridToWorldPos(position);
        transform.position = endPosition;
        SquareManager.PlayerPos = position;
    }
}
