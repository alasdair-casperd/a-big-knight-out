using UnityEngine;

namespace UI
{
    public class RotationIndicator: MonoBehaviour
    {
        private static Vector3 offset = new(0, 0.5f, 0);
        
        public void Configure(Vector2Int position, int rotation, EntityType? entityType, bool animateRotation)
        {
            transform.position = GridUtilities.GridToWorldPos(position) + offset;

            float rotationOffset = 0;
            if (entityType != null && entityType == EntityType.Bishop) rotationOffset = 45;
            Quaternion targetRotation = Quaternion.Euler(0, 90 * rotation + rotationOffset, 0);

            LeanTween.rotate(gameObject, targetRotation.eulerAngles, animateRotation ? 0.05f : 0);
        }
    }
}