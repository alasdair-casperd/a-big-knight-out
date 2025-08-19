using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        public float transitionDuration = 1;

        private TextMeshProUGUI[] childTextElements;
        private float[] childTextElementsDefaultAlphas;

        private Image[] childImages;
        private float[] childImagesDefaultAlphas;

        private bool initialised;

        private void Initialise()
        {
            // Access child images
            childImages = GetComponentsInChildren<Image>();
            childImages = Array.FindAll(childImages, image => image.GetComponent<Fader>() == null || image.gameObject == this.gameObject);

            // Store their default alphas
            childImagesDefaultAlphas = new float[childImages.Length];
            for (int i = 0; i < childImages.Length; i++)
            {
                childImagesDefaultAlphas[i] = childImages[i].color.a;
            }

            // Access child text elements
            childTextElements = GetComponentsInChildren<TextMeshProUGUI>();
            childTextElements = Array.FindAll(childTextElements, text => text.GetComponent<Fader>() == null || text.gameObject == this.gameObject);

            // Store their default alphas
            childTextElementsDefaultAlphas = new float[childTextElements.Length];
            for (int i = 0; i < childTextElements.Length; i++)
            {
                childTextElementsDefaultAlphas[i] = childTextElements[i].color.a;
            }

            // Close
            initialised = true;
        }

        public void Show(float duration = -1, Action onComplete = null)
        {
            if (duration < 0)
            {
                duration = transitionDuration;
            }

            // Create lists
            if (!initialised)
            {
                Initialise();
            }

            if (onComplete == null)
            {
                onComplete = delegate { };
            }

            LeanTween.value(gameObject, 0, 1, duration).setOnComplete(onComplete);

            for (int i = 0; i < childImages.Length; i++)
            {
                Image image = childImages[i];
                image.gameObject.SetActive(true);
                image.raycastTarget = true;

                Color transparent = image.color;
                transparent.a = 0;
                image.color = transparent;

                LeanTween.value(gameObject, 0, childImagesDefaultAlphas[i], duration).setOnUpdate((float val) =>
                {
                    Color newColor = image.color;
                    newColor.a = val;
                    image.color = newColor;
                });
            }

            for (int i = 0; i < childTextElements.Length; i++)
            {
                TextMeshProUGUI text = childTextElements[i];
                text.gameObject.SetActive(true);
                text.raycastTarget = true;

                Color transparent = text.color;
                transparent.a = 0;
                text.color = transparent;

                LeanTween.value(gameObject, 0, childTextElementsDefaultAlphas[i], duration).setOnUpdate((float val) =>
                {
                    Color newColor = text.color;
                    newColor.a = val;
                    text.color = newColor;
                });
            }
        }

        public void Dismiss(float duration = -1, Action onComplete = null)
        {
            if (duration < 0)
            {
                duration = transitionDuration;
            }

            // Create lists
            if (!initialised)
            {
                Initialise();
            }

            if (onComplete == null)
            {
                onComplete = delegate { };
            }

            LeanTween.value(gameObject, 0, 1, duration).setOnComplete(onComplete);

            for (int i = 0; i < childImages.Length; i++)
            {
                Image image = childImages[i];
                image.gameObject.SetActive(true);
                image.raycastTarget = true;

                LeanTween.value(gameObject, childImagesDefaultAlphas[i], 0, duration).setOnUpdate((float val) =>
                {
                    Color newColor = image.color;
                    newColor.a = val;
                    image.color = newColor;
                })
                .setOnComplete(() => image.gameObject.SetActive(false));
            }

            for (int i = 0; i < childTextElements.Length; i++)
            {
                TextMeshProUGUI text = childTextElements[i];
                text.gameObject.SetActive(true);
                text.raycastTarget = true;

                LeanTween.value(gameObject, childTextElementsDefaultAlphas[i], 0, duration).setOnUpdate((float val) =>
                {
                    Color newColor = text.color;
                    newColor.a = val;
                    text.color = newColor;
                }).setOnComplete(() => text.gameObject.SetActive(false));
            }
        }
    }
}
