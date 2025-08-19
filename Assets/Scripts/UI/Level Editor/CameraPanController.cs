
using UnityEngine;

namespace UI
{
    /// <summary>
    /// A component which, when present in a scene, lets the player pan in the x-z plane with the mouse
    /// </summary>
    public class CameraPanController : MonoBehaviour
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
                Camera.main.transform.position -= pan;
                lastMousePosition = MouseUtilities.GetMouseWorldPos();
            }
        }
    }
}