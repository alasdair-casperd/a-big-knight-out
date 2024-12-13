
using UnityEngine;

namespace UI
{
    public class LinkIndicator : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private Vector3 offset = new(0, 0.5f, 0);

        [SerializeField]
        private Material highlightMaterial;
        private Material defaultMaterial;

        public Vector2Int Start { get; protected set; }
        public Vector2Int End { get; protected set; }

        public bool MouseOver { get; protected set; }
        
        public void InitialiseAsPreview(Vector3 startPosition, Vector3 endPosition)
        {
            transform.position = Vector3.zero;

            SetPositions(startPosition, endPosition);
        }

        public void InitialiseAsInteractiveLink(Vector2Int startPosition, Vector2Int endPosition)
        {
            Start = startPosition;
            End = endPosition;
            
            transform.position = Vector3.zero;

            SetPositions(GridUtilities.GridToWorldPos(startPosition), GridUtilities.GridToWorldPos(endPosition));

            // Store default material
            defaultMaterial = lineRenderer.material;
            
            // Create mesh collider for mouse over detection
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            Mesh mesh = new();
            lineRenderer.BakeMesh(mesh, true);
            meshCollider.sharedMesh = mesh;
        }

        public void SetPositions(Vector3 startPosition, Vector3 endPosition)
        {
            // Calculate center handle position
            Vector3 middlePosition = (startPosition + endPosition) / 2 + 0.2f * (Quaternion.Euler(0, -90, 0) * (endPosition - startPosition));

            // Configure line renderer points
            lineRenderer.SetPositions(new Vector3[] { startPosition + offset, middlePosition + offset, endPosition + offset });
        }

        private void OnMouseEnter()
        {
            MouseOver = true;
            lineRenderer.material = highlightMaterial;
        }

        private void OnMouseExit()
        {
            MouseOver = false;
            lineRenderer.material = defaultMaterial;
        }
    }
}