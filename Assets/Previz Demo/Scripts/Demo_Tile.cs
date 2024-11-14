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

        public bool moving = false;
        private int stepCount = 0;

        // Transforms defining the path the tile will follow
        // The first transform should be the tile's initial position
        public List<Transform> targets;

        private void Start()
        {
            initialScale = transform.localScale;
            gameManager = FindAnyObjectByType<GameManager>();
        }

        private void Update()
        {
            // Show a highlight if the tile constitutes a valid player move
            highlight.SetActive(gameObject && !gameManager.movementLocked && gameManager.IsValidMove(this));
        }

        public void Reset()
        {
            destroyed = false;
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, initialScale, GameManager.AnimationTime);

            if (moving)
            {
                stepCount = 0;
                Move(targets[0]);
            }
        }

        // Called when the player leaves the tile
        public void HandlePlayerLeave()
        {
            // Destroy the tile, if appropriate
            if (singleUse && !destroyed) {
                destroyed = true;
                LeanTween.cancel(gameObject);
                LeanTween.scale(gameObject, destroyedScale, GameManager.AnimationTime);
            }
        }

        public void Step()
        {
            if (moving)
            {
                stepCount++;
                Move(targets[stepCount % targets.Count]);
            }
        }
        
        // Move to a transform
        private void Move(Transform targetTransform)
        {
            var targetPosition = targetTransform.position;
            targetPosition.y = transform.position.y;
            
            Player player = FindAnyObjectByType<Player>();
            
            if (player.currentTile == this)
            {
                player.ManualMove(targetPosition, false);
            }

            LeanTween.move(gameObject, targetPosition, GameManager.AnimationTime);
        }
    }
}
