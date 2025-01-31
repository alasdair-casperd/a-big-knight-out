using UnityEngine;

public class Rook : Enemy
{
    public override EntityType Type => EntityType.Rook;
    public override int GraphicsVariant { get; set; }
}
