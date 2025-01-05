using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI
{
    [System.Serializable]
    public class LevelEditorTool : MonoBehaviour
    {

        [Header("References")]
        public LevelEditor levelEditor;

        [SerializeField]
        private SelectionStyler selectionStyler;

        [Header("Events")]
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeselect = new();
        public UnityEvent OnSquareClick = new();
        public UnityEvent OnSquareMouseDown = new();
        public UnityEvent OnSquareMouseUp = new();
        public UnityEvent OnSquareDrag = new();
        public UnityEvent OnSquareMouseHeld = new();
        public UnityEvent OnLinkMouseDown = new();

        public void Select()
        {
            levelEditor.SelectTool(this);
            selectionStyler.SetHighlight(true);
        }

        public void Deselect()
        {
            levelEditor.SelectTool(this);
            selectionStyler.SetHighlight(false);
        }
    }

}