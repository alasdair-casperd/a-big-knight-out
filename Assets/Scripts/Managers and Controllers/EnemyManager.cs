using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// A manager to handle the top level interaction with all of the levels enemies.
/// </summary>
[RequireComponent(typeof(GameManager))]
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// A dictionary to find the square object at any given position
    /// </summary>
    public Dictionary<Vector2Int, Enemy> enemies;

    bool isPlayerTurn;

    PlayerController player;

    GameManager gameManager;

    SquareManager squareManager;


    public void InitialiseEnemies(Dictionary<Vector2Int, Enemy> inputEnemies)
    {
        enemies = inputEnemies;

        // Initialise the enemies
        foreach (Enemy enemy in enemies.Values)
        {
            enemy.PlayerController = player;
            enemy.OnLevelStart();
        }
    }

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        player = gameManager.player;
        squareManager = GetComponent<SquareManager>();
    }

    void Update()
    {
    }

    /// <summary>
    /// The actions to be performed once the player has input their move
    /// </summary>
    public void OnPlayerMove()
    {

        isPlayerTurn = false;
        foreach (Enemy enemy in enemies.Values)
        {
            enemy.OnPlayerMove();
        }
    }

    /// <summary>
    /// The actions to be performed once the player lands on its new tile.
    /// </summary>
    public void OnPlayerLand()
    {
        enemies[player.position].OnPlayerLand();
    }

    /// <summary>
    /// The actions to be performed on the level's turn.
    /// </summary>
    public void OnLevelTurn()
    {
        // Does all of the square's turns.
        foreach (Enemy enemy in enemies.Values)
        {
            enemy.OnLevelTurn();
        }
    }

    /// <summary>
    /// The actions to be performed at the start of the player's turn.
    /// </summary>
    public void OnPlayerTurnStart()
    {
        isPlayerTurn = true;

        foreach (Enemy enemy in enemies.Values)
        {
            enemy.OnPlayerTurnStart();
        }

    }

}
