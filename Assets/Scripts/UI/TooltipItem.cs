using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TooltipItem: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string content;
        public Tooltip.Position position;

        public Tooltip tooltipPrefab;
        private Tooltip tooltip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip = Instantiate(tooltipPrefab, transform);
            tooltip.Initialise(content, position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip != null) Destroy(tooltip.gameObject);
        }
    }
}