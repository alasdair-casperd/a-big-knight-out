using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Demo;

/// <summary>
/// A Spike square that is deadly to the player when active, controlled by an retraction
/// pattern and electricity
/// </summary>
public class SpikeSquare : Square
{
    public override TileType Type => TileType.Spikes;

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
    /// Are the spikes currently retracted?
    /// </summary>
    private bool spikesRetracted;

    /// <summary>
    /// The retraction pattern that the spikes will loop through, where false is retracted and true is active.
    /// Electricity effectively toggles each value.
    /// </summary>
    private bool[] retractionPattern
    {
        get
        {
            // Parse the initial state integer as a retraction pattern
            string stateString = State.ToString();
            if (stateString.All(c => c == '1' || c == '2'))
            {
                bool[] output = stateString.Select(c => c == '2').ToArray();
                if (output.Length > 0)
                {
                    return output;
                }
            }

            Debug.LogError("Invalid retraction pattern for SpikeSquare: " + State);
            return new bool[1] { false };
        }
    }

    /// <summary>
    /// Have the spike graphics retracted (or at least started the animation)?
    /// </summary>
    private bool graphicsRetracted;

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
    private int turnCounter { get; set; }


    /// <summary>
    /// Changes the spike up and down depending on the frequency.
    /// </summary>
    public override void OnPlayerMove()
    {
        turnCounter++;
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

        if (!spikesRetracted)
        {
            PlayerController.Die();
            AudioManager.Play(AudioManager.SoundEffects.ouch);
        }

    }

    /// <summary>
    /// Just to check whether the player has landed on a spike
    /// tile and deserves to die.
    /// </summary>
    public override void OnEnemyLand(Enemy enemy)
    {

        // Check for death
        if (spikesRetracted)
        {
            Debug.Log("Enemy lives");
        }
        else
        {
            Debug.Log("Enemy dies");
            AudioManager.Play(AudioManager.SoundEffects.ouch);
            enemyManager.enemies.Remove(enemy);
            Destroy(enemy.gameObject);
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
        turnCounter = 0;
        UpdateSpikes();
    }

    public override void OnChargeChanged()
    {
        UpdateSpikes();
    }

    public void UpdateSpikes()
    {
        // Read the current state of the spikes from the retraction pattern
        spikesRetracted = !retractionPattern[turnCounter % retractionPattern.Length];

        // If the spikes are receiving charge, invert the retraction pattern
        if (IsReceivingCharge)
        {
            spikesRetracted = !spikesRetracted;
        }

        UpdateGraphics();
    }

    public override void UpdateGraphics()
    {
        if (graphicsRetracted != spikesRetracted)
        {
            graphicsRetracted = spikesRetracted;
            Transform targetTransform = spikesRetracted ? retractedPosition : activePosition;
            SpikeGraphics.SlideTo(targetTransform.position, -1, false);
            AudioManager.Play(AudioManager.SoundEffects.metalSwoosh);
        }
    }
}
