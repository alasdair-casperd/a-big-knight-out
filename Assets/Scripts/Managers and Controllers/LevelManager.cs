using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelBuilder))]
public class LevelManager : MonoBehaviour
{
    // Inspector references
    public TilePrefabManager TilePrefabManager;
    public EntityPrefabManager EntityPrefabManager;

    [Header("Animation")]
    public float insertionDuration = 0.5f;
    public float deletionDuration = 0.5f;

    // ---
    private Level level;

    private PlayerController player;
    private Dictionary<Vector2Int, Square> squares = new();
    private Dictionary<Vector2Int, Enemy> enemies = new();
    private List<MovingPlatform> movingPlatforms = new();

    public void LoadLevel(Level level)
    {
        this.level = level;
        RebuildLevel();
    }

    /// <summary>
    /// Rebuild the level from scratch
    /// </summary>
    private void RebuildLevel()
    {
        ClearLevel();
        LevelBuilder levelBuilder = GetComponent<LevelBuilder>();
        squares = levelBuilder.BuildLevelSquares(transform, level);
        enemies = levelBuilder.BuildLevelEnemies(transform, level);
    }

    /// <summary>
    /// Destroy all created gameObjects related to teh level
    /// </summary>
    private void ClearLevel()
    {
        // Destroy the gameObjects
        if (player != null) Destroy(player.gameObject);
        foreach (var (_, enemy) in enemies) Destroy(enemy.gameObject);
        foreach (var (_, square) in squares) Destroy(square.gameObject);
        foreach (var movingPlatform in movingPlatforms) Destroy(movingPlatform);

        // Clear references
        player = null;
        enemies.Clear();
        squares.Clear();
        movingPlatforms.Clear();
    }

    /// <summary>
    /// Add a tile to the level
    /// </summary>
    /// <param name="withReplacement">Should any existing tile at the specified location be overridden?</param>
    public void AddTile(Vector2Int position, TileType type, bool withReplacement = true)
    {
        if (squares.ContainsKey(position))
        {
            // Cancel if not replacing tiles
            if (!withReplacement) return;

            // Delete existing square
            Destroy(squares[position].gameObject);
            squares.Remove(position);
        }

        // Create new square
        Square newSquare = Instantiate(TilePrefabManager.GetPrefab(type)).GetComponent<Square>();
        newSquare.transform.position = GridUtilities.GridToWorldPos(position);
        if (type.IsMultiState) newSquare.State = type.ValidStates[0];
        newSquare.IncomingCharges = new Dictionary<Square, bool?>();
        newSquare.UpdateGraphics();
        squares.Add(position, newSquare);

        // Update the level
        level.Tiles[position] = new Tile(type);

        // Animate
        Vector3 initialScale = newSquare.transform.localScale;
        newSquare.transform.localScale = Vector3.zero;
        LeanTween.scale(newSquare.gameObject, initialScale, insertionDuration).setEaseOutExpo();
    }
    
    public void DeleteTile(Vector2Int position)
    {
        if (squares.ContainsKey(position))
        {
            // Animate removal of the square
            Square targetSquare = squares[position];
            LeanTween.scale(targetSquare.gameObject, Vector3.zero, deletionDuration).setEaseOutExpo()
            .setOnComplete(() => {
                squares.Remove(position);
                Destroy(squares[position].gameObject);
            });
        }

        // Update the level
        if (level.Tiles.ContainsKey(position))
        {
            // Remove the tile
            level.Tiles.Remove(position);
        }
    }

    public void IncrementState(Vector2Int position)
    {
        // Update level
        if (!level.Tiles.ContainsKey(position)) return;
        Tile targetTile = level.Tiles[position];
        if (!targetTile.Type.IsMultiState) return;
        targetTile.IncrementInitialState();
        level.Tiles[position] = targetTile;
        
        // Update square
        Square targetSquare = squares[position];
        targetSquare.State = targetTile.InitialState;
        targetSquare.UpdateGraphics();
    }

    public void MoveTile(Vector2Int position, Vector2Int to)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceEntity(Vector2Int position, EntityType type)
    {
        throw new System.NotImplementedException();
    }

    public void PlacePlayer(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }

    public void RotateEntity(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }
}