using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class FeatureBrowser: MonoBehaviour
    {
        [SerializeField]
        private EntityBrowserItem EntityBrowserItemPrefab;

        [SerializeField]
        private LevelEditor levelEditor;

        private void Start()
        {
            foreach (var entityType in EntityType.All)
            {
                var item = Instantiate(EntityBrowserItemPrefab, transform);
                item.GetComponent<Button>().onClick.AddListener(() => levelEditor.SelectEntityType(entityType));
                item.EntityType = entityType;
            }
        }
    }

}