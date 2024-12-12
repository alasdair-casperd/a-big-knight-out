using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI
{
    [System.Serializable]
    public class LevelEditorTool : MonoBehaviour
    {

        [Header("Level Editor")]
        [SerializeField]
        private LevelEditor levelEditor;

        [Header("Events")]
        public UnityEvent OnSelect = new();
        public UnityEvent OnDeselect = new();
        public UnityEvent OnSquareClick = new();
        public UnityEvent OnSquareMouseDown = new();
        public UnityEvent OnSquareDrag = new();

        public void Select()
        {
            levelEditor.SelectTool(this);
        }
    }

}