using UnityEngine;

public class Bishop : Enemy
{
    public override EntityType Type => EntityType.Bishop;
    public override int GraphicsVariant { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
