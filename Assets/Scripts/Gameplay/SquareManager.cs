using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A manager to handle the top level interaction with all of the levels squares.
/// </summary>
public class SquareManager : MonoBehaviour
{
    /// <summary>
    /// The level object to build and manage.
    /// </summary>
    public  Level level;

    /// <summary>
    /// Tells the square manager how to convert the level's data into gameobjects
    /// </summary>
    public TilePrefabManager tilePrefabManager;

    /// <summary>
    /// A dictionary to find the square object at any given position
    /// </summary>
    Dictionary<Vector2Int,Square> squares;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        squares = new Dictionary<Vector2Int, Square>();
        BuildLevel();
    }

    /// <summary>
    /// Instantiates all the square prefabs in the level and sets up their initial conditions
    /// </summary>
    void BuildLevel()
    {
        level.ValidateLevel();

        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;

        GameObject currentSquareObject;
        Square currentSquare;

        // Loops over all the tiles in the level object recieved
        foreach(TilePositionPair tilePosPair in level.tiles)
        {
            //Gets the prefab for the tile's type from the prefab manager
            prefab = tilePrefabManager.GetPrefab(tilePosPair.tile.type);
            pos = tilePosPair.position;

            //Reads the initial state and graphics variant for the tile
            initialState = tilePosPair.tile.initialState;
            graphicsVariant = tilePosPair.tile.graphicsVariant;

            // Creates an instance of the prefab
            currentSquareObject = Instantiate(prefab, GridToWorldPos(pos), Quaternion.identity);
            // Gets the square component from the prefab and adds it to the list of all squares
            currentSquare = currentSquareObject.GetComponent<Square>();
            squares.Add(pos,currentSquare);

            // Sets up the square's initial state
            if(currentSquare.IsMultiState)
            {
                currentSquare.State = initialState;
            }
            // Sets up the square's graphics variant
            currentSquare.GraphicsVariant = graphicsVariant;
        }
        
        // Does some custom error handling to make sure all the links are set up properly
        ValidateLinks();

        // loops over all the tiles in the level
        foreach(TilePositionPair tilePositionPair in level.tiles)
        {
            // Finds the square corresponding to that tile
            currentSquare = squares[tilePositionPair.position];

            // If it's linkable loop over all its links
            if(currentSquare.IsLinkable)
            {
                foreach(Vector2Int link in tilePositionPair.tile.links)
                {
                    // Adds the square corresponding to the linked position to the square's list of links.
                    currentSquare.Links.Add(squares[link]);
                }
            }     
        }
    }

    /// <summary>
    /// Checks that all of the links are set up properly
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void ValidateLinks()
    {
        Vector2Int currentPos;

        // Loops over all of the tiles
        foreach(TilePositionPair tilePosPair in level.tiles)
        {
            currentPos = tilePosPair.position;
            // Checks if the tile is trying to link to itself (bad)
            if(tilePosPair.tile.links.Contains(currentPos))
            {
                Debug.LogWarning("Trying to link a tile to itself at position "+currentPos.ToString());
            }
            // Checks if there are links registered to an unlinkable tile
            if(tilePosPair.tile.links.Count!=0 && !squares[currentPos].IsLinkable)
            {
                Debug.LogWarning("Trying to link an unlinkable tile at position "+ currentPos.ToString());
            }
            // Checks if the link goes to a location with no tile created
            foreach(Vector2Int link in tilePosPair.tile.links)
            {
                if(!squares.Keys.Contains(link))
                {
                    throw new Exception("Trying to create a link to a tile that does not exist from "+currentPos.ToString()+" to "+link.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Converts the grid position to a world position
    /// </summary>
    /// <param name="gridPos">The position in grid coordinates</param>
    /// <returns>The position in world coordinates</returns>
    public Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        // To do: do something more clever and flexible here
        return new Vector3(gridPos.x, 0 , gridPos.y);
    }
}
