using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Instantiates all the square prefabs for the specified level, and returns a list of these
    /// </summary>
    public Dictionary<Vector2Int, Square> BuildLevelSquares(Transform parent, Level level, float animationDuration = -1, bool ignoreErrors = false)
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
            if(square.GetType() != typeof(TrackSquare)){continue;}

            TrackSquare trackSquare = (TrackSquare)square;
            trackSquare.AdjacentTracks = new Dictionary<Vector2Int, TrackSquare>();

            // Goes through and checks all 4 directions, adding the square to adjacent tracks if it's a track
            AddAdjacentTile(squares, trackSquare, Vector2Int.up);
            AddAdjacentTile(squares, trackSquare, Vector2Int.down);
            AddAdjacentTile(squares, trackSquare, Vector2Int.left);
            AddAdjacentTile(squares, trackSquare, Vector2Int.right);
            
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
        if(squares.ContainsKey(position+direction) && squares[position+direction].GetType() == typeof(TrackSquare))
        {
            // If it is, then add it to the list of adjacent tracks.
            trackSquare.AdjacentTracks.Add(direction,(TrackSquare)squares[position+direction]);
        }
    }


    /// <summary>
    /// Instantiates all theenemy prefabs for the specified level, and returns a list of these
    /// </summary>
    public Dictionary<Vector2Int, Enemy> BuildLevelEnemies(Transform parent, Level level, float animationDuration = -1, bool ignoreErrors = false)
    {
        if (!level.IsValidLevel && !ignoreErrors)
        {
            return null;
        }

        // Find existing Enemies on the parent transform
        List<Enemy> existingEnemies = parent.GetComponentsInChildren<Enemy>().ToList();

        // Create variables
        GameObject prefab;
        Vector2Int pos;
        int initialState;
        int graphicsVariant;
        GameObject currentEnemyObject;
        Enemy currentEnemy;
        bool animateInsertion;

        // Track enemies created
        Dictionary<Vector2Int, Enemy> enemies = new();

        // Loops over all the entities in the level object received
        foreach (var (position, entity) in level.Entities)
        {
            // Remove any conflicting existing enemies and decide whether to animate the entity or not
            animateInsertion = true;
            List<Enemy> conflictingEnemies = existingEnemies.FindAll(s => s.Position == position);
            if (conflictingEnemies.Count > 0)
            {
                // Only animate insertion if the entity type has changed
                animateInsertion = entity.Type != conflictingEnemies[0].Type;

                foreach (var enemy in conflictingEnemies)
                {
                    existingEnemies.Remove(enemy);
                    Destroy(enemy.gameObject);
                }
            }

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
            enemies.Add(pos, currentEnemy);

            // Perform insertion animations
            if (animateInsertion && animationDuration > 0)
            {
                Vector3 initialScale = currentEnemyObject.transform.localScale;
                currentEnemyObject.transform.localScale = Vector3.zero;
                LeanTween.scale(currentEnemyObject, initialScale, animationDuration).setEaseOutExpo();
            }
        }

        // Remove any remaining existing enemies (i.e. squares that haven't been replaced with a new enemy))
        foreach (var existingEnemy in existingEnemies)
        {
            var g = existingEnemy.gameObject;
            Destroy(existingEnemy);
            LeanTween.scale(g, Vector3.zero, animationDuration / 2)
                .setOnComplete(() => Destroy(g));
        }

        // Return
        return enemies;
    }

    /// <summary>
    /// Instantiates all the moving platforms
    /// </summary>
    /*
    public Dictionary<Vector2Int, MovingPlatform> BuildLevelMovingPlatforms(Transform parent, Level level)
    {
        if (!level.IsValidLevel)
        {
            return null;
        }

        foreach (var (position, direction) in level.MovingPlatforms)
        {
            // Create the moving platform
            MovingPlatform movingPlatformObject = Instantiate(prefabs.movingPlatform, GridUtilities.GridToWorldPos(position), Quaternion.identity);
            movingPlatformObject.transform.parent = parent;

            // Set the moving platform's direction
            movingPlatform.Direction = direction;
        }
    }
    */
}