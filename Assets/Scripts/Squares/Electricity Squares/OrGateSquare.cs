using UnityEngine;

/// <summary>
/// A square containing an OR logic gate
/// </summary>
public class OrGateSquare : Square
{
  public override TileType Type => TileType.OrGate;

  // Will always report as impassable, if you try to change that you get a warning.
  public override bool IsPassable
  {
    get
    {
      return false;
    }
    protected set
    {
      Debug.LogWarning("Trying to change if button is passable!");
    }
  }

  // Sets up the property for graphics variant
  public override int GraphicsVariant { get; set; }

  // Return true if at least one incoming charge is activated
  public override bool CalculateCharge()
  {
    return IsReceivingCharge;
  }
}
