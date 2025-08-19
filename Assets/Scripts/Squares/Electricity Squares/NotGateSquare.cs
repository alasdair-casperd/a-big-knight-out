using UnityEngine;

/// <summary>
/// A square containing a NOT logic gate
/// </summary>
public class NotGateSquare : Square
{
  public override TileType Type => TileType.NotGate;

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

  // Return true only if all incoming charges are activated
  public override bool CalculateCharge()
  {
    return !IsReceivingCharge;
  }
}
