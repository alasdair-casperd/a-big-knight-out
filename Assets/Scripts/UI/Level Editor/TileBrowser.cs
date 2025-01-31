using System.Linq;
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
            // Dynamically create browser items for each tile type
            // These are added in reverse order as an easy way of placing them before the moving platform selector
            foreach (var tileType in TileType.All.Reverse())
            {
                var item = Instantiate(TileBrowserItemPrefab, transform);
                item.transform.SetAsFirstSibling();
                item.GetComponent<Button>().onClick.AddListener(() => levelEditor.SelectTileType(tileType));
                item.TileType = tileType;
            }
        }
    }

}