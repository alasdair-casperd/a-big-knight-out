
using UnityEngine;

/// <summary>
/// A class storing utility functions relating to the grid layout of the levels
/// </summary>
public static class MouseUtilities
{
    /// <summary>
    /// Gets the grid position the mouse is currently hovering over.
    /// </summary>
    /// <returns> The world position of the mouse. </returns>
    public static Vector3 GetMouseWorldPos()
    {
        // Casts a ray from the camera to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // create a plane at 0,0,0 whose normal points to +Y:
        // It is currently assumed that all tiles will exist on this plane
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);

        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;

        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            // get the hit point:
            return ray.GetPoint(distance);
        }
        else
        {
            Debug.LogError("No world position found for mouse position.");
            return Vector3.zero;
        }
    }
}