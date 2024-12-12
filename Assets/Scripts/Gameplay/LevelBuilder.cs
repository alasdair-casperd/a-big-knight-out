using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder: MonoBehaviour
{
    /// <summary>
    /// Used to store and retrieve prefabs corresponding to each tileType 
    /// </summary>
    public TilePrefabManager tilePrefabManager;

    /// <summary>
    /// The scriptable object containing most of the game's prefabs
    /// </summary>
    public Prefabs prefabs;

    /// <summary>
    /// Instantiates all the square prefabs for the specified level, sets up their initial conditions, and
    /// optionally performs an action on each square
    /// </summary>
    public void BuildLevel(Level level, Action<Square> onSquareCreated = null)
    {
        level.ValidateLevel();

        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;

        GameObject currentSquareObject;
        Square currentSquare;

        Dictionary<Vector2Int, Square> squares = new();

        // Loops over all the tiles in the level object received
        foreach (TilePositionPair tilePosPair in level.tiles)
        {
            //Gets the prefab for the tile's type from the prefab manager
            prefab = tilePrefabManager.GetPrefab(tilePosPair.tile.type);
            pos = tilePosPair.position;

            //Reads the initial state and graphics variant for the tile
            initialState = tilePosPair.tile.initialState;
            graphicsVariant = tilePosPair.tile.graphicsVariant;

            // Creates an instance of the prefab
            currentSquareObject = Instantiate(prefab, GridUtilities.GridToWorldPos(pos), Quaternion.identity);

            // Names the square object
            currentSquareObject.gameObject.name = $"Square ({pos[0]}, {pos[1]})";

            // Gets the square component from the prefab and adds it to the list of all squares
            currentSquare = currentSquareObject.GetComponent<Square>();
            
            // Sets properties of the square
            currentSquare.Position = tilePosPair.position;
            currentSquare.validMoveIndicator = Instantiate(prefabs.validMoveIndicator, currentSquareObject.transform);
            currentSquare.validMoveIndicator.gameObject.SetActive(false);

            // Sets up the square's initial state
            if (currentSquare.IsMultiState)
            {
                currentSquare.State = initialState;
            }

            // Sets up the square's graphics variant
            currentSquare.GraphicsVariant = graphicsVariant;

            // If onSquareCreated is not null, apply it to this square
            onSquareCreated?.Invoke(currentSquare);

            // Store the square in the list of created squares
            squares.Add(pos, currentSquare);
        }

        // Does some custom error handling to make sure all the links are set up properly
        ValidateLinks(squares, level);

        // loops over all the tiles in the level
        foreach (TilePositionPair tilePositionPair in level.tiles)
        {
            // Finds the square corresponding to that tile
            currentSquare = squares[tilePositionPair.position];

            // If it's linkable loop over all its links
            if (currentSquare.IsLinkable)
            {
                // Creates a list for links to be added to
                currentSquare.Links = new List<Square>();
                foreach (Vector2Int link in tilePositionPair.tile.links)
                {
                    // Adds the square corresponding to the linked position to the square's list of links
                    currentSquare.Links.Add(squares[link]);
                }
            }
        }
    }

    /// <summary>
    /// Validate the links between squares in a coordinate-square dictionary against a those in a given level
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void ValidateLinks(Dictionary<Vector2Int, Square> squares, Level level)
    {
        Vector2Int currentPos;

        // Loops over all of the tiles
        foreach (TilePositionPair tilePosPair in level.tiles)
        {
            currentPos = tilePosPair.position;

            // Checks if the tile is trying to link to itself (bad)
            if (tilePosPair.tile.links.Contains(currentPos))
            {
                Debug.LogWarning("Trying to link a tile to itself at position " + currentPos.ToString());
            }

            // Checks if there are links registered to an unlinkable tile
            if (tilePosPair.tile.links.Count != 0 && !squares[currentPos].IsLinkable)
            {
                Debug.LogWarning("Trying to link an unlinkable tile at position " + currentPos.ToString());
            }

            // Checks if the link goes to a location with no tile created
            foreach (Vector2Int link in tilePosPair.tile.links)
            {
                if (!squares.Keys.Contains(link))
                {
                    throw new Exception("Trying to create a link to a tile that does not exist from " + currentPos.ToString() + " to " + link.ToString());
                }
            }
        }
    }
}