using System.Collections.Generic;
using UnityEngine;

public class Pawn : Enemy
{
  /// <summary>
  /// The type of entity.
  /// </summary>
  public override EntityType Type => EntityType.Pawn;



  /// <summary>
  /// The move of the pawn.
  /// </summary>
  public override Vector2Int[] EnemyMove
  {
    get
    {
      return new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), };
    }
  }

  /// <summary>
  /// The move of the pawn.
  /// </summary>
  private Vector2Int[,] CapturePairs
  {
    get
    {
      return new Vector2Int[4, 2] { { new Vector2Int(-1, 1), new Vector2Int(1, 1) }, { new Vector2Int(1, 1), new Vector2Int(1, -1) }, { new Vector2Int(1, -1), new Vector2Int(-1, -1) }, { new Vector2Int(-1, 1), new Vector2Int(-1, -1) } };
    }
  }

  /// <summary>
  /// The graphics variant.
  /// </summary>
  public override int GraphicsVariant { get; set; }

  public override void OnPlayerTurnStart()
  {
    SetCaptureSquares();
  }

  public override void OnLevelStart()
  {
    SetCaptureSquares();
  }

  public override void OnEnemyTurn()
  {
    CalculateNextSquare();
  }

  public override void OnLevelTurn()
  {

  }

  public override void OnPlayerLand()
  {

  }

  public void CalculateNextSquare()
  {
    if (CaptureSquares.Contains(PlayerController.position))
    {
      NextSquare = PlayerController.position;
      PlayerController.Die();

    }
    else if (Position + EnemyMove[Direction] != PlayerController.position
        && SquareManager.squares.ContainsKey(Position + EnemyMove[Direction])
        && SquareManager.squares[Position + EnemyMove[Direction]].IsPassable
        && !CheckSquareForEnemy(Position + EnemyMove[Direction]))
    {
      NextSquare = Position + EnemyMove[Direction];
    }
    else
    {
      NextSquare = Position;
    }
  }

  public override void SetCaptureSquares()
  {
    CaptureSquares = new List<Vector2Int>
        {
            Position + CapturePairs[Direction, 0],
            Position + CapturePairs[Direction, 1]
        };
  }
}
