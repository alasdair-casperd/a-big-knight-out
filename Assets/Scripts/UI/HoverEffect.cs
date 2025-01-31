using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class HoverEffect: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private float scaleFactor = 1.03f;
        private Vector3 baseSize;

        // Enlarge on mouse over
        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, scaleFactor * baseSize, 0.1f);
        }

        // Shrink on mouse exit
        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, baseSize, 0.1f);
        }

        private void Start()
        {
            baseSize = transform.localScale;
        }
    }
}