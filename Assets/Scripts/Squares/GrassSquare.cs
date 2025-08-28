using UnityEngine;

/// <summary>
/// A basic floor square which is always passable.
/// </summary>
public class GrassSquare : Square
{
    public override TileType Type => TileType.Grass;

    // Will always report as passable, if you try to change that you get a warning.
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

    // might want to change this to a softer sound
    public override void OnPlayerLand()
    {
        AudioManager.Play(AudioManager.SoundEffects.thud);
    }
    private void Start()
    {
        transform.eulerAngles += new Vector3(0, Random.Range(0, 4) * 90, 0);
    }
}
