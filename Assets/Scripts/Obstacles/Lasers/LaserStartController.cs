using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls a laser beam visual and logic, utilizing a <see cref="LineRenderer"/> for rendering 
/// and a <see cref="RayCastDetector"/> for hit detection and interaction.
/// </summary>
/// <remarks>
/// This script handles dynamic repositioning of the laser and deals damage to objects 
/// implementing the <see cref="IShootable"/> interface.
/// </remarks>
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(LineRenderer))]
public class LaserStartController : MonoBehaviour
{
    [Tooltip("The destination transform the laser aims towards.")]
    [SerializeField] private Transform _laserEnd;

    [Tooltip("Minimum movement distance required to trigger a repositioning logic update.")]
    [SerializeField] private float _laserRepositionDistance = 0.5f;

    [Tooltip("Amount of damage dealt per hit to IShootable objects.")]
    [SerializeField] private float _damageToDeal = 10.0f;

    [SerializeField] private UnityEvent OnLaserObjectInitialize;

    private LineRenderer _lineRenderer;
    private RayCastDetector _rayCastDetector;
    private Vector3 _lastEndPosition;
    private Vector3 _lastStartPosition;
    private bool _hasHitResetBeenValidated = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _rayCastDetector = GetComponent<RayCastDetector>();

        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = 0;
        }
    }

    /// <summary>
    /// Sets initial positions and plays the activation sound via <see cref="AudioManager"/>.
    /// </summary>
    private void Start()
    {
        if (_laserEnd == null)
            return;

        SetCurrentEndPosition();
        SetCurrentStartPosition();

        RefreshLinePositions(_lastStartPosition, _lastEndPosition);

        OnLaserObjectInitialize?.Invoke();
    }

    /// <summary>
    /// Performs frame-rate independent updates for physics-based hit detection and movement tracking.
    /// </summary>
    private void FixedUpdate()
    {
        if (_laserEnd == null)
            return;

        HandleLaserChecks(_lastStartPosition, _lastEndPosition, out RaycastHit hit);

        // Handle collision case
        if (hit.collider != null)
        {
            _hasHitResetBeenValidated = false;
            RefreshLinePositions(_lastStartPosition, hit.point);
            return;
        }

        // Reset laser to default length if no collision occurs
        if (!_hasHitResetBeenValidated)
        {
            RefreshLinePositions(_lastStartPosition, _lastEndPosition);
            _hasHitResetBeenValidated = true;
        }

        // Optimization: Exit if repositioning thresholds are not met
        if (_laserRepositionDistance <= 0.0f)
            return;

        if (!(HasLaserEndMoved() || HasLaserStartMoved()))
            return;

        RefreshLinePositions(_lastStartPosition, _lastEndPosition);
        SetCurrentEndPosition();
        SetCurrentStartPosition();
    }

    /// <summary>
    /// Executes raycasting and applies damage to <see cref="IShootable"/> targets.
    /// </summary>
    /// <param name="start">Origin of the laser.</param>
    /// <param name="end">Intended target end point.</param>
    /// <param name="hit">Output collision data from <see cref="RayCastDetector.Check(Vector3, Vector3, out RaycastHit, float)"/>.</param>
    private void HandleLaserChecks(Vector3 start, Vector3 end, out RaycastHit hit)
    {
        Vector3 direction = (end - start).normalized;
        float distance = (start - end).magnitude;

        if (!_rayCastDetector.Check(start, direction, out hit, distance))
            return;

        if (hit.collider == null)
            return;

        if (hit.collider.gameObject.TryGetComponent(out IShootable shootable))
        {
            shootable.Hit(_damageToDeal, hit.point);
        }
    }

    /// <summary>
    /// Calculates if the target transform has moved beyond <see cref="_laserRepositionDistance"/>.
    /// </summary>
    private bool HasLaserEndMoved()
    {
        return Vector3.Distance(_lastEndPosition, _laserEnd.position) >= _laserRepositionDistance;
    }

    /// <summary>
    /// Calculates if the origin transform has moved beyond <see cref="_laserRepositionDistance"/>.
    /// </summary>
    private bool HasLaserStartMoved()
    {
        return Vector3.Distance(_lastStartPosition, transform.position) >= _laserRepositionDistance;
    }

    /// <summary>
    /// Updates the cached end position.
    /// </summary>
    private void SetCurrentEndPosition()
    {
        if (_laserEnd != null)
        {
            _lastEndPosition = _laserEnd.position;
        }
    }

    /// <summary>
    /// Updates the cached start position.
    /// </summary>
    private void SetCurrentStartPosition()
    {
        _lastStartPosition = transform.position;
    }

    /// <summary>
    /// Updates the <see cref="LineRenderer"/> vertices.
    /// </summary>
    private void RefreshLinePositions(Vector3 start, Vector3 end)
    {
        if (_lineRenderer.positionCount != 2)
        {
            _lineRenderer.positionCount = 2;
        }

        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }
}