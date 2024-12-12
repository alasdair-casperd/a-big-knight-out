using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A square that after you land on, will dissapear.
/// </summary>
public class FallingFloorSquare : Square
{

    public override TileType Type
    {
        get { return TileType.Floor; }
    }

    public override bool IsLinkable { get { return false; } }

    /// <summary>
    /// Can be either fallen or not.
    /// </summary>
    public override bool IsMultiState { get { return true; } }

    /// <summary>
    /// The state of this square. 0 = landed on, 1 = ready to fall.
    /// </summary>
    public override int State
    {
        get; set;
    }



    // Is only passable before it is landed on.
    public override bool IsPassable
    {
        get
        {
            return State == 0;
        }
        protected set
        {
            Debug.LogWarning("Attempting to manually change whether a falling floor square is passable");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// When the platform lands, the state is set to 1.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Sets the state ready to fall when the player leaves.
        State = 1;

        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);
    }


    /// <summary>
    /// On the player landing, the square will dissapear
    /// and will no longer become passable.
    /// </summary>
    public override void OnPlayerMove()
    {
        if (State == 1)
        {
            gameObject.SetActive(false);
        }
    }

}