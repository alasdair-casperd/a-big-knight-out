
using UI;
using UnityEngine;

/// <summary>
/// A scriptable object on which to define the sequence of levels used in the game.
/// </summary>
[CreateAssetMenu(fileName = "LevelManager", menuName = "Scriptable Objects/LevelManager")]
public class LevelManager : ScriptableObject
{
    [System.Serializable]
    public class LevelEntry
    {
        public string Name;
        public TextAsset LevelFile;
    }

    public LevelEntry MenuLevel;

    /// <summary>
    /// The ordered list of levels used in the game, excluding the menu level.
    /// </summary>
    public LevelEntry[] Levels;
}