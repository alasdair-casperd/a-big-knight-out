using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(EntityPrefabManager))]
public class EntityPrefabManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EntityPrefabManager prefabManager = (EntityPrefabManager)target;
        EditorUtility.SetDirty(prefabManager);

        // Scott once again please forgive me, and also don't design more than 1000 entities.
        int maxEntityNumber = 1000;
        if(prefabManager.prefabs == null || prefabManager.prefabs.Length != maxEntityNumber)
        {
            prefabManager.prefabs = new GameObject[maxEntityNumber];
        }
        
        foreach(EntityType entity in EntityType.All)
        {
            EditorGUILayout.LabelField(entity.DisplayName);
            prefabManager.prefabs[entity.ID] = EditorGUILayout.ObjectField(prefabManager.prefabs[entity.ID],typeof(GameObject),false) as GameObject;
        }
    }
}

