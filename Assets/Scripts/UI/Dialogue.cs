using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{   
    /// <summary>
    /// A class representing a dialogue pop up box
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class Dialogue: MonoBehaviour
    {
        public Fader fader;
        public Fader faderPrefab;

        public struct HeaderData
        {
            public string title;
            public string body;
        }

        public struct ButtonData
        {
            public string text;
            public UnityEngine.Events.UnityAction action;
        }

        public TextMeshProUGUI Title;
        public TextMeshProUGUI Body;
        public TextMeshProUGUI PrimaryButtonText;
        public TextMeshProUGUI SecondaryButtonText;

        public Button PrimaryButton;
        public Button SecondaryButton;
        
        public void Dismiss()
        {
            fader.Dismiss(0.2f, onComplete: () => {
                Destroy(fader.gameObject);
            });

            GetComponent<Slider>().Dismiss(onComplete: () => {
                LeanTween.cancel(gameObject);
                Destroy(gameObject);
            });
        }

        public void Show()
        {
            
            fader = Instantiate(faderPrefab, transform.parent);
            fader.transform.SetSiblingIndex(transform.GetSiblingIndex());
            fader.Show(0.2f);
        }

        private void Awake()
        {
            Show();
        }
    }
}