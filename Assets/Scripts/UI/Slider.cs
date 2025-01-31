using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Slider : MonoBehaviour
    {
        public enum SlideType
        {
            top, bottom, left, right
        }

        public LeanTweenType showEasing = LeanTweenType.easeOutExpo;
        public LeanTweenType dismissEasing = LeanTweenType.linear;

        [SerializeField]
        private SlideType slideType;

        public float showDuration = 0.5f;
        public float dismissDuration = 0.2f;

        private Vector2 onPivot;
        private Vector2 offPivot;
        private Vector2 onPosition;
        private Vector2 offPosition;

        private bool initialised;

        private void Initialise()
        {
            RectTransform item = GetComponent<RectTransform>();
            
            onPivot = item.pivot;
            onPosition = item.anchoredPosition;
                        
            float itemWidth = item.rect.width * item.localScale.x;
            float itemHeight = item.rect.height * item.localScale.y;

            switch (slideType)
            {
                case SlideType.top:
                    offPivot = new Vector2(0.5f, 1);
                    offPosition = new Vector2(onPosition.x, itemHeight);
                    break;
                case SlideType.bottom:
                    offPivot = new Vector2(0.5f, 0);
                    offPosition = new Vector2(onPosition.x, -itemHeight);
                    break;
                case SlideType.left:
                    offPivot = new Vector2(0, 0.5f);
                    offPosition = new Vector2(-itemWidth, onPosition.y);
                    break;
                case SlideType.right:
                    offPivot = new Vector2(1, 0.5f);
                    offPosition = new Vector2(itemWidth, onPosition.y);
                    break;
            }

            initialised = true;
        }

        private void Start()
        {
            if (!initialised) Initialise();
        }

        public void ChangeSlideType(SlideType newType)
        {
            slideType = newType;
            Initialise();
        }

        public void Show(float duration = -1)
        {
            if (duration < 0) duration = showDuration;
            if (!initialised) Initialise();

            Vector2 startingPosition;
            Vector2 startingPivot;

            RectTransform item = GetComponent<RectTransform>();

            if (!gameObject.activeInHierarchy)
            {           
                startingPosition = offPosition;
                startingPivot = offPivot; 
            }
            else
            {
                startingPosition = item.anchoredPosition;
                startingPivot = item.pivot;
            }

            gameObject.SetActive(true);
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float t) =>
            {
                item.pivot = startingPivot + t * (onPivot - startingPivot);
                item.anchoredPosition = startingPosition + t * (onPosition - startingPosition);
            })
            .setEase(showEasing);
        }

        public void Dismiss(float duration = -1)
        {        
            if (duration < 0)
            {
                duration = dismissDuration;
            }

            if (!initialised) Initialise();
            RectTransform item = GetComponent<RectTransform>();
            
            Vector2 startingPosition = item.anchoredPosition;
            Vector2 startingPivot = item.pivot;

            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 1, 0, duration).setOnUpdate((float t) =>
            {
                item.pivot = offPivot + t * (startingPivot - offPivot);
                item.anchoredPosition = offPosition + t * (startingPosition - offPosition);
            })
            .setEase(dismissEasing)
            .setOnComplete(() => gameObject.SetActive(false));
        }
    }
}
