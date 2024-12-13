using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(TilePrefabManager))]
public class SquarePrefabManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        TilePrefabManager prefabManager = (TilePrefabManager)target;
        EditorUtility.SetDirty(prefabManager);

        int maxTileNumber = 1000;
        if(prefabManager.prefabs == null || prefabManager.prefabs.Length != maxTileNumber)
        {
            prefabManager.prefabs = new GameObject[maxTileNumber];
        }
        
        foreach(TileType tile in TileType.All)
        {
            EditorGUILayout.LabelField(tile.DisplayName);
            prefabManager.prefabs[tile.ID] = EditorGUILayout.ObjectField(prefabManager.prefabs[tile.ID],typeof(GameObject),false) as GameObject;
        }
    }
}
