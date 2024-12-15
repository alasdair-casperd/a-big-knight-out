using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    /// Instantiates all the square prefabs for the specified level, and returns a list of these
    /// </summary>
    public Dictionary<Vector2Int, Square> BuildLevel(Transform parent, Level level, float animationDuration = -1, bool ignoreErrors = false)
    {
        if (!level.IsValidLevel && !ignoreErrors)
        {
            return null;
        }

        // Find existing squares on the parent transform
        List<Square> existingSquares = parent.GetComponentsInChildren<Square>().ToList();

        // Create variables
        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;
        GameObject currentSquareObject;
        Square currentSquare;
        bool animateInsertion;

        // Track squares created
        Dictionary<Vector2Int, Square> squares = new();

        // Loops over all the tiles in the level object received
        foreach (var (position, tile) in level.Tiles)
        {
            // Remove any conflicting existing squares and decide whether to animate the tile or not
            animateInsertion = true;
            List<Square> conflictingSquares = existingSquares.FindAll(s => s.Position == position);
            if (conflictingSquares.Count > 0)
            {
                // Only animate insertion if the tile type has changed
                animateInsertion = tile.Type != conflictingSquares[0].Type;
                
                foreach (var square in conflictingSquares)
                {
                    existingSquares.Remove(square);
                    Destroy(square.gameObject);
                }
            }

            //Gets the prefab for the tile's type from the prefab manager
            prefab = tilePrefabManager.GetPrefab(tile.Type);
            pos = position;

            //Reads the initial state and graphics variant for the tile
            initialState = tile.InitialState;
            graphicsVariant = tile.GraphicsVariant;

            // Creates an instance of the prefab
            currentSquareObject = Instantiate(prefab, GridUtilities.GridToWorldPos(pos), Quaternion.identity);
            currentSquareObject.transform.parent = parent;

            // Names the square object
            currentSquareObject.gameObject.name = $"Square ({pos[0]}, {pos[1]})";

            // Gets the square component from the prefab and adds it to the list of all squares
            currentSquare = currentSquareObject.GetComponent<Square>();
            
            // Sets properties of the square
            currentSquare.Position = position;
            currentSquare.validMoveIndicator = Instantiate(prefabs.validMoveIndicator, currentSquareObject.transform);
            currentSquare.validMoveIndicator.gameObject.SetActive(false);

            // Sets up the square's initial state
            if (currentSquare.Type.IsMultiState)
            {
                currentSquare.State = initialState;
            }

            // Sets up the square's graphics variant
            currentSquare.GraphicsVariant = graphicsVariant;

            // Store the square in the list of created squares
            squares.Add(pos, currentSquare);

            // Perform insertion animations
            if (animateInsertion && animationDuration > 0)
            {
                Vector3 initialScale = currentSquareObject.transform.localScale;
                currentSquareObject.transform.localScale = Vector3.zero;
                LeanTween.scale(currentSquareObject, initialScale, animationDuration).setEaseOutExpo();
            }
        }

        // Remove any remaining existing squares (i.e. squares that haven't been replaced with a new tile))
        foreach (var existingSquare in existingSquares)
        {
            var g = existingSquare.gameObject;
            Destroy(existingSquare);
            LeanTween.scale(g, Vector3.zero, animationDuration / 2)
                .setOnComplete(() => Destroy(g));
        }
        
        // loops over all the tiles in the level
        foreach (var (position, tile) in level.Tiles)
        {
            if (tile.Type.IsLinkable)
            {
                // Finds the square corresponding to that tile
                currentSquare = squares[position];

                // Creates a list for links to be added to
                currentSquare.Links = new List<Square>();
                foreach (Vector2Int link in tile.Links)
                {
                    // Adds the square corresponding to the linked position to the square's list of links
                    if (squares.ContainsKey(link))
                    {
                        currentSquare.Links.Add(squares[link]);
                    }
                }
            }
        }

        // Return
        return squares;
    }
}