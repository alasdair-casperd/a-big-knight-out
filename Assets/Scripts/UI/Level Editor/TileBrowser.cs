using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class TileBrowser: MonoBehaviour
    {
        [SerializeField]
        private TileBrowserItem TileBrowserItemPrefab;

        [SerializeField]
        private LevelEditor levelEditor;

        private void Start()
        {
            foreach (var tileType in TileType.All)
            {
                var item = Instantiate(TileBrowserItemPrefab, transform);
                item.GetComponent<Button>().onClick.AddListener(() => levelEditor.SelectTileType(tileType));
                item.TileType = tileType;
            }
        }
    }

}