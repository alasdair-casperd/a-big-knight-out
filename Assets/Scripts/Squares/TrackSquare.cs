using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The new and improved track square for moving platforms.
/// </summary>
public class TrackSquare : Square
{
    public override TileType Type =>  TileType.Track;

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    // Is the square passable? Returns true if the 'actual platform' is currently at this platform square
    public override bool IsPassable
    {
        get
        {
            foreach(var platform in Platforms)
            {
                if(Position == platform.Position)
                {
                    return true;
                }
            }
            return false;
        }

        protected set
        {
            Debug.LogWarning("Trying to manually set wether a moving platform is passable or not!");
        }
    }

    // The track squares adjacent to this one, stores the relative position and the square
    public Dictionary<Vector2Int,TrackSquare> AdjacentTracks {get; set;}

    public List<MovingPlatform> Platforms {get; set;}

    public List<Vector2Int> GetPath(Vector2Int direction)
    {
        // This will get overwritten provided that there is a track adjacent to this one!
        TrackSquare nextTrack = this;
        Vector2Int nextDirection = direction;

        List<Vector2Int> path = new List<Vector2Int>();

        // Finds the two perpendicular directions
        Vector2Int perp1 = Vector2Int.RoundToInt(new Vector2(direction.x,direction.y).Perpendicular1());
        Vector2Int perp2 = Vector2Int.RoundToInt(new Vector2(direction.x,direction.y).Perpendicular2()); 

        // Runs throuhg the different options for where to go next
        if(AdjacentTracks.ContainsKey(direction))
        {
            nextTrack = AdjacentTracks[direction];
            nextDirection = direction;
        }
        // If it can turn one way but not the other, turn that way
        else if(AdjacentTracks.ContainsKey(perp1) && !AdjacentTracks.ContainsKey(perp2))
        {
            nextTrack = AdjacentTracks[perp1];
            nextDirection = perp1;
        }
        else if(AdjacentTracks.ContainsKey(perp2) && !AdjacentTracks.ContainsKey(perp1))
        {
            nextTrack = AdjacentTracks[perp2];
            nextDirection = perp2;
        }
        // if it can't go forwards, or go left or right, then bounce backwards (if possible)
        else if (AdjacentTracks.ContainsKey(-direction))
        {
            nextTrack = AdjacentTracks[-direction];
            nextDirection = -direction;
        }
        // At this point if you haven't found a path you must be isolated!
        else
        {
            Debug.LogWarning("Track found with no adjacent tracks at "+ Position.ToString());
        }

        // Checks if the next track found is a stopping point
        if(nextTrack.State == 0)
        {
            // If the next one is the stop then you've found the path
            path.Add(nextDirection);
        }
        else
        {
            // Otherwise recursively get the path from that next tile
            path = nextTrack.GetPath(nextDirection);
            // Inserts the new direction at the start (to make sure the path is in the right order)
            path.Insert(0,nextDirection);
        }

        // Safety feature to make sure we don't get stuck in an infinite recursive loop
        if(path.Count > 1000)
        {
            Debug.LogError("Path is running away!");
        }

        return path;
    }
}
