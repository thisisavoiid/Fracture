using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VerticeRetriever : MonoBehaviour
{
    private Mesh _mesh;
    private int _duplicateCount = 0;
    private List<GameObject> _meshFractionObjects = new();
    private Dictionary<int, Mesh> _meshCache = new();
    [SerializeField] private float _explosionForce;
    [SerializeField] private bool _useFractionGravity;
    [SerializeField] private bool _addBoxColliderToFractionObjects;
    [SerializeField] private int _trianglesPerFraction;

    private MeshRenderer _renderer;
    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _renderer = GetComponent<MeshRenderer>();
    }

    private int[] GetTriangles()
    {
        return _mesh.triangles.ToArray();
    }

    private Vector3[] GetVertices()
    {
        return _mesh.vertices;
    }

    private List<List<Vector3>> GetAllTriangles()
    {
        int[] triangles = GetTriangles();
        Vector3[] vertices = GetVertices();
        List<List<Vector3>> triangleBounds = new();

        for (int i = 0; i < triangles.Length - 2; i += 3)
        {
            // triangleBounds.Add(new Vector3[3] { vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]] }.ToList());

            triangleBounds.Add(new List<Vector3>
            {
                vertices[triangles[i]],
                vertices[triangles[i+1]],
                vertices[triangles[i+2]]
            });
        }

        return triangleBounds;
    }

    private Mesh BuildMesh(Vector3[] vertices)
    {
        int meshHash = 16;

        for (int i = 0; i < vertices.Length; i++)
            meshHash *= vertices[i].GetHashCode();

        if (_meshCache.ContainsKey(meshHash))
        {
            _duplicateCount += 1;
            return _meshCache[meshHash];
        }

        Mesh mesh = new();

        mesh.vertices = vertices;
        mesh.triangles = Enumerable.Range(0, vertices.Length).ToArray(); // Outputs a new array with integers sorted in ascending order

        _meshCache.Add(
            meshHash,
            mesh
        );

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void BuildGameObject(Mesh mesh, Material[] materials, out GameObject newObj, bool isActiveByDefault = true)
    {
        GameObject newGo = new($"{gameObject.name} Fraction");
        newGo.SetActive(isActiveByDefault);

        newGo.transform.position = gameObject.transform.position;
        newGo.transform.rotation = gameObject.transform.rotation;
        newGo.transform.localScale = gameObject.transform.localScale;

        MeshRenderer renderer = newGo.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = materials;

        MeshFilter filter = newGo.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        Rigidbody rb = newGo.AddComponent<Rigidbody>();
        rb.mass = Random.Range(1.0f, 3.0f);
        rb.useGravity = _useFractionGravity;
        rb.sleepThreshold = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        if (_addBoxColliderToFractionObjects)
        {
            BoxCollider boxCollider = newGo.AddComponent<BoxCollider>();
            boxCollider.size = mesh.bounds.size / 2;
        }


        MeshFraction meshFraction = newGo.AddComponent<MeshFraction>();

        newObj = newGo;

        _meshFractionObjects.Add(newGo);
    }

    private void BuildAllMeshFractions()
    {
        List<List<Vector3>> allTriangles = GetAllTriangles();

        for (int i = 0; i < allTriangles.Count; i += _trianglesPerFraction)
        {
            var chunk = allTriangles
                .Skip(i)
                .Take(_trianglesPerFraction)
                .SelectMany(t => t)
                .ToArray();

            Mesh mesh = BuildMesh(chunk);

            BuildGameObject(mesh, _renderer.sharedMaterials, out _, true);
        }
    }

    public void TriggerFractureExplosions(Vector3 origin)
    {
        BuildAllMeshFractions();
        Debug.Log($"[VERTICE RETRIEVER] Avoided building {_duplicateCount} meshes due to duplicate occurences -");

        foreach (var obj in _meshFractionObjects)
            obj.GetComponent<MeshFraction>().Explosion(_explosionForce, origin);

        Destroy(gameObject);
    }
}
