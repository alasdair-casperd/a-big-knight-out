using UnityEngine;

/// <summary>
/// A basic square that always kills the player.
/// </summary>
public class PitSquare : Square
{
    public override TileType Type => TileType.Pit;

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change whether a pit square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    public override void OnPlayerLand()
    {
        AudioManager.Play(AudioManager.SoundEffects.ouch);
        PlayerController.Die();
    }
    public override void OnEnemyLand(Enemy enemy)
    {
        enemyManager.enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
