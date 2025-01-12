using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    /// <summary>
    /// A class to draw a grid mesh in the scene
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class GridMesh : MonoBehaviour
    {
        public int GridSize;
        public Color GridColor;

        void Awake()
        {
            DrawGrid();
        }

        void DrawGrid()
        {
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();        
            var mesh = new Mesh();
            var vertices = new List<Vector3>();

            // Define grid vertices and indices
            var indices = new List<int>();
            for (int i = 0; i < GridSize; i++)
            {
                vertices.Add(new Vector3(i, 0, 0));
                vertices.Add(new Vector3(i, 0, GridSize));

                indices.Add(4 * i + 0);
                indices.Add(4 * i + 1);

                vertices.Add(new Vector3(0, 0, i));
                vertices.Add(new Vector3(GridSize, 0, i));

                indices.Add(4 * i + 2);
                indices.Add(4 * i + 3);
            }

            // Create the grid
            mesh.vertices = vertices.ToArray(); 
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            filter.mesh = mesh;
            
            // Set the material
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = GridColor
            };

            // Offset to center at (0,0)
            transform.position = new Vector3(- (float) GridSize / 2, 0, - (float) GridSize / 2);
    }
    }
}