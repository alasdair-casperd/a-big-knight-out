using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public PlayerController player;

    SquareManager squareManager;
    LevelBuilder levelBuilder;

    EnemyManager enemyManager;

    /// <summary>
    /// A levelBuilder instance used to create the squares
    /// </summary>
    private LevelBuilder LevelBuilder;

    /// <summary>
    /// The level object to build and manage.
    /// </summary>
    private Level level;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        squareManager = GetComponent<SquareManager>();
        enemyManager = GetComponent<EnemyManager>();
        levelBuilder = GetComponent<LevelBuilder>();

        // Override the selected level if transitioning directly from the level editor
        // This should be removed when we add proper level management
        var receivedLevel = LevelEditor.LevelToPreview;
        if (receivedLevel != null)
        {
            level = receivedLevel;
        }

        // Generate the level from the JSON file provided
        else
        {
            level = LevelFileManager.ParseLevelFromJSON(levelFile.text);
        }

        // Position the player
        player.SetInitialPosition(level.StartPosition);


        // Build the level
        Dictionary<Vector2Int, Square> squares = levelBuilder.BuildLevelSquares(transform, level);

        Dictionary<Vector2Int, Enemy> enemies = levelBuilder.BuildLevelEnemies(transform, level);

        // Give the square manager its squares to manage, and initialize them
        squareManager.InitialiseSquares(squares);

        // Initialise electricity
        squareManager.InitialiseElectricity();

        enemyManager.InitialiseEnemies(enemies);

        // Start the player's turn
        squareManager.OnPlayerTurnStart();
    }

    // Update is called once per frame
    void Update()
    {
        ActionQueue.Update();
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
        ActionQueue.QueueAction(OnLevelTurn);
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    public void OnLevelTurn()
    {
        enemyManager.OnLevelTurn();
        squareManager.OnLevelTurn();

        // Triggers the start of the players turn once all game-blocking animations have finished
        ActionQueue.QueueAction(OnPlayerTurnStart);
    }

    /// <summary>
    /// The actions to be performed at the start of the player's turn.
    /// </summary>
    public void OnPlayerTurnStart()
    {
        enemyManager.OnPlayerTurnStart();
        squareManager.OnPlayerTurnStart();
    }

    /// <summary>
    /// This is a *very temporary* method for restarting the level player, to be replaced later
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
