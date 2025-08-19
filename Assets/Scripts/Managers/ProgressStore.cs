
using UI;
using UnityEngine;

/// <summary>
/// A static class used to store which levels the player has completed in PlayerPrefs.
/// </summary>
public static class ProgressStore
{
    private static string totalLevelsKey = "total-levels-completed";

    /// <summary>
    /// The total number of levels completed.
    /// </summary>
    /// <returns></returns>
    public static int TotalLevelsCompleted()
    {
        return PlayerPrefs.GetInt(totalLevelsKey);
    }

    /// <summary>
    /// Checks whether a level has been marked as completed.
    /// </summary>
    /// <param name="id">The id of the level to check.</param>
    /// <returns></returns>
    public static bool LevelCompleted(string id)
    {
        return PlayerPrefs.GetInt(id) == 1;
    }

    /// <summary>
    /// Store whether or not a level has been completed.
    /// </summary>
    /// <param name="id">The id of the level to target.</param>
    /// <param name="value"></param>
    public static void SetLevelCompletion(string id, bool value)
    {
        if (value && PlayerPrefs.GetInt(id) == 0)
        {
            var totalLevelsCompleted = TotalLevelsCompleted();
            PlayerPrefs.SetInt(totalLevelsKey, totalLevelsCompleted + 1);
        }
        PlayerPrefs.SetInt(id, value ? 1 : 0);
    }

    /// <summary>
    /// Reset the progress of the player so that all levels are marked as not completed.
    /// </summary>
    public static void ResetAllProgress()
    {
        // TODO: Implement this. Needs a way to access the list of all levels.   
    }
}