using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demo {
    public class Player : MonoBehaviour
    {
        public float jumpHeight;

        private Vector3 startingPosition;

        [HideInInspector]
        public Tile startingTile;

        [HideInInspector]
        public Tile currentTile;

        private void Start()
        {
            startingPosition = transform.position;
        }

        // Reset the player to its starting position
        public void Reset() 
        {
            Move(startingTile, false);
        }

        // Move to a tile
        public void Move(Tile tile, bool asTurn = true)
        {
            Move(tile.transform.position);
            if (asTurn && currentTile != null) currentTile.HandlePlayerLeave();
            currentTile = tile;
        }

        public void ManualMove(Vector3 targetPosition, bool withAnimation)
        {
            Move(targetPosition, withAnimation);
        }

        // Move to a position
        private void Move(Vector3 targetPosition, bool withAnimation = true)
        {
            float duration = GameManager.AnimationTime;
            targetPosition.y = 0.5f;
            LeanTween.cancel(gameObject); 

            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            Vector3 direction = (targetPosition - startPosition).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float t) => {
                float targetHeight = 0.5f;
                if (withAnimation) targetHeight = startPosition.y + jumpHeight * Mathf.Sin(Mathf.PI * t);
                Vector3 interpolatedPosition = Vector3.Lerp(startPosition, targetPosition, t);
                interpolatedPosition.y = targetHeight;
                transform.SetPositionAndRotation(interpolatedPosition, Quaternion.Slerp(startRotation, targetRotation, t));
            });
        }
    }
}
