using System;
using NaughtyAttributes;
using UnityEditor.EditorTools;
using UnityEngine;

[Serializable]
public class PlaneMeshGeneratorSettings
{
    [SerializeField]
    [Tooltip("Name of the mesh to be generated.")]
    private string _name = "New Mesh";

    /// <summary>
    /// Specified name of the mesh to be generated.
    /// </summary>
    public string Name => _name;

    [SerializeField] 
    [Tooltip("Size of the mesh to be generated.")]
    private Vector2Int _size = new Vector2Int(5, 5);
    /// <summary>
    /// Specified size of the mesh to be generated.
    /// </summary>
    public Vector2Int Size => _size;

    [SerializeField]
    [Tooltip("Resolution of the mesh to be generated. The higher this value, the more vertices will be generated.")]
    [Range(1, 128)]
    private int _resolution = 1;

    /// <summary>
    /// The resolution of the mesh to be generated. Determines how many vertices to generate.
    /// </summary>
    public int Resolution => _resolution;

    [SerializeField] 
    [Tooltip("The mesh filter target which is going to show the generated mesh.")]
    [Required("You must specify a mesh filter component in order to generate a mesh!")]
    private MeshFilter _meshFilter;

    /// <summary>
    /// The <see cref="MeshFilter"/> to use for mesh generation.
    /// </summary>
    public MeshFilter MeshFilter => _meshFilter;

    [SerializeField]
    [Tooltip("The path to save the asset to.")]
    private string _path;

    /// <summary>
    /// The path to save the mesh asset to.
    /// </summary>
    public string Path => _path;
}