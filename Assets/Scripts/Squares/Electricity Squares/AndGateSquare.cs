using UnityEngine;

/// <summary>
/// A square containing an AND logic gate
/// </summary>
public class AndGateSquare : Square
{
  public override TileType Type => TileType.AndGate;

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
    foreach (var (_, value) in IncomingCharges)
    {
      if (!(value ?? false))
      {
        return false;
      }
    }
    return IncomingCharges.Keys.Count > 0;
  }
}
