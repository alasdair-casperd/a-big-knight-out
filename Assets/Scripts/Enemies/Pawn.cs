using System;
using UnityEngine;

public class Pawn : Enemy
{
    /// <summary>
    /// The type of entity.
    /// </summary>
    public override EntityType Type => EntityType.Pawn;

    /// <summary>
    /// The graphics variant.
    /// </summary>
    public override int GraphicsVariant { get; set; }

    public override void OnEnemyTurn()
    {
        Vector2Int capture1 = Position + new Vector2Int(-1, -1);
        Vector2Int capture2 = Position + new Vector2Int(1, -1);

        if (PlayerController.position == capture1 || PlayerController.position == capture2)
        {
            Debug.Log("Player has been captured by a naughty pawn");
        }
        else if (PlayerController.position == Position + new Vector2Int(0, -1))
        {
            Debug.Log("Pawn blocked by the player");
        }
    }

    public override void OnLevelTurn()
    {

    }

    public override void OnPlayerLand()
    {

    }
}
