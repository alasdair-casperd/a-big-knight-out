using System.Collections.Generic;
using Demo;
using UnityEngine;

public class Bishop : Enemy
{
    public override EntityType Type => EntityType.Bishop;

    /// <summary>
    /// The move of the pawn.
    /// </summary>
    public override Vector2Int[] EnemyMove
    {
        get
        {
            return new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1), };
        }
    }
    public override int GraphicsVariant { get; set; }

    public override void OnLevelStart()
    {
        SetCaptureSquares();
    }

    public override void OnPlayerTurnStart()
    {
        SetCaptureSquares();
    }

    public override void OnEnemyTurn()
    {
        CalculateNextSquare();
    }

    public void CalculateNextSquare()
    {
        int oppositeDirection;

        if (Direction + 2 < 4)
        {
            oppositeDirection = Direction + 2;
        }
        else
        {
            oppositeDirection = Direction - 2;
        }

        if (CaptureSquares.Contains(PlayerController.position))
        {
            NextSquare = PlayerController.position;
            PlayerController.Die();
        }
        else if (SquareManager.squares.ContainsKey(Position + EnemyMove[Direction])
            && SquareManager.squares[Position + EnemyMove[Direction]].IsPassable
            && !CheckSquareForEnemy(Position + EnemyMove[Direction]))
        {
            NextSquare = Position + EnemyMove[Direction];
        }
        else if (SquareManager.squares.ContainsKey(Position + EnemyMove[oppositeDirection])
            && SquareManager.squares[Position + EnemyMove[oppositeDirection]].IsPassable
            && !CheckSquareForEnemy(Position + EnemyMove[oppositeDirection]))
        {
            Direction = oppositeDirection;
            NextSquare = Position + EnemyMove[Direction];
        }
        else
        {
            NextSquare = Position;
        }

    }

    public void CheckNextSquareCapture(Vector2Int startSquare, int captureDirection)
    {
        if (SquareManager.squares.ContainsKey(startSquare + EnemyMove[captureDirection])
            && SquareManager.squares[startSquare + EnemyMove[captureDirection]].IsPassable
            && !CheckSquareForEnemy(startSquare + EnemyMove[captureDirection]))
        {
            CaptureSquares.Add(startSquare + EnemyMove[captureDirection]);
            CheckNextSquareCapture(startSquare + EnemyMove[captureDirection], captureDirection);
        }
    }

    public override void SetCaptureSquares()
    {
        CaptureSquares = new List<Vector2Int>();
        for (int i = 0; i < 4; i++)
        {
            CheckNextSquareCapture(Position, i);
        }
    }
}
