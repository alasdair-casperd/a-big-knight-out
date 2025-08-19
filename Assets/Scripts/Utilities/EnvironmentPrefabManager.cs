using UnityEngine;

/// <summary>
/// A temporary solution for creating level-specific environments.
/// </summary>
[CreateAssetMenu(fileName = "EnvironmentPrefabManager", menuName = "Scriptable Objects/EnvironmentPrefabManager")]
public class EnvironmentPrefabManager : ScriptableObject
{
    [System.Serializable]
    public class EnvironmentPrefab
    {
        public string LevelName;
        public GameObject prefab;
    }

    public EnvironmentPrefab[] Environments;
}
