
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// A component which, when present in a scene, lets the player pan in the x-z plane with the mouse
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// The last known position of the mouse during a scroll action
        /// </summary>
        private Vector3 lastMousePosition;

        /// <summary>
        /// Should the right mouse button be used for scrolling?
        /// </summary>
        public bool useRightClick = true;

        /// <summary>
        /// Should the middle mouse button be used for scrolling?
        /// </summary>
        public bool useMiddleClick = true;

        /// <summary>
        /// The maximum starting distance from the player
        /// </summary>
        public float maxPlayerDistance = 3f;

        /// <summary>
        /// Move the camera to a sensible starting position.
        /// </summary>
        /// <param name="playerController"></param>
        /// <param name="squares"></param>
        public void InitialiseCameraPosition(PlayerController playerController, Dictionary<Vector2Int, Square> squares)
        {
            Debug.Log("Initialising camera position");

            int minX = int.MaxValue;
            int maxY = int.MinValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;

            // Compute level bounds
            foreach (var square in squares.Values)
            {
                if (square.Position.x < minX) minX = square.Position.x;
                if (square.Position.x > maxX) maxX = square.Position.x;
                if (square.Position.y < minY) minY = square.Position.y;
                if (square.Position.y > maxY) maxY = square.Position.y;
            }

            // Convert level center to world position
            var levelCenter = GridUtilities.GridToWorldPos(new Vector2Int(
                (int)Math.Round(0.5f * (minX + maxX)),
                (int)Math.Round(0.5f * (minY + maxY))));

            Debug.Log("Level center x: " + levelCenter.x);
            Debug.Log("Level center y: " + levelCenter.y);

            transform.position = levelCenter;
            if (playerController == null) return;

            // Clamp max distance of camera from player
            Vector3 toPlayer = playerController.transform.position - levelCenter;
            if (toPlayer.magnitude > maxPlayerDistance)
            {
                transform.position = playerController.transform.position - 3 * toPlayer.normalized;
            }
        }

        private void Update()
        {
            if (GameManager.Paused) return;

            // Start scroll
            if ((useRightClick && Input.GetMouseButtonDown(1)) || (useMiddleClick && Input.GetMouseButtonDown(2)))
            {
                lastMousePosition = MouseUtilities.GetMouseWorldPos();
            }

            // Continue scroll
            else if ((useRightClick && Input.GetMouseButton(1)) || (useMiddleClick && Input.GetMouseButton(2)))
            {
                Vector3 currentMousePosition = MouseUtilities.GetMouseWorldPos();
                Vector3 pan = currentMousePosition - lastMousePosition;
                pan.y = 0;
                transform.position -= pan;
            }
        }
    }
}