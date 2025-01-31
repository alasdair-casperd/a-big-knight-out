using UnityEngine;

public class Pawn : Enemy
{
  public override EntityType Type => EntityType.Pawn;
  public override int GraphicsVariant { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
