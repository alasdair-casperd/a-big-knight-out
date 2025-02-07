using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelBuilder : MonoBehaviour
{
    /// <summary>
    /// Used to store and retrieve prefabs corresponding to each tileType 
    /// </summary>
    public TilePrefabManager tilePrefabManager;

    /// <summary>
    /// Used to store and retrieve prefabs corresponding to each entityType
    /// </summary>
    public EntityPrefabManager entityPrefabManager;

    /// <summary>
    /// The scriptable object containing most of the game's prefabs
    /// </summary>
    public Prefabs prefabs;

    public PlayerController BuildPlayer(Transform parent, Level level)
    {
        if (!level.IsValidLevel)
        {
            return null;
        }

        // Instantiate the player
        PlayerController player = Instantiate(prefabs.Player, parent);

        // Position the player
        player.SetInitialPosition(level.StartPosition);

        return player;
    }

    /// <summary>
    /// Instantiates all the square prefabs for the specified level, and returns a list of these
    /// </summary>
    public Dictionary<Vector2Int, Square> BuildLevelSquares(Transform parent, Level level)
    {
        if (!level.IsValidLevel)
        {
            return null;
        }

        // Create variables
        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;
        GameObject currentSquareObject;
        Square currentSquare;

        // Track squares created
        Dictionary<Vector2Int, Square> squares = new();

        // Loops over all the tiles in the level object received
        foreach (var (position, tile) in level.Tiles)
        {
            // Gets the prefab for the tile's type from the prefab manager
            prefab = tilePrefabManager.GetPrefab(tile.Type);
            pos = position;

            // Reads the initial state and graphics variant for the tile
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
            currentSquare.IncomingCharges = new Dictionary<Square, bool?>();

            // Sets up the square's initial state
            if (currentSquare.Type.IsMultiState)
            {
                currentSquare.State = initialState;
            }

            // Sets up the square's graphics variant
            currentSquare.GraphicsVariant = graphicsVariant;

            // Store the square in the list of created squares
            squares.Add(pos, currentSquare);
        }

        // Create links and incoming charge placeholders
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
                    // Adds the square corresponding to the linked position to the square's list of links and sets up incoming charges
                    if (squares.ContainsKey(link))
                    {
                        currentSquare.Links.Add(squares[link]);
                        squares[link].IncomingCharges.Add(currentSquare, null);
                    }
                }
            }
        }

        // Figures out the adjacent tiles for the track tiles
        foreach (var (position, square) in squares)
        {
            // Ignores the square if it's not a track
            if (square.GetType() != typeof(TrackSquare)) { continue; }

            TrackSquare trackSquare = (TrackSquare)square;
            trackSquare.AdjacentTracks = new Dictionary<Vector2Int, TrackSquare>();

            // Goes through and checks all 4 directions, adding the square to adjacent tracks if it's a track
            AddAdjacentTile(squares, trackSquare, Vector2Int.up);
            AddAdjacentTile(squares, trackSquare, Vector2Int.down);
            AddAdjacentTile(squares, trackSquare, Vector2Int.left);
            AddAdjacentTile(squares, trackSquare, Vector2Int.right);

        }

        // Initialise square graphics
        foreach (var (_, square) in squares)
        {
            square.UpdateGraphics();
        }

        // Return
        return squares;
    }


    /// <summary>
    /// Tests if the square in a given direction from a track square is a track square, and if so adds it to the list of adjacent tracks.
    /// </summary>
    /// <param name="squares">The list of all squares</param>
    /// <param name="trackSquare">The tracksquare to add the adjacent tracks to</param>
    /// <param name="direction">The direction to check for an adjacent track square</param>
    public void AddAdjacentTile(Dictionary<Vector2Int, Square> squares, TrackSquare trackSquare, Vector2Int direction)
    {
        // Finds the position of the track square
        Vector2Int position = trackSquare.Position;

        // Checks if there is a square in the specified direction, and if it's a track square
        if (squares.ContainsKey(position + direction) && squares[position + direction].GetType() == typeof(TrackSquare))
        {
            // If it is, then add it to the list of adjacent tracks.
            trackSquare.AdjacentTracks.Add(direction, (TrackSquare)squares[position + direction]);
        }
    }


    /// <summary>
    /// Instantiates all the enemy prefabs for the specified level, and returns a list of these
    /// </summary>
    public List<Enemy> BuildLevelEnemies(Transform parent, Level level)
    {
        if (!level.IsValidLevel)
        {
            return null;
        }

        // Create variables
        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;
        GameObject currentEnemyObject;
        Enemy currentEnemy;

        // Track enemies created
        List<Enemy> enemies = new();

        // Loops over all the entities in the level object received
        foreach (var (position, entity) in level.Entities)
        {
            //Gets the prefab for the entity's type from the prefab manager
            prefab = entityPrefabManager.GetPrefab(entity.Type);
            pos = position;

            //Reads the initial state and graphics variant for the tile
            initialState = entity.InitialState;
            graphicsVariant = entity.GraphicsVariant;

            // Creates an instance of the prefab
            currentEnemyObject = Instantiate(prefab, GridUtilities.GridToWorldPos(pos), Quaternion.identity);
            currentEnemyObject.transform.parent = parent;

            // Names the Enemy object
            currentEnemyObject.gameObject.name = $"Enemy ({pos[0]}, {pos[1]})";

            // Gets the enemy component from the prefab and adds it to the list of all enemies
            currentEnemy = currentEnemyObject.GetComponent<Enemy>();

            // Sets properties of the enemy
            currentEnemy.Position = position;

            // Sets up the enemies's initial state
            if (currentEnemy.Type.IsMultiState)
            {
                currentEnemy.State = initialState;
            }

            // Sets up the enemies's graphics variant
            currentEnemy.GraphicsVariant = graphicsVariant;

            // Store the enemy in the list of created enemies
            enemies.Add(currentEnemy);
        }

        // Return
        return enemies;
    }

    /// <summary>
    /// Instantiates all the moving platforms
    /// </summary>

    public List<MovingPlatform> BuildLevelMovingPlatforms(Transform parent, Level level)
    {
        if (!level.IsValidLevel)
        {
            return null;
        }

        // Creates the variables
        GameObject movingPlatformObject;
        MovingPlatform movingPlatform;
        List<MovingPlatform> movingPlatforms = new();

        foreach (var (position, direction) in level.MovingPlatforms)
        {
            // Create the moving platform
            movingPlatformObject = Instantiate(prefabs.movingPlatform, GridUtilities.GridToWorldPos(position), Quaternion.identity);
            movingPlatformObject.transform.parent = parent;

            // Finds the moving platform component
            movingPlatform = movingPlatformObject.GetComponent<MovingPlatform>();

            // Set the moving platform's direction and position
            movingPlatform.Initialise(position, direction);
            movingPlatforms.Add(movingPlatform);
        }

        return movingPlatforms;
    }

}