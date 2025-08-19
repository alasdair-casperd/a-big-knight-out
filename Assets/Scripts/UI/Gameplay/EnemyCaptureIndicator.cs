using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Used to indicate squares that the player can move to
    /// </summary>
    public class EnemyCaptureIndicator : MonoBehaviour
    {
        /// <summary>
        /// The time taken for the indicator to appear or disappear
        /// </summary>
        public float transitionDuration = 0.05f;

        /// <summary>
        /// Hides the move indicator with an animation
        /// </summary>
        public void Hide()
        {
            // LeanTween.value(gameObject, 1, 0, transitionDuration)
            // .setOnUpdate(t => transform.localScale = new Vector3(t, 1, t))
            // .setOnComplete(() => gameObject.SetActive(false));

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Reveals the move indicator with an animation
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            // LeanTween.value(gameObject, 0, 1, transitionDuration)
            // .setOnUpdate(t => transform.localScale = new Vector3(t, 1, t));
        }
    }
}
