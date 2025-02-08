using UnityEngine;
using System;
using System.Collections.Generic;

public class DynamicSquareGraphics : MonoBehaviour
{
    /// <summary>
    /// The graphics items that can be displayed by this square.
    /// </summary>
    public DynamicSquareGraphicsItem[] GraphicsItems;

    /// <summary>
    /// The default graphics if no matches are found in GraphicsItems.
    /// </summary>
    public GameObject defaultGraphics;

    /// <summary>
    /// The current graphics object.
    /// </summary>
    public GameObject currentGraphics;

    public void UpdateGraphics<T>(Dictionary<Vector2Int, T> adjacencies)
    {
        bool[] adjacencyArray = new bool[4];

        adjacencyArray[0] = adjacencies.ContainsKey(Vector2Int.up);
        adjacencyArray[1] = adjacencies.ContainsKey(Vector2Int.right);
        adjacencyArray[2] = adjacencies.ContainsKey(Vector2Int.down);
        adjacencyArray[3] = adjacencies.ContainsKey(Vector2Int.left);

        UpdateGraphics(adjacencyArray);
    }

    /// <summary>
    /// Searches for an item with a matching set of adjacencies and updates the graphics accordingly.
    /// If no match is found, the default graphics are used.
    /// </summary>
    /// <param name="adjacencies"></param>
    public void UpdateGraphics(bool[] adjacencies)
    {
        if (currentGraphics != null)
        {
            Destroy(currentGraphics);
            currentGraphics = null;
        }

        bool graphicsCreated = false;
        foreach (var item in GraphicsItems)
        {
            var (matches, rotation) = item.CompareAdjacencies(adjacencies);
            if (matches)
            {
                currentGraphics = Instantiate(item.Prefab, transform.position,  Quaternion.Euler(0, 90 * (rotation + item.RotationOffset), 0) * item.Prefab.transform.rotation, transform);
                graphicsCreated = true;
            }
        }

        if (!graphicsCreated)
        {
            currentGraphics = Instantiate(defaultGraphics, transform.position, Quaternion.identity, transform);
        }
    }
}