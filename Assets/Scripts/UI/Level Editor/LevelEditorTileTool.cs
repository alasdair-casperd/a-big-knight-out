using UnityEngine;
using TMPro;
using System.Linq;

namespace UI
{
    [System.Serializable]
    public class LevelEditorTileTool: LevelEditorTool
    {
        private TileType _tileType;
        public TileType TileType
        {
            get => _tileType;
            set
            {
                _tileType = value;
                UpdateVisuals();
            }
        }

        public TextMeshProUGUI ToolName;

        private void UpdateVisuals()
        {   
            gameObject.name = TileType.DisplayName + " Tool";
            ToolName.text = TileType.DisplayName;
        }
    }

}