using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A basic floor square which is always passable.
/// </summary>
public class MovingPlatformSquare : Square
{
    public override TileType Type
    {
        get { return TileType.MovingPlatform; }
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
        protected set
        {
            Debug.LogWarning("Trying to change whether a floor square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// Just to check whether the player has landed on a moving platform.
    /// May be deleted in due course.
    /// </summary>
    public override void OnPlayerLand()
    {
        Debug.Log("I have landed on a moving platform");
        PlayerController.MoveToVector2(Links[0].Position);
    }

    /// <summary>
    /// On the level turn we want to move the player and reset the platform.
    /// </summary>
    public override void OnLevelTurn()
    {

        State += 1;
        if (State % (Links.Count + 1) == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {
        if (State % (Links.Count + 1) == 0)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
