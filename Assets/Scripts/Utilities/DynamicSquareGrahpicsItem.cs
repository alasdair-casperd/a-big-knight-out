
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A class used to define conditions under which a particular set of graphics should be displayed for square.
/// </summary>
[Serializable]
public class DynamicSquareGraphicsItem
{
    /// <summary>
    /// The prefab to be displayed if the rules are met
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// An offset to the rotation of the prefab
    /// </summary>
    public int RotationOffset;

    /// <summary>
    /// A list of adjacencies that must be matched for this tile to be displayed. True denotes an adjacent square.
    /// This array should always be of length 4.
    /// </summary>
    public bool[] Adjacencies;

    public (bool matches, int rotation) CompareAdjacencies(bool[] adjacencies)
    {
        // Check if the adjacencies match in any rotation
        for (int i = 0; i < 4; i++)
        {
            bool matches = true;
            for (int j = 0; j < 4; j++)
            {
                if (Adjacencies[j] != adjacencies[(j + i) % 4])
                {
                    matches = false;
                }
            }
            if (matches) return (true, i);
        }
        return (false, 0);
    }
}