using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used to enable the SquareManager to queue actions to occur once all game-blocking animations have completed
/// </summary>
public static class ActionQueue
{
    /// <summary>
    /// A GameObject on which to store all animations which should prevent game logic from advancing
    /// </summary>
    public static GameObject GameBlockingAnimationsContainer
    {
        get {
            if (_gameBlockingAnimationsContainer == null)
            {
                _gameBlockingAnimationsContainer = new GameObject("Game Blocking Animations");
            }
            return _gameBlockingAnimationsContainer;
        }
    }
    private static GameObject _gameBlockingAnimationsContainer;

    /// <summary>
    ///  A queue of actions to be performed in sequence after all game-blocking animations have finished
    /// </summary>
    private static List<Action> queue = new();
    
    /// <summary>
    /// Adds an action to the queue to be performed after all game-blocking animations have finished
    /// </summary>
    /// <param name="action"></param>
    public static void QueueAction(Action action)
    {
        queue.Add(action);
    }

    public static void Update()
    {
        // Invokes queued actions in order as soon as no game-blocking animations are playing
        while (!(queue.Count == 0) && !LeanTween.isTweening(GameBlockingAnimationsContainer))
        {
            Action action = queue[0];
            queue.RemoveAt(0);
            action.Invoke();
        }
    }
}
