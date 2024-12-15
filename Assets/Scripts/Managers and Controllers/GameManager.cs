using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SquareManager))]
[RequireComponent(typeof(LevelBuilder))]
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
        Dictionary<Vector2Int,Square> squares = new Dictionary<Vector2Int, Square>();
        
        levelBuilder.BuildLevel(transform, level, (square) => {
            squares.Add(square.Position, square);
            square.PlayerController = player;
        });

        // Give the square manager its squares to manage, and initialize them
        squareManager.InitialiseSquares(squares);

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
        squareManager.OnPlayerMove();
    }

    /// <summary>
    /// The actions to be performed once the player lands on its new tile.
    /// This is queued by the square manager to occur after the player's move is complete.
    /// </summary>
    public void OnPlayerLand()
    {
        squareManager.OnPlayerLand();

        // Once the horse has landed and game-blocking animations have finished, initiate the level's turn.
        ActionQueue.QueueAction(OnLevelTurn);
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    public void OnLevelTurn()
    {
        squareManager.OnLevelTurn();
        
        // Triggers the start of the players turn once all game-blocking animations have finished
        ActionQueue.QueueAction(OnPlayerTurnStart);
    }

    /// <summary>
    /// The actions to be performed at the start of the player's turn.
    /// </summary>
    public void OnPlayerTurnStart()
    {
        squareManager.OnPlayerTurnStart();
    }
}
