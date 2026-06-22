using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class Climber : MonoBehaviour
{
    private Vector3 FindSuitableClimbPosition(RaycastHit hit)
    {
        float xPos = hit.point.x;
        float yPos = hit.collider.bounds.max.y + transform.lossyScale.y / 2;
        float zPos = hit.point.z;

        Vector3 climbPos = new Vector3(
            xPos,
            yPos,
            zPos
        );

        return climbPos;
    }

    public bool CanClimb(
        Vector3 origin,
        Vector3 direction,
        float range,
        float maxClimbHeight,
        float minClimbHeight,
        out Vector3 climbPos,
        float pushDistance = 0.0f
    )
    {
        RaycastHit climbHit = FindClimbablePoint(origin, direction, range);

        climbPos = origin;

        if (climbHit.collider == null)
            return false;

        Vector3 tempClimbPos = FindSuitableClimbPosition(climbHit) + (direction * pushDistance);

        float heightDifference = tempClimbPos.y - origin.y;

        if (heightDifference > maxClimbHeight)
            return false;

        if (heightDifference < minClimbHeight)
            return false;

        climbPos = tempClimbPos;
        return true;
    }

    private RaycastHit FindClimbablePoint(Vector3 origin, Vector3 direction, float range)
    {
        Physics.Raycast(origin, direction, out RaycastHit hit, range);
        return hit;
    }
}
