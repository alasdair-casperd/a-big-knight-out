using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(SpriteGenerator))]
    [RequireComponent(typeof(Button))]
    public class EntityBrowserItem: MonoBehaviour
    {   
        // The image to apply the generated sprite to
        public Image TargetImage;

        public EntityPrefabManager EntityPrefabManager;

        private EntityType _entityType;
        public EntityType EntityType
        {
            get => _entityType;
            set
            {
                _entityType = value;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {   
            gameObject.name = EntityType.DisplayName + " Selector";
            TargetImage.sprite = GetComponent<SpriteGenerator>().GeneratePreviewSprite(EntityPrefabManager.GetPrefab(EntityType));
        }
    }

}