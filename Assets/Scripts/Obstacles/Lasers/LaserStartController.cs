using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(LineRenderer))]
public class LaserStartController : MonoBehaviour
{
    [SerializeField] private Transform _laserEnd;
    [SerializeField] private float _laserRepositionDistance = 0.5f;
    [SerializeField] private float _damageToDeal = 10.0f;
    [SerializeField] private Sound _sound;
    private LineRenderer _lineRenderer;
    private RayCastDetector _rayCastDetector;
    private Vector3 _lastEndPosition;
    private Vector3 _lastStartPosition;
    private bool _hasHitResetBeenValidated = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _rayCastDetector = GetComponent<RayCastDetector>();

        if (_lineRenderer == null)
            return;

        _lineRenderer.positionCount = 0;
    }

    private void Start()
    {
        if (_laserEnd == null)
            return;

        SetCurrentEndPosition();
        SetCurrentStartPosition();

        RefreshLinePositions(_lastStartPosition, _lastEndPosition);

        AudioManager.Instance.PlaySound(_sound, gameObject.transform);
    }

    private void FixedUpdate()
    {
        if (_laserEnd == null)
            return;

        HandleLaserChecks(_lastStartPosition, _lastEndPosition, out RaycastHit hit);

        if (hit.collider != null)
        {
            _hasHitResetBeenValidated = false;
            RefreshLinePositions(_lastStartPosition, hit.point);
            
            Debug.Log($"[LASER START CONTROLLER] {gameObject.name} hit object: {hit.collider.name} at {hit.point} -");
            return;
        }

        if (!_hasHitResetBeenValidated)
        {
            RefreshLinePositions(_lastStartPosition, _lastEndPosition);
            _hasHitResetBeenValidated = true;
            
            Debug.Log($"[LASER START CONTROLLER] {gameObject.name} laser reset to default end position -");
        }

        if (_laserRepositionDistance <= 0.0f)
            return;

        if (!(HasLaserEndMoved() || HasLaserStartMoved()))
            return;

        RefreshLinePositions(_lastStartPosition, _lastEndPosition);
        SetCurrentEndPosition();
        SetCurrentStartPosition();
        
        Debug.Log($"[LASER START CONTROLLER] {gameObject.name} repositioned due to movement -");
    }

    private void HandleLaserChecks(Vector3 start, Vector3 end, out RaycastHit hit)
    {
        Vector3 direction = (end - start).normalized;
        float distance = (start - end).magnitude;

        if (!_rayCastDetector.Check(start, direction, out hit, distance))
            return;

        if (hit.collider == null)
            return;

        if (!hit.collider.gameObject.TryGetComponent<IShootable>(out IShootable shootable))
            return;

        shootable.Hit(_damageToDeal, hit.point);
    }

    private bool HasLaserEndMoved()
    {
        float distanceToLastPoint = (_lastEndPosition - _laserEnd.position).magnitude;

        if (distanceToLastPoint < _laserRepositionDistance)
            return false;

        return true;
    }

    private bool HasLaserStartMoved()
    {
        float distanceToLastPoint = (_lastStartPosition - transform.position).magnitude;

        if (distanceToLastPoint < _laserRepositionDistance)
            return false;

        return true;
    }

    private void SetCurrentEndPosition()
    {
        if (_laserEnd == null)
            return;

        _lastEndPosition = _laserEnd.position;
    }

    private void SetCurrentStartPosition()
    {
        _lastStartPosition = transform.position;
    }

    private void RefreshLinePositions(Vector3 start, Vector3 end)
    {
        if (_lineRenderer.positionCount != 2)
            _lineRenderer.positionCount = 2;

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }
}