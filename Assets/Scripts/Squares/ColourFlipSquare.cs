using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A floor square which changes between two colours when the
/// player lands.
/// </summary>
public class ColourFlipSquare : Square
{
    public override TileType Type
    {
        get { return TileType.ColourFlip; }
    }

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change a colour flip square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    [SerializeField]
    private GameObject _whiteTile;

    [SerializeField]
    private GameObject _blackTile;

    /// <summary>
    /// Swaps the graphics for a tile.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Plays portal sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);

        // Changes the state from black to white and 
        // vice versa.
        if (State == 0)
        {
            _whiteTile.SetActive(false);
            _blackTile.SetActive(true);
            State = 1;
        }
        else if (State == 1)
        {
            _blackTile.SetActive(false);
            _whiteTile.SetActive(true);
            State = 0;
        }


    }

    public override void OnLevelStart()
    {
        if (State == 0)
        {
            _whiteTile.SetActive(true);
            _blackTile.SetActive(false);
        }
        else if (State == 1)
        {
            _blackTile.SetActive(true);
            _whiteTile.SetActive(false);
        }
    }
}
