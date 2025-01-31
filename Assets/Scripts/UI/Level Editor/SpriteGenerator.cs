
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// A class used to dynamically generate a preview sprite of a prefab
    /// </summary>
    public class SpriteGenerator : MonoBehaviour
    {
        public Camera previewCameraPrefab;

        private readonly int width = 256;
        private readonly int height = 256;
        private readonly int depth = 25;
        
        public virtual Sprite GeneratePreviewSprite(GameObject prefab)
        {
            Camera previewCamera = Instantiate(previewCameraPrefab, transform);
            GameObject target = Instantiate(prefab, transform);

            RenderTexture renderTexture = new RenderTexture(width, height, depth);
            Rect rect = new(0, 0, width, height);
            Texture2D texture = new(width, height, TextureFormat.RGBA32, false);

            previewCamera.targetTexture = renderTexture;
            previewCamera.Render();

            RenderTexture currentRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();

            previewCamera.targetTexture = null;
            RenderTexture.active = currentRenderTexture;

            Destroy(renderTexture);
            Destroy(previewCamera.gameObject);
            Destroy(target);

            Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
            return sprite;
        }
    }
}