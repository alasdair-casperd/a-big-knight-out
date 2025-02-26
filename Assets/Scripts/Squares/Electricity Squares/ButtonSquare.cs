using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Demo;

/// <summary>
/// A square containing a switch that can trigger events via links, and is tog
/// </summary>
public class ButtonSquare : Square
{
    public override TileType Type => TileType.Button;

    private Enemy enemyOnButton;

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change if button is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    // Is the button currently pressed? This will be true when the player or an enemy is on the button
    private bool _isPressed;
    public bool IsPressed
    {
        get
        {
            return _isPressed;
        }
        set
        {
            _isPressed = value;
            UpdateOutgoingCharge();
        }
    }

    // To calculate this buttons charge, we just need to know if it is pressed
    public override bool CalculateCharge()
    {
        return IsPressed;
    }

    // TODO: Add support for enemies landing
    public override void OnPlayerLand()
    {
        IsPressed = true;

        // Play a click sound effect
        AudioManager.Play(AudioManager.SoundEffects.click);
    }

    // TODO: Add support for enemies landing
    public override void OnEnemyLand()
    {
        Enemy currentEnemy = null;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy.Position == Position)
            {
                currentEnemy = enemy;
                break;
            }
        }

        if (currentEnemy)
        {
            IsPressed = true;
            if (currentEnemy != enemyOnButton)
            {
                // Play a click sound effect if new enemy lands
                AudioManager.Play(AudioManager.SoundEffects.click);
            }
            foreach (Enemy enemy in enemyManager.enemies)
            {
                if (enemy.Position == Position)
                {
                    enemyOnButton = enemy;
                    break;
                }
            }

        }
    }

    public override void OnPlayerLeave()
    {
        IsPressed = false;

        // Play a click sound effect
        AudioManager.Play(AudioManager.SoundEffects.click);
    }

    public override void OnLevelTurn()
    {

        bool hasEnemyOn = false;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy.Position == Position)
            {
                hasEnemyOn = true;
            }
        }



        if (!hasEnemyOn && enemyOnButton && PlayerController.position != Position)
        {

            // Play a click sound effect
            AudioManager.Play(AudioManager.SoundEffects.click);
            IsPressed = false;
            enemyOnButton = null;
        }


    }
}
