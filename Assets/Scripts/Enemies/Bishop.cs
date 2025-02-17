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

    }
}
