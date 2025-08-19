using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A manager to handle the top level interaction with all of the levels enemies.
/// </summary>
[RequireComponent(typeof(GameManager))]
public class EnemyManager : MonoBehaviour
{
  /// <summary>
  /// A list to find the enemy object at any given position
  /// </summary>
  public List<Enemy> enemies;

  PlayerController player;

  GameManager gameManager;

  SquareManager squareManager;


  public void InitialiseEnemies(List<Enemy> inputEnemies)
  {
    enemies = inputEnemies;

    // Initialise the enemies
    foreach (Enemy enemy in enemies)
    {
      enemy.PlayerController = player;
      enemy.SquareManager = squareManager;
      enemy.EnemyManager = this;
      enemy.OnLevelStart();
    }
  }

  public void Initialise(PlayerController player)
  {
    gameManager = GetComponent<GameManager>();
    this.player = player;
    squareManager = GetComponent<SquareManager>();

    foreach (Enemy enemy in enemies)
    {
      enemy.PlayerController = player;
    }
  }

  /// <summary>
  /// The actions to be performed once the player has input their move
  /// </summary>
  public void OnPlayerMove()
  {

    foreach (Enemy enemy in enemies)
    {
      enemy.OnPlayerMove();
    }
  }

  /// <summary>
  /// The actions to be performed once the player lands on an enemy.
  /// </summary>
  public void OnPlayerLand()
  {
    // Does all of the enemies's turns.
    foreach (Enemy enemy in enemies.ToList())
    {
      if (enemy.Position == player.position)
      {
        Destroy(enemy.gameObject);
        enemies.Remove(enemy);
      }
    }
  }

  public void OnEnemyTurn()
  {
    // Does all of the enemies's turns.
    foreach (Enemy enemy in enemies)
    {
      enemy.OnEnemyTurn();
      if (enemy.Position != enemy.NextSquare)
      {
        enemy.MoveTo(enemy.NextSquare, AnimationController.MovementType.Slide);
      }
    }
  }

  /// <summary>
  /// The actions to be performed on the level's turn.
  /// </summary>
  public void OnLevelTurn()
  {
  }

  /// <summary>
  /// The actions to be performed at the start of the player's turn.
  /// </summary>
  public void OnPlayerTurnStart()
  {
    foreach (Enemy enemy in enemies)
    {
      enemy.OnPlayerTurnStart();
    }
  }


}
