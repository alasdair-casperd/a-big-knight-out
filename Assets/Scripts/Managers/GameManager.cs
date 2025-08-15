using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Demo;
using System.Linq;
using System;

[RequireComponent(typeof(SquareManager))]
[RequireComponent(typeof(LevelBuilder))]
[RequireComponent(typeof(EnemyManager))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The text file storing the level data
    /// </summary>
    [SerializeField]
    private TextAsset levelFile;

    /// <summary>
    /// The player object's player controller
    /// </summary>
    [HideInInspector]
    public PlayerController player;

    /// <summary>
    /// Should the game run automatically?
    /// </summary>
    [SerializeField]
    private bool autoStart = true;

    /// <summary>
    /// The gameplay UI manager, if one is present (this will be null in the level editor, for example)
    /// </summary>
    public GameplayUIManager gameplayUIManager;

    /// <summary>
    /// Is all player input locked?
    /// </summary>
    public bool InputLocked = false;

    LevelBuilder levelBuilder;
    SquareManager squareManager;
    EnemyManager enemyManager;
    
    /// <summary>
    /// The environment prefab manager.
    /// </summary>
    public EnvironmentPrefabManager environmentPrefabManager;

    /// <summary>
    /// The level object to build and manage.
    /// </summary>
    private Level level;

    private List<MovingPlatform> movingPlatforms;

    // An empty gameObject on which to store the instantiated level objects (including the player)
    public Transform levelContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (autoStart)
        {
            level = LevelFileManager.ParseLevelFromJSON(levelFile.text);
            Initialise();
        }
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void Initialise(Level providedLevel = null)
    {
        if (providedLevel != null) level = providedLevel;

        levelBuilder = GetComponent<LevelBuilder>();
        squareManager = GetComponent<SquareManager>();
        enemyManager = GetComponent<EnemyManager>();

        player = levelBuilder.BuildPlayer(levelContainer, level);
        Dictionary<Vector2Int, Square> squares = levelBuilder.BuildLevelSquares(levelContainer, level);
        List<Enemy> enemies = levelBuilder.BuildLevelEnemies(levelContainer, level);
        movingPlatforms = levelBuilder.BuildLevelMovingPlatforms(levelContainer, level);

        // Give the square manager its squares to manage, and initialize them
        enemyManager.Initialise(player);
        squareManager.Initialise(squares, player, enemyManager);

        // Figures out the adjacent tiles for the track tiles
        foreach (var (position, square) in squares)
        {
            // Ignores the square if it's not a track
            if (square.GetType() != typeof(TrackSquare)) { continue; }

            TrackSquare trackSquare = (TrackSquare)square;

            // Tells the track square who all the platforms are (so it can check if there is a platform on top of it)
            trackSquare.Platforms = movingPlatforms;

        }

        // Initialise electricity
        squareManager.InitialiseElectricity();

        // Initialise enemies
        enemyManager.InitialiseEnemies(enemies);

        // Create the environment
        var environment = environmentPrefabManager.Environments.FirstOrDefault(x => x.LevelName == level.Name);
        if (environment != null) Instantiate(environment.prefab);

        InputLocked = false;

        // Start the player's turn
        enemyManager.OnPlayerTurnStart();
        squareManager.OnPlayerTurnStart();

    }

    // Update is called once per frame
    void Update()
    {
        // Manage the action queue
        ActionQueue.Update();

        // Listen for reset button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Restart();
        }
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void Restart()
    {
        if (player.HasMoved && !InputLocked)
        {
            InputLocked = true;
            ActionQueue.QueueAction(() =>
            {

                void action()
                {
                    Clear();
                    Initialise();
                    if (gameplayUIManager != null) gameplayUIManager.SetRestartPrompt(false);
                }

                if (gameplayUIManager != null) gameplayUIManager.FadeThroughAction(action);
                else action();
            });
        }
    }

    /// <summary>
    /// Destroy all gameObjects associated with the level (including the player)
    /// </summary>
    public void Clear()
    {
        foreach (Transform child in levelContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // The following sequences the various actions in a turn

    /// <summary>
    /// The actions to be performed once the player has input their move.
    /// This is called by the square manager once the move is input
    /// </summary>
    public void OnPlayerMove()
    {
        enemyManager.OnPlayerMove();
        squareManager.OnPlayerMove();
    }

    /// <summary>
    /// The actions to be performed once the player lands on its new tile.
    /// This is queued by the square manager to occur after the player's move is complete.
    /// </summary>
    public void OnPlayerLand()
    {
        enemyManager.OnPlayerLand();
        squareManager.OnPlayerLand();

        // Once the horse has landed and game-blocking animations have finished, initiate the level's turn.
        ActionQueue.QueueAction(OnEnemyTurn);
    }

    public void OnEnemyTurn()
    {
        enemyManager.OnEnemyTurn();
        squareManager.OnEnemyLeave();
        ActionQueue.QueueAction(OnLevelTurn);
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    public void OnLevelTurn()
    {
        enemyManager.OnLevelTurn();
        squareManager.OnLevelTurn();
        UpdatePlatforms();

        // Triggers the start of the players turn once all game-blocking animations have finished
        ActionQueue.QueueAction(OnPlayerTurnStart);
    }

    void UpdatePlatforms()
    {
        foreach (var platform in movingPlatforms)
        {
            platform.MovePlatform(squareManager.squares, player, enemyManager.enemies);
        }        
    }

    void DestroyPlatforms()
    {
        List<MovingPlatform> toDestroy = new();
        foreach (var platform in movingPlatforms)
        {
            if (platform.Exploded)
            {
                // First should check if player or any enemies are on it...
                toDestroy.Add(platform);
                if (platform.Position == player.position)
                {
                    player.Die();
                }
                List<Enemy> toKill = new();
                foreach (Enemy enemy in enemyManager.enemies)
                {
                    if (platform.Position == enemy.Position)
                    {
                        toKill.Add(enemy);
                    }
                }
                foreach (Enemy deadman in toKill)
                {
                    enemyManager.enemies.Remove(deadman);
                    Destroy(deadman.gameObject);
                }
            }
        }
        foreach (var platform in toDestroy)
        {
            movingPlatforms.Remove(platform);
            Destroy(platform.gameObject);
        }
    }


    /// <summary>
    /// The actions to be performed at the start of the player's turn.
    /// </summary>
    public void OnPlayerTurnStart()
    {
        DestroyPlatforms();
        enemyManager.OnPlayerTurnStart();
        squareManager.OnPlayerTurnStart();
    }
}
