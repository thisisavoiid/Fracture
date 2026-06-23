using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

/// <summary>
/// A custom plane mesh generator.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshAssetSaver))]
public class PlaneMeshGenerator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Settings of the plane mesh generator.")]
    private PlaneMeshGeneratorSettings _settings = new();

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

        DateTime startTime = DateTime.Now;

        List<Vector3> vertices = GenerateVertices(_settings, out List<Vector2> uv);
        List<int> triangles = GenerateTriangles(vertices);

        if (vertices.Count == 0 || triangles.Count == 0 || uv.Count == 0)
        {
            Debug.LogError($"[PLANE MESH GENERATOR] An issue occured while generating vertices and triangles! -");
            return;
        }

        // This part actually creates a new Mesh object and assigns all the values needed to it.
        Mesh mesh = new();

        mesh.name = _settings.Name;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        // Since I'm too lazy to calculate normals, bounds and tangents myself (and frankly, I forgot how to...)
        // I'm leaving it up to Unity and hope for the best.
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        _settings.MeshFilter.sharedMesh = mesh;

        DateTime endTime = DateTime.Now;
        TimeSpan timeUsed = endTime-startTime;

        Debug.Log($"[PLANE MESH GENERATOR] Finished building the mesh. Process took {timeUsed.TotalMilliseconds}ms. -");
    }

    private List<Vector3> GenerateVertices(PlaneMeshGeneratorSettings settings, out List<Vector2> uv)
    {
        Debug.Log("[PLANE MESH GENERATOR] Generating vertices... -");

        if (
            settings.Size.x % settings.Resolution != 0 ||
            settings.Size.y % settings.Resolution != 0
        )
        {
            Debug.LogWarning($"[PLANE MESH GENERATOR] Entering values so their remainder is not equal to zero might cause the mesh to be slightly offset. -");
        }

        // This calculates the step distance between each iteration.
        float xStep = (float)settings.Size.x / settings.Resolution;
        float zStep = (float)settings.Size.y / settings.Resolution;

        List<Vector3> vertices = new();
        uv = new();

        for (int x = 0; x < settings.Resolution; x++)
        {
            for (int z = 0; z < settings.Resolution; z++)
            {
                // Creates 4 vertices with local offsets => One vertex per quad corner!
                // I'm multiplying the index of the current iteration (for both x and z axis) with the associated step distance
                // in order to sort of "add" the distance and thus "move" the vertex. When applying offsets for the quad corner vertices,
                // I'm simply adding "1" to the current iteration index which results in a offset which is being influenced by the 
                // step distance.
                Vector3 vertex01 = new Vector3(x * xStep, 0, z * zStep);
                Vector3 vertex02 = new Vector3(x * xStep, 0, (z + 1) * zStep);
                Vector3 vertex03 = new Vector3((x + 1) * xStep, 0, (z + 1) * zStep);
                Vector3 vertex04 = new Vector3((x + 1) * xStep, 0, z * zStep);

                // Adds all vertices to the vertex list
                vertices.Add(vertex01);
                vertices.Add(vertex02);
                vertices.Add(vertex03);
                vertices.Add(vertex04);

                // This part generates UV coordinates based on the vertices position and the 
                // specified plane dimensions.
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

        // Generated triangle indices by adding them to a new int array which serves as 
        // an index list for each quads two triangle indices. Since I know what order I'm adding the 
        // vertex positions, I just go ahead and use the individual indices in the vertex array.
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

    /// <summary>
    /// Saves the currently active mesh selected in the specified mesh filter component.
    /// </summary>
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