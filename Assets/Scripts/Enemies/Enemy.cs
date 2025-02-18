using UnityEngine;
using System.Linq;
using NUnit.Framework;

[RequireComponent(typeof(AnimationController))]
public abstract class Enemy : MonoBehaviour
{
    /// <summary>
    /// The current square manager
    /// </summary> 
    public SquareManager SquareManager;

    /// <summary>
    /// The current enemy manager.
    /// </summary>
    public EnemyManager EnemyManager;

    /// <summary>
    /// The attached animation controller
    /// </summary> 
    private AnimationController AnimationController;

    /// <summary>
    /// Allows access to the player controller script.
    /// </summary>
    public PlayerController PlayerController { get; set; }

    /// <summary>
    /// The type of entity.
    /// </summary>
    public abstract EntityType Type { get; }

    /// <summary>
    /// The possible move of the enemy.
    /// </summary>
    public abstract Vector2Int[] EnemyMove { get; }

    public Vector2Int Position { get; set; }

    /// <summary>
    /// The next square the enemy will move to.
    /// </summary>
    public Vector2Int NextSquare { get; set; }

    public int Direction { get; set; }

    /// <summary>
    /// The graphics variant to use for this enemy.
    /// </summary>
    public abstract int GraphicsVariant { get; set; }

    /// <summary>
    /// The state of this enemy, if not implemented, it will raise a warning.
    /// </summary>
    public int State
    {
        get
        {
            return _state;
        }
        set
        {
            if (Type.ValidStates.Contains(value)) _state = value;
            else Debug.Log($"Attempting to assign a state of {value} to a tile of type '{Type.DisplayName}'");
        }
    }
    private int _state;

    private void Start()
    {
        AnimationController = GetComponent<AnimationController>();
    }

    public void SetInitialPosition(Vector2Int startPos)
    {
        Position = startPos;
        transform.position = GridUtilities.GridToWorldPos(Position);
    }

    /// <summary>
    /// Move the Enemy to a given position with the specified animation style
    /// </summary>
    /// <param name="endPosition"> The world position to move the player to </param>
    public void MoveTo(Vector2Int to, AnimationController.MovementType movementType, float duration = -1)
    {
        // Update the square manager
        Position = to;

        // Convert grid position to world position
        Vector3 endPosition = GridUtilities.GridToWorldPos(to);

        // Animate the movement
        switch (movementType)
        {
            case AnimationController.MovementType.Jump:
                AnimationController.JumpTo(endPosition, duration);
                break;
            case AnimationController.MovementType.Slide:
                AnimationController.SlideTo(endPosition, duration);
                break;
            case AnimationController.MovementType.Teleport:
                AnimationController.TeleportTo(endPosition, duration);
                break;
        }
    }

    public void MoveAlongPath(Vector2Int[] path, float duration = -1)
    {
        // Update the player controller position
        if (path.Length > 0) Position = path.Last();

        // Animate
        AnimationController.MoveAlongPath(path, duration);
    }

    /// <summary>
    /// The actions to be performed at the start of the level.
    /// </summary>
    public virtual void OnLevelStart() { }

    /// <summary>
    /// The actions to be performed at the start of the player's turn
    /// </summary>
    public virtual void OnPlayerTurnStart() { }

    /// <summary>
    /// The actions to be performed once the player has input their move
    /// </summary>
    public virtual void OnPlayerMove() { }

    /// <summary>
    /// The actions to be performed immediately when the player lands on this tile
    /// </summary>
    public virtual void OnPlayerLand() { }

    /// <summary>
    /// The actions to be perfored during the enemy turn.
    /// </summary>
    public virtual void OnEnemyTurn() { }

    /// <summary>
    /// The actions to be performed at the start of the Level's turn
    /// </summary>
    public virtual void OnLevelTurn() { }

    /// <summary>
    /// Checks if there is an enemy on a given square.
    /// </summary>
    /// <param name="square">The square you are checking.</param>
    /// <returns></returns>
    public bool CheckSquareForEnemy(Vector2Int square)
    {
        foreach (Enemy enemy in EnemyManager.enemies)
        {
            if (enemy.Position == square)
            {
                return true;
            }
        }

        return false;
    }
}
