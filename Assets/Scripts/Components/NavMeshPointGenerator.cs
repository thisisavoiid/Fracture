
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPointGenerator : MonoBehaviour
{
    public Vector3 FindPosition(float radius)
    {
        bool hasFoundPosition = NavMesh.SamplePosition(
            Random.insideUnitSphere * radius,
            out NavMeshHit hit,
            radius,
            NavMesh.AllAreas
        );

        if (!hasFoundPosition)
            return Vector3.zero;
        
        return hit.position;
    }
}