using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using System.Linq;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshAssetSaver))]
public class PlaneMeshGenerator : MonoBehaviour
{
    [SerializeField] private PlaneMeshGeneratorSettings _settings = new();

    [Button]
    public void BuildMesh()
    {
        if (_settings.MeshFilter == null)
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] You must set a mesh filter component in the settings in order to generate any meshes! -");
            return;
        }

        if (_settings.Size.x < 1 || _settings.Size.y < 1)
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] Neither y nor x size can be smaller than 1! -");
            return;
        }

        if (_settings.Resolution < 1)
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] Mesh resolution mustn't be less than 1! -");
            return;
        }

        Debug.Log("[PLANE MESH GENERATOR] Starting generation... -");

        List<Vector3> vertices = GenerateVertices(_settings, out List<Vector2> uv);
        List<int> triangles = GenerateTriangles(vertices);

        if (vertices.Count == 0 || triangles.Count == 0 || uv.Count == 0)
        {
            Debug.LogError($"[PLANE MESH GENERATOR] An issue occured while generating vertices and triangles! -");
            return;
        }

        Mesh mesh = new();

        mesh.name = _settings.Name;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();


        _settings.MeshFilter.sharedMesh = mesh;
    }

    private List<Vector3> GenerateVertices(PlaneMeshGeneratorSettings settings, out List<Vector2> uv)
    {
        Debug.Log("[PLANE MESH GENERATOR] Generating vertices... -");

        List<Vector3> vertices = new();
        uv = new();

        for (float x = 0; x < settings.Size.x; x += 1.0f / settings.Resolution)
        {
            for (float z = 0; z < settings.Size.y; z += 1.0f / settings.Resolution)
            {
                Vector3 vertex01 = new Vector3(x, 0, z);
                Vector3 vertex02 = new Vector3(x, 0, z + (1.0f / settings.Resolution));
                Vector3 vertex03 = new Vector3(x + (1.0f / settings.Resolution), 0, z + (1.0f / settings.Resolution));
                Vector3 vertex04 = new Vector3(x + (1.0f / settings.Resolution), 0, z);

                vertices.Add(vertex01);
                vertices.Add(vertex02);
                vertices.Add(vertex03);
                vertices.Add(vertex04);

                uv.Add(
                    new Vector2(
                        vertex01.x / _settings.Size.x, vertex01.z / _settings.Size.y
                    )
                );

                uv.Add(
                    new Vector2(
                        vertex02.x / _settings.Size.x, vertex02.z / _settings.Size.y
                    )
                );

                uv.Add(
                    new Vector2(
                        vertex03.x / _settings.Size.x, vertex03.z / _settings.Size.y
                    )
                );

                uv.Add(
                    new Vector2(
                        vertex04.x / _settings.Size.x, vertex04.z / _settings.Size.y
                    )
                );
            }
        }

        Debug.Log($"[PLANE MESH GENERATOR] {vertices.Count} vertices generated! -");
        return vertices;
    }

    private List<int> GenerateTriangles(List<Vector3> vertices)
    {
        Debug.Log("[PLANE MESH GENERATOR] Generating triangles... -");

        List<int> triangles = new();

        for (int i = 0; i <= vertices.Count - 4; i += 4)
        {
            int[] triangle = new int[6];
            triangle[5] = i + 2;    // Upper Right    
            triangle[4] = i + 1;    // Upper Left
            triangle[3] = i;        // Bottom left
            triangle[2] = i;        // Bottom left
            triangle[1] = i + 3;    // Bottom right
            triangle[0] = i + 2;    // Upper Right

            foreach (int index in triangle)
                triangles.Add(index);
        }

        Debug.Log($"[PLANE MESH GENERATOR] {triangles.Count} triangles generated! -");
        return triangles;
    }

    [Button]
    public void SaveMeshAsset()
    {
        if (_settings.MeshFilter == null)
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] You must set a mesh filter component in the settings in order to save any meshes! -");
            return;
        }

        if (_settings.MeshFilter.sharedMesh == null)
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] There's no valid mesh applied to the mesh filter component! -");
            return;
        }

        if (!TryGetComponent(out MeshAssetSaver meshAssetSaver))
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] There's no mesh asset saver component on this object! -");
            return;
        }
        else
        {
            Mesh mesh = _settings.MeshFilter.sharedMesh;
            meshAssetSaver.SaveMesh(mesh, _settings.Path, _settings.Name);
        }
    }
}