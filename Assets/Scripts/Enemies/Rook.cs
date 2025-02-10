using UnityEngine;

public class Rook : Enemy
{
    public override EntityType Type => EntityType.Rook;

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
    public override int GraphicsVariant { get; set; }
}
