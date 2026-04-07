using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;
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

    public int[] GetTriangles()
    {
        return _mesh.triangles.ToArray();
    }

    public Vector3[] GetVertices()
    {
        return _mesh.vertices;
    }

    public List<List<Vector3>> GetAllTriangles()
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

    public Mesh BuildMesh(Vector3[] vertices)
    {
        int[] meshTriangles = new int[vertices.Count()];

        for (int i = 0; i < vertices.Count(); i++)
            meshTriangles[i] = i;

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
        mesh.triangles = new int[3] { 0, 1, 2 };

        _meshCache.Add(
            meshHash,
            mesh
        );

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    public void BuildGameObject(Mesh mesh, Material[] materials, out GameObject newObj, bool isActiveByDefault = true)
    {
        GameObject go = new($"{gameObject.name} Fraction");
        go.SetActive(isActiveByDefault);

        Vector3 currGoPos = go.transform.position;
        Vector3 targetGoPos = currGoPos + gameObject.transform.position;

        go.transform.position = targetGoPos;
        go.transform.rotation = gameObject.transform.rotation;
        go.transform.localScale = gameObject.transform.localScale;

        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        renderer.materials = materials;

        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.mass = Random.Range(1.0f, 3.0f);
        rb.useGravity = _useFractionGravity;
        rb.sleepThreshold = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        if (_addBoxColliderToFractionObjects)
        {
            BoxCollider boxCollider = go.AddComponent<BoxCollider>();
            boxCollider.size = mesh.bounds.size / 2;
        }


        MeshFraction meshFraction = go.AddComponent<MeshFraction>();

        newObj = go;

        _meshFractionObjects.Add(go);
    }

    private void BuildAllMeshFractions()
    {
        List<List<Vector3>> allTriangles = GetAllTriangles();

        for (int i = 0; i < allTriangles.Count; i+=_trianglesPerFraction)
        {
            var chunk = allTriangles
                .Skip(i)
                .Take(8)
                .SelectMany(t => t)
                .ToArray();

            Mesh mesh = BuildMesh(chunk);
            Material[] materials = GetComponent<MeshRenderer>().materials;
            BuildGameObject(mesh, _renderer.materials, out _, true);
        }
    }

    public void TriggerFractureExplosions()
    {
        BuildAllMeshFractions(); 
        Debug.Log($"[VERTICE RETRIEVER] Avoided building {_duplicateCount} meshes due to duplicate occurences -");
        
        foreach (var obj in _meshFractionObjects)
            obj.GetComponent<MeshFraction>().Explosion(_explosionForce);
            
        Destroy(gameObject);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     List<List<Vector3>> bounds = GetAllTriangles();

    //     foreach (var bound in bounds)
    //     {
    //         Gizmos.DrawLineStrip(bound.ToArray(), false);
    //     }
    // }

}
