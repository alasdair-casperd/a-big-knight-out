using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A Spike square that is impassable when active. It is on a cycle,
/// decided by the state for the frequency. Starts UP.
/// </summary>
public class SpikeUpSquare : Square
{
    public override TileType Type
    {
        get { return TileType.SpikeUp; }
    }

    public override List<Square> Links
    {
        get
        {
            Debug.LogWarning("This type of spike cannot be linked.");
            return new List<Square>();
        }
        set
        {
            Debug.LogWarning("Not meant to set this");
        }
    }

    /// <summary>
    /// The animator on the graphics gameObject representing the retracting spikes for this tile
    /// </summary>
    [SerializeField]
    private AnimationController SpikeGraphics;

    /// <summary>
    /// A transform indicating the position the spikes should protrude to
    /// </summary>
    [SerializeField]
    private Transform activePosition;

    /// <summary>
    /// A transform indicating the position the spikes should retract to
    /// </summary>
    [SerializeField]
    private Transform retractedPosition;


    // Is always passable but spikes will kill you.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set { }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// Keeps track of what turn it is on.
    /// </summary>
    private int _turnCounter { get; set; }



    /// <summary>
    /// Changes the spike up and down depending on the frquency.
    /// </summary>
    public override void OnPlayerMove()
    {
        _turnCounter++;
        if (_turnCounter % State == 0)
        {
            ApplyVisuals();

            // Play a sound effect
            AudioManager.Play(AudioManager.SoundEffects.metalSwoosh);
        }
        else if (_turnCounter % State == 1)
        {
            ApplyVisuals();
        }
    }

    /// <summary>
    /// Just to check whether the player has landed on a spike
    /// tile and deserves to die.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);
        // Check for death
        if (_turnCounter % State == 0)
        {
            Debug.Log("You have survived... for now");

        }
        else
        {
            Debug.Log("Player Dies");
            AudioManager.Play(AudioManager.SoundEffects.ouch);
        }

    }


    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {
        _turnCounter = 1;
        SpikeGraphics.transform.position = activePosition.transform.position;
        ApplyVisuals();
    }

    /// <summary>
    /// Increases the turn counter by one.
    /// </summary>
    public override void OnLevelTurn()
    {
        //
    }

    /// <summary>
    /// Apply the visual positioning of the spike graphics
    /// </summary>
    public void ApplyVisuals()
    {
        Transform targetTransform = _turnCounter % State == 0 ? retractedPosition : activePosition;
        SpikeGraphics.SlideTo(targetTransform.position, -1, false);
    }
}
