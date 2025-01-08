
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// A class used to dynamically generate preview images of each of the game's tiles
    /// </summary>
    public class TilePreviewGenerator : MonoBehaviour
    {
        public Camera previewCameraPrefab;
		public TilePrefabManager tilePrefabManager;

		private readonly int width = 256;
		private readonly int height = 256;
		private readonly int depth = 25;
		
		public Sprite GeneratePreviewSprite(TileType tileType)
		{
			Debug.Log("Generating Preview for tile type: " + tileType.DisplayName);

			Camera previewCamera = Instantiate(previewCameraPrefab, transform);
			GameObject square = Instantiate(tilePrefabManager.GetPrefab(tileType), transform);

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
			Destroy(square);

			Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

			Debug.Log("Preview generated successfully");
			return sprite;

		}
    }
}