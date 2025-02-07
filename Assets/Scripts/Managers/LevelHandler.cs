using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A class to perform updates to a level with animations.
/// When an update is performed: (1) The level is immediately updated; (2) existing gameObjects are animated appropriately; (3) the level is fully regenerated via the usual LevelBuilder.
/// </summary>
[RequireComponent(typeof(LevelBuilder))]
public class LevelHandler : MonoBehaviour
{
    // Inspector references
    public TilePrefabManager TilePrefabManager;
    public EntityPrefabManager EntityPrefabManager;
    public Prefabs Prefabs;

    [Header("Animation Durations")]
    public float insertionDuration = 0.5f;
    public float deletionDuration = 0.5f;

    // The current level being edited
    public Level level { get; private set; }

    // References to current instantiated gameObjects
    private PlayerController player;
    private Dictionary<Vector2Int, Square> squares = new();
    private List<Enemy> enemies = new();
    private List<MovingPlatform> movingPlatforms = new();

    // References to current instantiated gameObjects which should be deleted
    // when the level is next rebuilt
    private List<Square> temporarySquares = new();
    private List<Enemy> temporaryEnemies = new();
    private List<MovingPlatform> temporaryMovingPlatforms = new();

    /// <summary>
    /// Load a level into the level handler
    /// </summary>
    /// <param name="level"></param>
    public void LoadLevel(Level level)
    {
        this.level = level;
        RegenerateLevel();
    }

    /// <summary>
    /// Rebuild the level from scratch
    /// </summary>
    public void RegenerateLevel()
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
    public void ClearLevel()
    {
        // Destroy the gameObjects
        if (player != null) Destroy(player.gameObject);

        foreach (var enemy in enemies) if (enemy != null) Destroy(enemy.gameObject);
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
        if (level.Entities.ContainsKey(position)) level.Entities.Remove(position);

        // Animate away any existing square
        if (squares.ContainsKey(position))
        {
            Square existingSquare = squares[position];
            Vector3 initialScale = existingSquare.transform.localScale;
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                .setOnUpdate((t) => existingSquare.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
        }

        foreach (Enemy enemy in enemies)
        {
            // Animate away any existing enemy
            if (enemy.Position == position)
            {
                Enemy existingEnemy = enemy;
                temporaryEnemies.Add(existingEnemy);
                enemies.Remove(enemy);
                Vector3 initialScale = existingEnemy.transform.localScale;
                LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                    .setOnUpdate((t) => existingEnemy.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
            }
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

    /// <summary>
    /// Delete a tile from the level
    /// </summary>
    /// <param name="position"></param>
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

    /// <summary>
    /// Increment the state of a multi-state tile
    /// </summary>
    /// <param name="position"></param>
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

    /// <summary>
    /// Move a tile to a new position
    /// </summary>
    public void MoveTile(Vector2Int position, Vector2Int to)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Place an entity in the level
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    public void PlaceEntity(Vector2Int position, EntityType type)
    {
        // Update level
        if (level.Entities.ContainsKey(position) && level.Entities[position].Type == type) return;
        if (!level.Tiles.ContainsKey(position)) return;
        if (!level.Tiles[position].Type.IsValidStartPosition) return;
        level.Entities[position] = new Entity(type);

        foreach (Enemy potentialEnemy in enemies)
        {
            // Animate away any existing enemy
            if (potentialEnemy.Position == position)
            {
                Enemy existingEnemy = potentialEnemy;
                temporaryEnemies.Add(existingEnemy);
                enemies.Remove(potentialEnemy);
                Vector3 initialScale = existingEnemy.transform.localScale;
                LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                    .setOnUpdate((t) => existingEnemy.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
            }
        }

        // Animate
        Enemy enemy = Instantiate(EntityPrefabManager.GetPrefab(type)).GetComponent<Enemy>();
        temporaryEnemies.Add(enemy);
        Vector3 targetScale = enemy.transform.localScale;
        LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
            .setOnUpdate((t) =>
            {
                enemy.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
                enemy.transform.position = GridUtilities.GridToWorldPos(position) + Vector3.up * (1 - t);
            }).setEaseOutExpo();

        ActionQueue.QueueAction(RegenerateLevel);
    }

    /// <summary>
    /// Delete an entity from the level
    /// </summary>
    /// <param name="position"></param>
    public void DeleteEntity(Vector2Int position)
    {
        // Update level
        if (!level.Entities.ContainsKey(position)) return;
        level.Entities.Remove(position);

        foreach (Enemy potEnemy in enemies)
        {
            if (potEnemy.Position == position)
            {

                temporaryEnemies.Add(potEnemy);
                enemies.Remove(potEnemy);

                Vector3 initialScale = potEnemy.transform.localScale;
                LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                    .setOnUpdate((t) =>
                    {
                        potEnemy.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
                        potEnemy.transform.position = GridUtilities.GridToWorldPos(position) + Vector3.up * (t);
                    }).setEaseOutExpo();
                break;
            }
        }


        ActionQueue.QueueAction(RegenerateLevel);
    }

    /// <summary>
    /// Rotate an entity
    /// </summary>
    /// <param name="position"></param>
    public void RotateEntity(Vector2Int position)
    {
        if (!level.Entities.ContainsKey(position)) return;
        Entity targetEntity = level.Entities[position];
        targetEntity.Direction = (targetEntity.Direction + 1) % 4;
        level.Entities[position] = targetEntity;
    }

    /// <summary>
    /// Place a moving platform in the level
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
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
                movingPlatform.transform.position = GridUtilities.GridToWorldPos(position) + Vector3.up * (1 - t);
            }).setEaseOutExpo();

        ActionQueue.QueueAction(RegenerateLevel);
    }

    /// <summary>
    /// Delete a moving platform from the level
    /// </summary>
    /// <param name="position"></param>
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

    /// <summary>
    /// Rotate a moving platform
    /// </summary>
    /// <param name="position"></param>
    public void RotateMovingPlatform(Vector2Int position)
    {
        if (!level.MovingPlatforms.ContainsKey(position)) return;
        level.MovingPlatforms[position] = (level.MovingPlatforms[position] + 1) % 4;
    }

    /// <summary>
    /// Place the player at a given position
    /// </summary>
    /// <param name="position"></param>
    public void PlacePlayer(Vector2Int position)
    {
        // Update level
        if (!level.Tiles.ContainsKey(position)) return;
        if (!level.Tiles[position].Type.IsValidStartPosition) return;
        level.StartPosition = position;
        if (level.Entities.ContainsKey(position)) level.Entities.Remove(position);

        foreach (Enemy potentialEnemy in enemies)
        {
            // Animate away any existing enemy
            if (potentialEnemy.Position == position)
            {
                Enemy existingEnemy = potentialEnemy;
                temporaryEnemies.Add(existingEnemy);
                enemies.Remove(potentialEnemy);
                Vector3 initialScale = existingEnemy.transform.localScale;
                LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, 0, 1, insertionDuration)
                    .setOnUpdate((t) => existingEnemy.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t)).setEaseOutExpo();
            }
        }

        // Animate
        if (player != null)
        {
            // Move player to new target position
            LeanTween.value(ActionQueue.GameBlockingAnimationsContainer, player.transform.position, GridUtilities.GridToWorldPos(position), insertionDuration)
                .setOnUpdate((Vector3 v) => player.transform.position = v).setEaseOutExpo();
        }

        ActionQueue.QueueAction(RegenerateLevel);
    }

    /// <summary>
    /// Create a link between two tiles
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
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

    private void Update()
    {
        ActionQueue.Update();
    }
}