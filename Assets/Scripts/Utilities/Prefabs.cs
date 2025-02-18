
using UI;
using UnityEngine;

/// <summary>
/// A scriptable object on which to store prefabs.
/// Note that square prefabs are stored on the TilePrefabManager instead
/// </summary>
[CreateAssetMenu(fileName = "Prefabs", menuName = "Scriptable Objects/Prefabs")]
public class Prefabs : ScriptableObject
{
    [Header("Gameplay")]
    public PlayerController Player;
    public GameObject movingPlatform;

    [Header("UI")]
    public ValidMoveIndicator validMoveIndicator;
    public EnemyCaptureIndicator enemyCaptureIndicator;
    public LinkIndicator linkIndicator;
    public RotationIndicator rotationIndicator;
    public StateIndicator stateIndicator;
    public GameObject levelStartIndicator;
    public Dialogue dialogue;
}