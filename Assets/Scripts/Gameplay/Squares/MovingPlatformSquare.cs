using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// A square which moves between other locations.
/// </summary>
public class MovingPlatformSquare : Square
{
    public override TileType Type
    {
        get { return TileType.MovingPlatform; }
    }

    public override List<Square> Links
    {
        get; set;
    }

    /// <summary>
    /// Recursively finds all moving platform squares linked to this one
    /// </summary>
    private List<MovingPlatformSquare> AllLinks
    {
        get
        {
            var output = new List<MovingPlatformSquare>();

            void Check(MovingPlatformSquare target)
            {
                foreach (var square in target.Links)
                {
                    if (square is MovingPlatformSquare platform)
                    {
                        if (!output.Contains(platform))
                        {
                            output.Add(platform);
                            Check(platform);
                        }
                    }
                }
            }

            Check(this);
            return output;
        }
    }

    /// <summary>
    /// The graphics prefab for this moving platform
    /// </summary>
    public AnimationController GraphicsPrefab;

    /// <summary>
    /// The graphics for this moving platform, i.e. the visual component that actually moves between locations
    /// This animator is shared between this moving platform and all of its links
    /// </summary>
    public AnimationController Graphics { get; set; }

    // Has the platform just moved to this square? This is used to prevent multiple platform moves occurring in a single level turn
    public bool HasMoved { get; set; }

    // An integer used to track which of the square's links should be used next time the platform is at this square
    private int LinkState { get; set; }

    // Is the square passable? Returns true if the 'actual platform' is currently at this platform square
    public override bool IsPassable
    {
        get
        {
            return State == 1;
        }
        protected set
        {
            Debug.LogWarning("Attempting to manually change whether a moving platform square is passable");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    // Is the platform carrying a player?
    private bool carryingPlayer;

    /// <summary>
    /// Reset HasMoved to false
    /// </summary>
    public override void OnPlayerTurnStart()
    {
        HasMoved = false;
    }

    public override void OnPlayerLand()
    {
        // Log that this platform is now carrying the player
        carryingPlayer = true;

        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);
    }

    /// <summary>
    /// On the level turn we want to move the player and reset the platform.
    /// Also want to change if it is passable.
    /// </summary>
    public override void OnLevelTurn()
    {
        if (State == 1 && !HasMoved)
        {
            if (Links[LinkState % Links.Count] is MovingPlatformSquare nextSquare)
            {
                // Activates the next linked moving platform square
                nextSquare.State = 1;
                nextSquare.HasMoved = true;

                // Step this square's links
                LinkState++;

                // Moves the platform
                Graphics.SlideTo(GridUtilities.GridToWorldPos(nextSquare.Position), Graphics.SlideDuration);
                State = 0;
            }
        }

        // Moves the player, if carrying
        if (carryingPlayer)
        {
            PlayerController.MoveTo(Links[0].Position, AnimationController.MovementType.Slide, Graphics.SlideDuration);
        }

        carryingPlayer = false;
    }

    /// <summary>
    /// Setting up the platforms for the start of the level.
    /// </summary>
    public override void OnLevelStart()
    {
        if (State == 1)
        {
            // Creates the platform graphics
            Graphics = Instantiate(GraphicsPrefab, transform.position, transform.rotation).GetComponent<AnimationController>();

            // Give the other linked squares a reference to these graphics
            foreach (MovingPlatformSquare platform in AllLinks)
            {
                platform.Graphics = Graphics;
            }
        }
    }
}
