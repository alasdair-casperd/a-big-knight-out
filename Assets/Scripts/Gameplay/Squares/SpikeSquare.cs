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

    /// <summary>
    /// Note this is multistate as this is multiplatforms pretending to be one.
    /// </summary>
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
                nextSquare.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

                // Step this square's links
                LinkState++;

                // Stops it being spiky
                GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                State = 0;
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
        if (State == 1)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }

    }

}
