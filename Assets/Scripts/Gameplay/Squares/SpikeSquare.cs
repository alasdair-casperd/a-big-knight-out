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

    // Will always report as passable, but if state is wrong will not let you land.
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
    /// Changes the state as the player moves.
    /// </summary>
    public override void OnPlayerMove()
    {
        State += 1;
        if (State % (Links.Count + 1) == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;

        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }

    /// <summary>
    /// Just to check whether the player has landed on a spike
    /// tile and deserves to die.
    /// </summary>
    public override void OnPlayerLand()
    {
        if (State % (Links.Count + 1) == 0)
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
        if (State % (Links.Count + 1) == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;

        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }
}
