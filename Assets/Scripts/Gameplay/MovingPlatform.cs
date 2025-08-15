using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    
    public Vector2Int Position {get; set; }

    Vector2Int Direction {get; set; }

    public void Initialise(Vector2Int position, int direction)
    {
        Position = position;
        Direction = directions[direction];
    }

    public void MovePlatform(Dictionary<Vector2Int,Square> squares, PlayerController player, List<Enemy> enemies)
    {
        if(squares[Position].GetType() != typeof(TrackSquare))
        {
            Debug.LogError("Moving Platform has ended up on a non-track tile at position " + Position);
            return;
        }
        TrackSquare trackSquare = (TrackSquare)squares[Position];
        List<Vector2Int> path = trackSquare.GetPath(Direction);
        Vector2Int totalDisplacement = new();
        foreach(var step in path)
        {
            totalDisplacement += step;
        }
        
        // Animate the movement of the platform)
        GetComponent<AnimationController>().MoveAlongPath(PathUtilities.AbsolutePathFromRelativePath(path, Position).ToArray(), -1);
        transform.Translate(totalDisplacement.x,0,totalDisplacement.y);
        
        // If the player is on the platform, move them along the path and animate
        if(player.position == Position)
        {
            player.MoveAlongPath(PathUtilities.AbsolutePathFromRelativePath(path, Position).ToArray());
        }
        
        foreach(Enemy enemy in enemies)
        {
            if(enemy.Position == Position)
            {
                enemy.MoveAlongPath(PathUtilities.AbsolutePathFromRelativePath(path,Position).ToArray());
            }
        }
        
        Position += totalDisplacement;
        Direction = path.Last();
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<MovingPlatform>()!= null)
        {
            Explode();
        }
    }
    
    void Explode()
    {
        Debug.Log("Kaboom?");
        Destroy(gameObject);
    }
}
