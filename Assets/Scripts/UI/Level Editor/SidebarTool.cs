using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI
{
    [System.Serializable]
    [RequireComponent(typeof(SidebarToolSelector))]
    public class SidebarTool : MonoBehaviour
    {

        [Header("References")]
        public LevelEditor levelEditor;

        [Header("Selection Events")]
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeselect = new();
        
        [Header("Tile Events")]
        public UnityEvent OnTileClick = new();
        public UnityEvent OnTileMouseDown = new();
        public UnityEvent OnTileMouseUp = new();
        public UnityEvent OnTileDrag = new();
        public UnityEvent OnTileMouseHeld = new();

        [Header("Entity Events")]
        public UnityEvent OnEntityClick = new();
        public UnityEvent OnEntityMouseDown = new();
        public UnityEvent OnEntityMouseUp = new();
        public UnityEvent OnEntityDrag = new();
        public UnityEvent OnEntityMouseHeld = new();
    }
}