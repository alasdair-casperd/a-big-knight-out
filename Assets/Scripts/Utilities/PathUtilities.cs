using System.Collections.Generic;
using UnityEngine;

class PathUtilities
{
    /// <summary>
    /// Converts a path consisting of relative directions to a path consisting of absolute positions.
    /// </summary>
    public static List<Vector2Int> AbsolutePathFromRelativePath(List<Vector2Int> directions, Vector2Int startPosition)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = startPosition;
        foreach (var direction in directions)
        {
            current += direction;
            path.Add(current);
        }
        return path;
    }
}