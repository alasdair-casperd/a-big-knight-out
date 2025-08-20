using UnityEngine;
using System;
using TMPro;

/// <summary>
/// A square used to launch a specified level
/// </summary>
public class LevelSquare : Square
{
    public override TileType Type => TileType.Level;

    [SerializeField] private TextMeshPro levelText;

    [SerializeField] private GameObject lockedGraphics;
    [SerializeField] private GameObject activeGraphics;
    [SerializeField] private GameObject pressedGraphics;
    [SerializeField] private Light pointLight;

    private GameManager gameManager;

    /// <summary>
    /// The length of time to pause before transitioning to the level.
    /// </summary>
    public static float pauseTime = 0.5f;

    private static float maxLightBrightness = 0.5f;

    private void Start()
    {
        GetGameManager();
    }

    private void GetGameManager()
    {
        if (gameManager != null) return;
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.None)[0];
    }

    // Always passable
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

    public static Vector2Int LastUsedSquarePosition
    {
        get
        {
            return new(PlayerPrefs.GetInt("last-position-x"), PlayerPrefs.GetInt("last-position-y"));
        }
    }

    /// <summary>
    /// When the player lands on the square, transition to the target level. The target level is specified by the state.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Play a success sound
        AudioManager.Play(AudioManager.SoundEffects.click);

        PlayerPrefs.SetInt("last-position-x", Position.x);
        PlayerPrefs.SetInt("last-position-y", Position.y);

        GetGameManager();
        if (gameManager == null) return;

        // Disable player input
        gameManager.InputLocked = true;

        // Transition to the target level
        void transition()
        {

            var targetIndex = State - 1;

            var targetLevel = gameManager.LevelManager.Levels[targetIndex];
            if (targetLevel == null)
            {
                Debug.Log("Error: target level does not exist.");
                return;
            }

            gameManager.TransitionToLevel(targetLevel);
        }

        // After a delay, transition to the target level
        LeanTween.delayedCall(pauseTime, transition);
    }

    public override void UpdateGraphics()
    {
        // Update text
        var text = State.ToString();
        if (text.Length == 1) text = '0' + text;
        levelText.text = text;

        //  Update state (locked, inactive, active, pressed)
        GetGameManager();
        if (gameManager == null) return;

        lockedGraphics.SetActive(false);
        activeGraphics.SetActive(false);
        pressedGraphics.SetActive(false);

        if (gameManager.player == null)
        {
            activeGraphics.SetActive(true);
            return;
        }

        var playerDistance = (gameManager.player.position - Position).magnitude;
        if (!IsPassable) lockedGraphics.SetActive(true);
        else if (playerDistance < 0.1) pressedGraphics.SetActive(true);
        else activeGraphics.SetActive(true);
    }

    public override void OnLevelTurn()
    {
        UpdateGraphics();
    }

    public void Update()
    {
        GetGameManager();
        if (gameManager == null) return;
        if (gameManager.player == null) return;
        var playerDistance = (gameManager.player.gameObject.transform.position - GridUtilities.GridToWorldPos(Position)).magnitude;
        pointLight.intensity = Math.Clamp(maxLightBrightness * 1 / (1 + 0.5f * playerDistance), 0, 1 / 1.75f);
    }
}
