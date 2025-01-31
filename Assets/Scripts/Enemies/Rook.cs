using UnityEngine;

public class Rook : Enemy
{
    public override EntityType Type => EntityType.Rook;
    public override int GraphicsVariant { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
