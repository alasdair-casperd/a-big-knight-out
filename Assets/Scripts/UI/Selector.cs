using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Selector: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool Selected { get; private set; }
        protected bool Hovered { get; private set; }

        public bool AllowMultiselect = false;

        private List<Selector> siblingSelectors
        {
            get
            {
                List<Selector> siblings = new List<Selector>();
                if (transform.parent == null) return siblings;
                foreach (Selector sibling in transform.parent.GetComponentsInChildren<Selector>())
                {
                    if (sibling != this) siblings.Add(sibling);
                }
                return siblings;
            }
        }

        private void DeselectSiblings()
        {
            foreach (Selector sibling in siblingSelectors)
            {
                if (!sibling.AllowMultiselect) sibling.Deselect();
            }
        }

        public void Select()
        {
            DeselectSiblings();
            Selected = true;
            OnSelect();
            UpdateVisuals();
        }

        public void Deselect()
        {
            Selected = false;
            OnDeselect();
            UpdateVisuals();
        }

        public void Toggle()
        {
            if (Selected) Deselect();
            else Select();
        }

        protected virtual void OnSelect()
        {
            return;
        }

        protected virtual void OnDeselect()
        {
            return;
        }

        protected virtual void UpdateVisuals()
        {
            return;
        }

        private void Awake()
        {
            UpdateVisuals();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered = true;
            UpdateVisuals();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hovered = false;
            UpdateVisuals();
        }
  }
}