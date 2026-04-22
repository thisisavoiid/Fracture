using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(OverlapSphereDetector))]

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BoolVariable _isPlayerInSight;
    [SerializeField] private float _viewDistance;
    [SerializeField] private LayerMask _targetLayers;
    private RayCastDetector _raycastDetector;
    private Transform _transform;
    private OverlapSphereDetector _overlapSphereDetector;

    private void Awake()
    {
        _raycastDetector = GetComponent<RayCastDetector>();
        _overlapSphereDetector = GetComponent<OverlapSphereDetector>();
        _transform = GetComponent<Transform>();

        _overlapSphereDetector.SetRadius(_viewDistance / 2);
    }

    private Collider GetClosestCollider(Vector3 origin, List<Collider> colliders)
    {
        if (colliders == null)
            return null;

        if (colliders.Count == 0)
            return null;

        if (colliders.Count == 1)
            return colliders[0];

        IEnumerable<Collider> query = from collider in colliders
                                      orderby (collider.transform.position - _transform.position).sqrMagnitude
                                      select collider;

        return query.ToArray()[0];
    }
    private void Update()
    {
        List<Collider> foundColliders = _overlapSphereDetector.GetColliders(_targetLayers);
        Collider closestTargetCollider = GetClosestCollider(_transform.position, foundColliders);

        if (closestTargetCollider == null)
        {
            _isPlayerInSight.SetValue(false);
            return;
        }

        Transform closestTargetColliderTransform = closestTargetCollider.transform;

        Vector3 dir = closestTargetColliderTransform.position - _transform.position;

        bool isPlayerInSight = _raycastDetector.Check(_transform.position, dir, out RaycastHit hit, _viewDistance);

        if (!isPlayerInSight)
        {
            _isPlayerInSight.SetValue(false);
            return;
        }

        if (hit.collider.gameObject != closestTargetCollider.gameObject)
            return;

        _isPlayerInSight.SetValue(true);
    }
}
