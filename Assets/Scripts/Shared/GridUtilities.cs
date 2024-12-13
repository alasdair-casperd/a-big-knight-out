
using UnityEngine;

/// <summary>
/// A class storing utility functions relating to the grid layout of the levels
/// </summary>
public static class GridUtilities
{
    /// <summary>
    /// Converts the grid position to a world position
    /// </summary>
    /// <param name="gridPos">The position in grid coordinates</param>
    /// <returns>The position in world coordinates</returns>
    public static Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        // To do: do something more clever and flexible here
        return new Vector3(gridPos.x, 0, gridPos.y);
    }

    /// <summary>
    /// Converts the world position to a grid position
    /// </summary>
    /// <param name="worldPos">The position in global world coordinates</param>
    /// <returns>The position in grid coordinates</returns>
    public static Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        // To do: see GridToWorldPos()...
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
    }

    /// <summary>
    /// Gets the world position the mouse is hovering over.
    /// </summary>
    /// <returns>The grid position of the mouse</returns>
    public static Vector2Int GetMouseGridPos()
    {
        return WorldToGridPos(MouseUtilities.GetMouseWorldPos());
    }
}