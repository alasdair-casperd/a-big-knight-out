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

        if (CheckNextSquareCapture(Position, Direction) || CheckNextSquareCapture(Position, oppositeDirection))
        {
            NextSquare = PlayerController.position;
            PlayerController.Die();
        }
        else if (SquareManager.squares.ContainsKey(Position + EnemyMove[Direction])
            && SquareManager.squares[Position + EnemyMove[Direction]].IsPassable)
        {
            NextSquare = Position + EnemyMove[Direction];
        }
        else if (SquareManager.squares.ContainsKey(Position + EnemyMove[oppositeDirection])
            && SquareManager.squares[Position + EnemyMove[oppositeDirection]].IsPassable)
        {
            Direction = oppositeDirection;
            NextSquare = Position + EnemyMove[Direction];
        }
        else
        {
            NextSquare = Position;
        }

    }

    public bool CheckNextSquareCapture(Vector2Int startSquare, int captureDirection)
    {
        if (SquareManager.squares.ContainsKey(startSquare + EnemyMove[captureDirection])
            && SquareManager.squares[startSquare + EnemyMove[captureDirection]].IsPassable)
        {
            if (startSquare + EnemyMove[captureDirection] == PlayerController.position)
            {
                return true;
            }
            else
            {
                return CheckNextSquareCapture(startSquare + EnemyMove[captureDirection], captureDirection);
            }
        }
        return false;
    }
}
