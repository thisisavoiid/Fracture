using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
public class NavMeshBaker : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface _navMeshSurface;

    [Button]
    public void BakeNavMesh()
    {
        if (_navMeshSurface == null)
        {
            Debug.LogError("[NAV MESH BAKER] Couldn't bake nav mesh as the assigned nav mesh surface was null! -");
            return;
        }
            
        _navMeshSurface.BuildNavMesh();
    }
}