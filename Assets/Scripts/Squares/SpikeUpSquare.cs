using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A Spike square that is impassable when active. It is on a cycle,
/// decided by the state for the frequency. Starts UP.
/// </summary>
public class SpikeUpSquare : Square
{
    public override TileType Type =>  TileType.SpikeUp;

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

    private bool retracted;
    private bool graphicsRetracted;

    /// <summary>
    /// Changes the spike up and down depending on the frequency.
    /// </summary>
    public override void OnPlayerMove()
    {
        _turnCounter++;
        UpdateSpikes();
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

    public override void OnLevelTurn()
    {
        UpdateSpikes();
    }
    
    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {
        _turnCounter = 1;
        UpdateSpikes();
    }

    public override void OnChargeChanged()
    {
        Debug.Log("Update spikes");
        UpdateSpikes();
    }

    public void UpdateSpikes()
    {
        retracted = IsReceivingCharge;
        if (_turnCounter % State == 0 && State != 1)
        {
            retracted = !retracted;
        }
        
        if (graphicsRetracted != retracted)
        {
            graphicsRetracted = retracted;
            Transform targetTransform = retracted ? retractedPosition : activePosition;
            SpikeGraphics.SlideTo(targetTransform.position, -1, false);
            AudioManager.Play(AudioManager.SoundEffects.metalSwoosh);
        }
    }
}
