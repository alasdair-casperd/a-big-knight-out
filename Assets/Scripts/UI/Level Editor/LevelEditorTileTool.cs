using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;


namespace UI
{
    [System.Serializable]
    [RequireComponent(typeof(TilePreviewGenerator))]
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

        public Image PreviewImage;

        public TextMeshProUGUI ToolName;

        private void UpdateVisuals()
        {   
            gameObject.name = TileType.DisplayName + " Tool";
            ToolName.text = TileType.DisplayName;
            PreviewImage.sprite = GetComponent<TilePreviewGenerator>().GeneratePreviewSprite(TileType);
        }
    }

}