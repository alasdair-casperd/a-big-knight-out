using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelBuilder))]
public class LevelAnimator : MonoBehaviour
{
    // Inspector references
    public TilePrefabManager TilePrefabManager;
    public EntityPrefabManager EntityPrefabManager;

    [Header("Animation")]
    public float insertionDuration = 0.5f;
    public float deletionDuration = 0.5f;

    // ---
    public Level level { get; private set; }

    private PlayerController player;
    private Dictionary<Vector2Int, Square> squares = new();
    private Dictionary<Vector2Int, Enemy> enemies = new();
    private List<MovingPlatform> movingPlatforms = new();

    public void LoadLevel(Level level)
    {
        this.level = level;
        RegenerateLevel();
    }

    /// <summary>
    /// Rebuild the level from scratch
    /// </summary>
    private void RegenerateLevel()
    {
        ClearLevel();
        LevelBuilder levelBuilder = GetComponent<LevelBuilder>();
        squares = levelBuilder.BuildLevelSquares(transform, level);
        enemies = levelBuilder.BuildLevelEnemies(transform, level);
        movingPlatforms = levelBuilder.BuildLevelMovingPlatforms(transform, level);
    }

    /// <summary>
    /// Destroy all created gameObjects related to the level
    /// </summary>
    private void ClearLevel()
    {
        // Destroy the gameObjects
        if (player != null) Destroy(player.gameObject);
        foreach (var (_, enemy) in enemies) if (enemy != null) Destroy(enemy.gameObject);
        foreach (var (_, square) in squares) if (square != null) Destroy(square.gameObject);
        foreach (var movingPlatform in movingPlatforms) if (movingPlatform != null) Destroy(movingPlatform);

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
        if (level.Tiles.ContainsKey(position))
        {
            // Cancel if tile type is already correct
            if (level.Tiles[position].Type == type) return;
            
            // Cancel if not replacing tiles
            if (!withReplacement) return;
        }

        if (squares.ContainsKey(position))
        {
            // Animate away the existing square
            Square existingSquare = squares[position];
            Vector3 initialScale = existingSquare.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingSquare.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }

        
        // Create new square
        Square newSquare = Instantiate(TilePrefabManager.GetPrefab(type)).GetComponent<Square>();
        squares[position] = newSquare;
        newSquare.transform.position = GridUtilities.GridToWorldPos(position);

        // Animate (scale in)
        Vector3 targetScale = newSquare.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) => newSquare.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t)).setEaseOutExpo();

        // Update the level
        level.Tiles[position] = new Tile(type);
        ActionQueue.QueueAction(RegenerateLevel);
    }
    
    public void DeleteTile(Vector2Int position)
    {
        // Cancel if there is no square to delete
        if (!squares.ContainsKey(position)) return;

        // Animate away the existing square
        Square existingSquare = squares[position];
        Vector3 initialScale = existingSquare.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) => existingSquare.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();

        // Update the level
        if (level.Tiles.ContainsKey(position))
        {
            // Remove the tile
            level.Tiles.Remove(position);
        }

        ActionQueue.QueueAction(RegenerateLevel);
    }

    public void IncrementState(Vector2Int position)
    {
        // Update level
        if (!level.Tiles.ContainsKey(position)) return;
        Tile targetTile = level.Tiles[position];
        if (!targetTile.Type.IsMultiState) return;
        targetTile.IncrementInitialState();
        level.Tiles[position] = targetTile;
        RegenerateLevel();
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

    private void Update()
    {
        ActionQueue.Update();
    }
}