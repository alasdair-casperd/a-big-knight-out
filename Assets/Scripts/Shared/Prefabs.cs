
using UnityEngine;

/// <summary>
/// A scriptable object on which to store prefabs.
/// Note that square prefabs are stored on the TilePrefabManager instead
/// </summary>
[CreateAssetMenu(fileName = "Prefabs", menuName = "Scriptable Objects/Prefabs")]
public class Prefabs: ScriptableObject {

    [Header("Gameplay")]
    // Add gameplay prefabs here

    [Header("UI")]
    public UI.ValidMoveIndicator validMoveIndicator;
    public UI.LinkIndicator linkIndicator;
}