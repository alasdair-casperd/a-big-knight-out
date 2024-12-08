using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Moves the player to a given position.
    /// </summary>
    /// <param name="position"> The world position to move the player to </param>
    /// <param name="manager"> The square manager responsible for this knight </param>
    public void MoveTo(Vector3 position, SquareManager manager)
    {
        transform.position = position;
        
        // Once the movement has been performed, the knight proudly reports to its manager that it has landed.
        manager.OnPlayerLand();
    }
}
