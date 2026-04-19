using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
public class EnemyController : MonoBehaviour
{
    private RayCastDetector _raycastDetector;
    [SerializeField] private BoolVariable _isPlayerInSight;
    [SerializeField] private float _viewDistance; 

    private void Awake()
    {
        _raycastDetector = GetComponent<RayCastDetector>();
    }

    private void Update()
    {
        bool isPlayerInSight = _raycastDetector.Check(transform.position, transform.forward, out RaycastHit hit, _viewDistance);

        if (!isPlayerInSight)
        {
            _isPlayerInSight.SetValue(false);
            return;
        }
            
        if (hit.collider == null) 
            return;
        
        _isPlayerInSight.SetValue(true);
    }
}   
