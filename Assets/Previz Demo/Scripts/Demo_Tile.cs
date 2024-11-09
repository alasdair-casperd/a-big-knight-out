using System.Collections.Generic;
using UnityEngine;

namespace Demo {    
    public class Tile : MonoBehaviour
    {
        public bool singleUse;
        public bool destroyed;

        [SerializeField]
        private GameObject highlight;
        private GameManager gameManager;

        private Vector3 initialScale;
        private Vector3 destroyedScale = Vector3.one * 0.1f;

        private void Start()
        {
            initialScale = transform.localScale;
            gameManager = FindAnyObjectByType<GameManager>();
        }

        private void Update()
        {
            // Show a highlight if the tile constitutes a valid player move
            highlight.SetActive(gameObject && gameManager.IsValidMove(this));
        }

        public void Reset()
        {
            destroyed = false;
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, initialScale, 0.1f);
        }

        // Called when the player leaves the tile
        public void HandlePlayerLeave()
        {
            // Destroy the tile, if appropriate
            if (singleUse && !destroyed) {
                destroyed = true;
                LeanTween.cancel(gameObject);
                LeanTween.scale(gameObject, destroyedScale, 0.1f);
            }
        }
    }
}
