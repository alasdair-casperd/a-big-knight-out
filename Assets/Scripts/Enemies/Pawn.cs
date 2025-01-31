using UnityEngine;

public class Pawn : Enemy
{
    public override EntityType Type => EntityType.Pawn;
    public override int GraphicsVariant { get; set; }
}
