using UnityEngine;

[CreateAssetMenu(fileName = "EntityPrefabManager", menuName = "Scriptable Objects/EntityPrefabManager")]
public class EntityPrefabManager : ScriptableObject
{
    public GameObject[] prefabs;

    /// <summary>
    /// Returns the game object associated with a given tile type.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public GameObject GetPrefab(EntityType entity)
    {
        return prefabs[entity.ID];
    }
}
