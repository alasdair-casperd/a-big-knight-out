using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(SpriteGenerator))]
    [RequireComponent(typeof(Button))]
    public class GeneralBrowserItem: MonoBehaviour
    {   
        // The image to apply the generated sprite to
        public Image TargetImage;

        // The prefab to use to generate the preview image
        public GameObject previewSource;

        private void Start()
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {   
            gameObject.name = previewSource.name + " Selector";
            TargetImage.sprite = GetComponent<SpriteGenerator>().GeneratePreviewSprite(previewSource);
        }
    }

}