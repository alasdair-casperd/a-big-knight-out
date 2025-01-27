using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Vector2Int[] directions = {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right};
    
    public Vector2Int Position {get; set; }

    Vector2Int Direction {get; set; }

    public void Initialise(Vector2Int position, int direction)
    {
        Position = position;
        Direction = directions[direction];
    }

    public void MovePlatform(Dictionary<Vector2Int,Square> squares, PlayerController player)
    {
        if(squares[Position].GetType() != typeof(TrackSquare))
        {
            Debug.LogError("Moving Platform has ended up on a non-track tile at position "+ Position);
            return;
        }
        TrackSquare trackSquare = (TrackSquare)squares[Position];
        List<Vector2Int> path = trackSquare.GetPath(Direction);
        Vector2Int totalDisplacement = new();
        foreach(var step in path)
        {
            totalDisplacement += step;
        }
        //Temporary for testing!!! Alasdair do a nice animation here pls
        transform.Translate(totalDisplacement.x,0,totalDisplacement.y);
        
        if(player.position == Position)
        {
            player.MoveTo(Position+totalDisplacement,AnimationController.MovementType.Slide);
        }
        

        Position += totalDisplacement;
        Direction = path.Last();
        // Gets the path from the square at its current position
        // Moves it along this path
        // updates its direcrtion
        // Do some validation to make sure two platforms haven't crashed???
    }

}
