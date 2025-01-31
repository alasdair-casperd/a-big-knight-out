using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LevelBuilder))]
public class LevelHandler : MonoBehaviour
{
    // Inspector references
    public TilePrefabManager TilePrefabManager;
    public EntityPrefabManager EntityPrefabManager;
    public Prefabs Prefabs;

    [Header("Animation")]
    public float insertionDuration = 0.5f;
    public float deletionDuration = 0.5f;

    // ---
    public Level level { get; private set; }

    private PlayerController player;
    private Dictionary<Vector2Int, Square> squares = new();
    private Dictionary<Vector2Int, Enemy> enemies = new();
    private List<MovingPlatform> movingPlatforms = new();

    private List<Square> temporarySquares = new();
    private List<Enemy> temporaryEnemies = new();
    private List<MovingPlatform> temporaryMovingPlatforms = new();

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
        player = levelBuilder.BuildPlayer(transform, level);
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
        foreach (var movingPlatform in movingPlatforms) if (movingPlatform != null) Destroy(movingPlatform.gameObject);

        foreach (var square in temporarySquares) if (square != null) Destroy(square.gameObject);
        foreach (var enemy in temporaryEnemies) if (enemy != null) Destroy(enemy.gameObject);
        foreach (var movingPlatform in temporaryMovingPlatforms) if (movingPlatform != null) Destroy(movingPlatform.gameObject);

        // Clear references
        player = null;
        enemies.Clear();
        squares.Clear();
        movingPlatforms.Clear();
    }

    /// <summary>
    /// Remove all links to a position
    /// </summary>
    /// <param name="position"></param>
    private void RemoveLinksToPosition(Vector2Int position)
    {
        foreach (var (_, tile) in level.Tiles)
        {
            tile.Links?.RemoveAll(link => link == position);
        }
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

        // Cancel if the player is at this position and the type is not a valid start position
        if (level.StartPosition == position && !type.IsValidStartPosition) return;

        // Update the level
        level.Tiles[position] = new Tile(type);
        if (level.MovingPlatforms.ContainsKey(position)) level.MovingPlatforms.Remove(position);

        // Animate away any existing square
        if (squares.ContainsKey(position))
        {
            Square existingSquare = squares[position];
            Vector3 initialScale = existingSquare.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingSquare.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }

        // Animate away any existing moving platform
        if (movingPlatforms.FirstOrDefault((x) => x.Position == position) is MovingPlatform existingMovingPlatform)
        {
            movingPlatforms.Remove(existingMovingPlatform);
            temporaryMovingPlatforms.Add(existingMovingPlatform);

            Vector3 initialScale = existingMovingPlatform.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingMovingPlatform.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }
        
        // Create new square
        Square newSquare = Instantiate(TilePrefabManager.GetPrefab(type)).GetComponent<Square>();
        temporarySquares.Add(newSquare);
        newSquare.transform.position = GridUtilities.GridToWorldPos(position);

        // Animate (scale in)
        Vector3 targetScale = newSquare.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) => newSquare.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t))
            .setEaseOutExpo();

        ActionQueue.QueueAction(RegenerateLevel);
    }
    
    public void DeleteTile(Vector2Int position)
    {
        // Cancel if there is no square to delete
        if (!level.Tiles.ContainsKey(position)) return;

        // Cancel if the player is at this position
        if (level.StartPosition == position) return;

        // Cancel if there is no tile to delete
        if (!level.Tiles.ContainsKey(position)) return;
        
        // Update the level
        RemoveLinksToPosition(position);
        level.Tiles.Remove(position);
        if (level.MovingPlatforms.ContainsKey(position)) level.MovingPlatforms.Remove(position);

        // Animate away the existing square
        if (squares.ContainsKey(position))
        {
            Square existingSquare = squares[position];
            squares.Remove(position);
            temporarySquares.Add(existingSquare);

            Vector3 initialScale = existingSquare.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingSquare.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }

        // Animate away any existing moving platform
        if (movingPlatforms.FirstOrDefault((x) => x.Position == position) is MovingPlatform existingMovingPlatform)
        {
            movingPlatforms.Remove(existingMovingPlatform);
            temporaryMovingPlatforms.Add(existingMovingPlatform);

            Vector3 initialScale = existingMovingPlatform.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingMovingPlatform.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
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
        
        // Animate away any existing moving platform
        if (movingPlatforms.FirstOrDefault((x) => x.Position == position) is MovingPlatform existingMovingPlatform)
        {
            movingPlatforms.Remove(existingMovingPlatform);
            temporaryMovingPlatforms.Add(existingMovingPlatform);

            Vector3 initialScale = existingMovingPlatform.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingMovingPlatform.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }

        ActionQueue.QueueAction(RegenerateLevel);

    }

    public void MoveTile(Vector2Int position, Vector2Int to)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceEntity(Vector2Int position, EntityType type)
    {
        throw new System.NotImplementedException();
    }

    public void DeleteEntity(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceMovingPlatform(Vector2Int position, int direction)
    {
        // Update level
        if (level.MovingPlatforms.ContainsKey(position)) return;
        if (!level.Tiles.ContainsKey(position)) return;
        if (level.Tiles[position].Type != TileType.Track) return;
        level.MovingPlatforms[position] = direction;

        // Animate
        MovingPlatform movingPlatform = Instantiate(Prefabs.movingPlatform).GetComponent<MovingPlatform>();
        temporaryMovingPlatforms.Add(movingPlatform);
        Vector3 targetScale = movingPlatform.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) =>
            {
                movingPlatform.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
                movingPlatform.transform.position = GridUtilities.GridToWorldPos(position) + Vector3.up * (1-t);
            }).setEaseOutExpo();

        ActionQueue.QueueAction(RegenerateLevel);
        
    }

    public void DeleteMovingPlatform(Vector2Int position)
    {
        // Update level
        if (!level.MovingPlatforms.ContainsKey(position)) return;
        level.MovingPlatforms.Remove(position);

        // Animate
        MovingPlatform movingPlatform = movingPlatforms.FirstOrDefault((x) => x.Position == position);
        if (movingPlatform == null) return;
        temporaryMovingPlatforms.Add(movingPlatform);
        movingPlatforms.Remove(movingPlatform);
        Vector3 initialScale = movingPlatform.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) =>
            {
                movingPlatform.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
                movingPlatform.transform.position = GridUtilities.GridToWorldPos(position) + Vector3.up * (t);
            }).setEaseOutExpo();

        ActionQueue.QueueAction(RegenerateLevel);
        
    }

    public void PlacePlayer(Vector2Int position)
    {
        // Update level
        if (!level.Tiles.ContainsKey(position)) return;
        if (!level.Tiles[position].Type.IsValidStartPosition) return;
        level.StartPosition = position;

        // Animate
        if (player != null)
        {
            // Move player to new target position
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, player.transform.position, GridUtilities.GridToWorldPos(position), insertionDuration)
                .setOnUpdate((Vector3 v) => player.transform.position = v).setEaseOutExpo();
        }

        ActionQueue.QueueAction(RegenerateLevel);
    }

    public void CreateLink(Vector2Int start, Vector2Int end)
    {
        // Prevent self-links
        if (start == end) return;

        // Check link starts at a tile
        if (!level.Tiles.ContainsKey(start)) return;

        // Prevent links to non-existant tiles
        if (!level.Tiles.ContainsKey(end)) return;

        // Read start and target tiles
        Tile startTile = level.Tiles[start];
        Tile targetTile = level.Tiles[end];

        // Check that the tiles are compatible
        if (!startTile.Type.ValidLinkTargets.Contains(targetTile.Type)) return;

        // Create the link
        level.Tiles[start].Links.Add(end);
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