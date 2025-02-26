using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A square that can be made passable or impassable through electricity.
/// </summary>
public class BarricadeSquare : Square
{
    public override TileType Type => TileType.Spikes;

    /// <summary>
    /// The animator on the barricade graphics
    /// </summary>
    [SerializeField]
    private AnimationController barricadeGraphics;

    /// <summary>
    /// A transform indicating the position the barricade should protrude to
    /// </summary>
    [SerializeField]
    private Transform activePosition;

    /// <summary>
    /// A transform indicating the position the barricade should retract to
    /// </summary>
    [SerializeField]
    private Transform retractedPosition;

    /// <summary>
    /// Is the barricade currently closed?
    /// </summary>
    private bool closed;

    /// <summary>
    /// Have the barricade graphics retracted (or at least started the animation)?
    /// </summary>
    private bool? graphicsClosed;

    // Is passable when not closed
    public override bool IsPassable
    {
        get
        {
            return !closed;
        }
        protected set { }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {

    }

    public override void OnChargeChanged()
    {

    }

    public override void OnLevelTurn()
    {
        UpdateBarricade();
    }

    public void UpdateBarricade()
    {
        bool charged = IsReceivingCharge;
        closed = State == 1 && !charged || State != 1 && charged;

        UpdateGraphics();
    }

    public override void UpdateGraphics()
    {
        if (graphicsClosed != closed)
        {
            graphicsClosed = closed;
            Transform targetTransform = closed ? activePosition : retractedPosition;
            barricadeGraphics.SlideTo(targetTransform.position, -1, false);
            AudioManager.Play(AudioManager.SoundEffects.metalSwoosh);
        }
    }

}
