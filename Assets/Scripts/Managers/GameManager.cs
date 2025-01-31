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
    [HideInInspector]
    public PlayerController player;

    /// <summary>
    /// Should the game run automatically?
    /// </summary>
    [SerializeField]
    private bool autoStart = true;

    LevelBuilder levelBuilder;
    SquareManager squareManager;
    EnemyManager enemyManager;

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
        Dictionary<Vector2Int, Enemy> enemies = levelBuilder.BuildLevelEnemies(levelContainer, level);
        movingPlatforms  = levelBuilder.BuildLevelMovingPlatforms(levelContainer, level);

        // Give the square manager its squares to manage, and initialize them
        squareManager.Initialise(squares, player);
        enemyManager.Initialise(player);

        // Figures out the adjacent tiles for the track tiles
        foreach (var (position, square) in squares)
        {
            // Ignores the square if it's not a track
            if(square.GetType() != typeof(TrackSquare)){ continue; }

            TrackSquare trackSquare = (TrackSquare)square;

            // Tells the track square who all the platforms are (so it can check if there is a platform on top of it)
            trackSquare.Platforms = movingPlatforms;     
            
        }

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

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void Restart()
    {
        Clear();
        Initialise();
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
        ActionQueue.QueueAction(OnLevelTurn);
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    public void OnLevelTurn()
    {
        enemyManager.OnLevelTurn();
        squareManager.OnLevelTurn();

        foreach(var platform in movingPlatforms)
        {
            platform.MovePlatform(squareManager.squares,player);
        }


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
}
