using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A Spike square that is impassable when active.
/// </summary>
public class SpikeSquare : Square
{
    public override TileType Type
    {
        get { return TileType.Spike; }
    }

    public override bool IsLinkable { get { return true; } }

    public override List<Square> Links
    {
        get; set;
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

    /// <summary>
    /// Recursively finds all spike squares linked to this one.
    /// </summary>
    private List<SpikeSquare> AllLinks
    {
        get
        {
            var output = new List<SpikeSquare>();

            void Check(SpikeSquare target)
            {
                foreach (var square in target.Links)
                {
                    if (square is SpikeSquare spikeSquare)
                    {
                        if (!output.Contains(spikeSquare))
                        {
                            output.Add(spikeSquare);
                            Check(spikeSquare);
                        }
                    }
                }
            }

            Check(this);
            return output;
        }
    }

    // State is used to indicate whether or not spikes are raised
    public override bool IsMultiState { get { return true; } }

    /// <summary>
    /// The state of this square.
    /// </summary>
    public override int State
    {
        get; set;
    }

    // Has the square just been spiky? 
    public bool HasSpiked { get; set; }

    // An integer used to track which of the square's links should be used next time the platform is at this square
    private int LinkState { get; set; }

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
    /// Reset HasMoved to false
    /// </summary>
    public override void OnPlayerTurnStart()
    {
        HasSpiked = false;
    }

    /// <summary>
    /// Changes the state as the player moves.
    /// </summary>
    public override void OnPlayerMove()
    {
        if (State == 1 && !HasSpiked)
        {
            if (Links[LinkState % Links.Count] is SpikeSquare nextSquare)
            {
                // Activates the next spike square
                nextSquare.State = 1;
                nextSquare.HasSpiked = true;

                // Step this square's links
                LinkState++;

                // Stops it being spiky
                State = 0;

                // Apply visuals for this spike and the next
                ApplyVisuals();
                nextSquare.ApplyVisuals();

                // Play a sound effect
                AudioManager.Play(AudioManager.SoundEffects.metalSwoosh);
            }
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
        if (State == 1)
        {
            Debug.Log("Player Dies");
            AudioManager.Play(AudioManager.SoundEffects.ouch);
        }
        else
        {
            Debug.Log("You have survived... for now");
        }
    }


    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {
        SpikeGraphics.transform.position = retractedPosition.transform.position;
        ApplyVisuals();
    }

    /// <summary>
    /// Apply the visual positioning of the spike graphics
    /// </summary>
    public void ApplyVisuals()
    {
        Transform targetTransform = State == 1 ? activePosition : retractedPosition;
        SpikeGraphics.SlideTo(targetTransform.position, -1, false);
    }
}
