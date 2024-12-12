using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// A manager to handle the top level interaction with all of the levels squares.
/// </summary>
public class SquareManager : MonoBehaviour
{
    /// <summary>
    /// The player object's player controller.
    /// </summary>
    public PlayerController player;

    /// <summary>
    /// The level object to build and manage.
    /// </summary>
    public Level level;

    /// <summary>
    /// A levelBuilder instance used to create the squares
    /// </summary>
    public LevelBuilder LevelBuilder;

    /// <summary>
    /// A dictionary to find the square object at any given position
    /// </summary>
    Dictionary<Vector2Int, Square> squares;

    /// <summary>
    /// The player's position
    /// </summary>
    public Vector2Int PlayerPos;

    bool isPlayerTurn;

    /// <summary>
    /// The valid moves a knight can make
    /// </summary>
    Vector2Int[] KnightMoves =
    {
        new Vector2Int(1,2),
        new Vector2Int(2,1),
        new Vector2Int(1,-2),
        new Vector2Int(2,-1),
        new Vector2Int(-1,2),
        new Vector2Int(-2,1),
        new Vector2Int(-1,-2),
        new Vector2Int(-2,-1)
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Position the player
        PlayerPos = level.startPos;
        player.transform.position = GridUtilities.GridToWorldPos(PlayerPos);
        
        // Build the level
        squares = new Dictionary<Vector2Int, Square>();
        LevelBuilder.BuildLevel(level, (square) => {
            squares.Add(square.Position, square);
            square.PlayerController = player;
        });

        // Initialise the squares and start gameplay
        foreach (Square square in squares.Values)
        {
            square.OnLevelStart();
        }
        OnPlayerTurnStart();
    }

    void Update()
    {
        // Handles player movement
        if (Input.GetMouseButtonDown(0) && isPlayerTurn)
        {
            Vector2Int mousePos = GridUtilities.GetMouseGridPos();
            if (GetValidMoves().Contains(mousePos))
            {
                // Moves the player
                player.MoveTo(mousePos, AnimationController.MovementType.Jump);

                // Informs the squares that the player has moved
                OnPlayerMove();

                // Handles player land once game-blocking animations have finished
                ActionQueue.QueueAction(OnPlayerLand);
            }
        }

        // Process the action queue
        ActionQueue.Update();
    }


    /// <summary>
    /// The actions to be performed at the start of the player's turn.
    /// </summary>
    void OnPlayerTurnStart()
    {
        isPlayerTurn = true;

        foreach (Square square in squares.Values)
        {
            square.OnPlayerTurnStart();
        }

        // Add square highlights to valid moves
        HighlightSquares(GetValidMoves());
    }

    /// <summary>
    /// The actions to be performed once the player has input their move
    /// </summary>
    void OnPlayerMove()
    {
        // Remove all square highlights
        HighlightSquares(new());

        isPlayerTurn = false;
        foreach (Square square in squares.Values)
        {
            square.OnPlayerMove();
        }
    }

    /// <summary>
    /// The actions to be performed once the player lands on its new tile.
    /// </summary>
    public void OnPlayerLand()
    {
        squares[PlayerPos].OnPlayerLand();

        // Once the horse has landed and game-blocking animations have finished, initiate the level's turn.
        ActionQueue.QueueAction(OnLevelTurn);
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    void OnLevelTurn()
    {
        // Does all of the square's turns.
        foreach (Square square in squares.Values)
        {
            square.OnLevelTurn();
        }

        // Triggers the start of the players turn once all game-blocking animations have finished
        ActionQueue.QueueAction(OnPlayerTurnStart);
    }

    public List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (Vector2Int position in squares.Keys)
        {
            if (KnightMoves.Contains(position - PlayerPos) && squares[position].IsPassable)
            {
                moves.Add(position);
            }
        }

        return moves;
    }

    /// <summary>
    /// Highlight squares at a given set of Vector2Int coordinates
    /// </summary>
    /// <param name="coordinatesToHighlight">The set of coordinates to highlight</param>
    public void HighlightSquares(List<Vector2Int> coordinatesToHighlight)
    {

        foreach (var (coordinate, square) in squares)
        {
            square.IndicateMoveValidity(coordinatesToHighlight.Contains(coordinate));
        }
    }
}
