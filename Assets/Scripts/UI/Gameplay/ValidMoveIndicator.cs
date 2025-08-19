using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Used to indicate squares that the player can move to
    /// </summary>
    public class ValidMoveIndicator : MonoBehaviour
    {
        /// <summary>
        /// The time taken for the indicator to appear or disappear
        /// </summary>
        public float transitionDuration = 0.05f;

        public float hoverOpacity = 0.5f;
        public float defaultOpacity = 0.1f;

        [SerializeField] private SpriteRenderer sprite;

        /// <summary>
        /// Hides the move indicator with an animation
        /// </summary>
        public void Hide()
        {
            if (!gameObject.activeSelf) { return; }
            LeanTween.value(gameObject, 1, 0, transitionDuration)
            .setOnUpdate(t => transform.localScale = new Vector3(t, 1, t))
            .setOnComplete(() => gameObject.SetActive(false));
        }

        /// <summary>
        /// Reveals the move indicator with an animation
        /// </summary>
        public void Show()
        {
            SetHoverState(false);
            gameObject.SetActive(true);
            transform.localScale = new Vector3(1, 1, 1);
            LeanTween.value(gameObject, 0, 1, transitionDuration)
            .setOnUpdate(t => transform.localScale = new Vector3(t, 1, t));
        }

        private void OnMouseEnter()
        {
            if (!GameManager.Paused) SetHoverState(true);
        }

        private void OnMouseExit()
        {
            if (!GameManager.Paused) SetHoverState(false);
        }

        private void SetHoverState(bool hovered)
        {
            var color = sprite.color;
            color.a = hovered ? hoverOpacity : defaultOpacity;
            sprite.color = color;
        }
    }
}
