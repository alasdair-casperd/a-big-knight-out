using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(SpriteGenerator))]
    public class TileBrowserItem: MonoBehaviour
    {   
        // The image to apply the generated sprite to
        public Image TargetImage;

        public TilePrefabManager TilePrefabManager;

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

        private void UpdateVisuals()
        {   
            gameObject.name = TileType.DisplayName + " Selector";
            TargetImage.sprite = GetComponent<SpriteGenerator>().GeneratePreviewSprite(TilePrefabManager.GetPrefab(TileType));
        }
    }

}