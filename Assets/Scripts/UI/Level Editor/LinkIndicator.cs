
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class LinkIndicator : MonoBehaviour
    {
        [SerializeField]
        private GameObject arrowHead;

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

        /// <summary>
        /// Dynamically generate line renderer points following a parabolic curve
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public void SetPositions(Vector3 startPosition, Vector3 endPosition)
        {
            Vector3 perpendicular = Quaternion.Euler(0, -90, 0) * (endPosition - startPosition);
            List<Vector3> positions = new();

            float Curve(float t) => 4 * t * (1 - t);

            // Add curved positions
            int n = lineRenderer.positionCount - 1;
            for (int i = 0; i <= n; i++)
            {
                float t = ((float) i) / ((float) n);
                positions.Add(Vector3.Lerp(startPosition, endPosition, t) + offset + 0.05f * Curve(t) * perpendicular);
            }

            lineRenderer.SetPositions(positions.ToArray());
            
            // Add arrow head
            arrowHead.transform.position = endPosition + offset;
            Vector3 arrowHeadDirection = endPosition - positions[n - 1];
            arrowHeadDirection.y = 0;
            arrowHead.transform.rotation = Quaternion.LookRotation(arrowHeadDirection);
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