using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;

public class DroneBrain : Spawnable
{
    [SerializeField] private OverlapSphereDetector _swarmDetector;
    [SerializeField] private OverlapSphereDetector _targetDetector;
    [SerializeField] private RayCastDetector _rayCastDetector;
    [SerializeField] private float _viewDistance;
    [SerializeField] private LayerMask _selfMask;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private Transform _target;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GunController _gunController;
    [SerializeField] private Transform _bulletOrigin;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _speed;

    private Dictionary<State, List<Transition>> _states = new();
    private State _currentState;

    public Vector3 CalculateForce()
    {
        Vector3 force = new();
        Vector3 alignment = new();
        Vector3 seperation = new();
        Vector3 cohesion = new();
        Vector3 cohesionCenter = new();
        Vector3 forceToAgent = new();

        Vector3 currentPosition = transform.position;
        Vector3 currentAgentPosition = _agent.transform.position;
        currentAgentPosition.y = currentPosition.y;

        Vector3 agentDiff = (currentAgentPosition - currentPosition);
        float agentDistance = agentDiff.magnitude;

        forceToAgent = agentDiff.normalized * Mathf.Min(agentDistance, 5.0f);

        List<Transform> transforms = GetSurroundingDroneTransforms().Where(obj => obj.gameObject != this.gameObject).ToList();
        Vector3 totalForward = new();

        if (transforms.Count > 0)
        {
            foreach (Transform objTransform in transforms)
            {
                totalForward += objTransform.forward;

                Vector3 diff = (transform.position - objTransform.position);
                float distance = diff.magnitude;

                if (distance > 0.075f)
                    seperation += diff.normalized / distance;

                cohesionCenter += objTransform.position;
            }

            alignment = totalForward / transforms.Count;
            alignment.y = 0;

            cohesionCenter = cohesionCenter / transforms.Count;
            cohesion = (cohesionCenter - transform.position).normalized;
        }

        seperation = Vector3.ClampMagnitude(seperation, 5.0f);
        forceToAgent = Vector3.ClampMagnitude(forceToAgent, 5.0f);

        force = seperation * 1.75f + cohesion * 0.5f + forceToAgent * 3f;

        return force;
    }

    private List<Transform> GetSurroundingDroneTransforms()
    {
        _swarmDetector.SetRadius(10);
        List<Collider> colliders = _swarmDetector.GetColliders(_selfMask);
        return colliders.Select(collider => collider.gameObject.transform).ToList();
    }

    public override void Spawn()
    {
        ConfigureStateMachine(); // temporary!!
    }
    private void ConfigureStateMachine()
    {
        State chaseState = new DroneChaseState(
            this,
            _rb,
            _agent,
            _target,
            _speed
        );

        State attackState = new DroneAttackState(
            this,
            _rb,
            _agent,
            _target,
            _bulletOrigin,
            _gunController,
            _speed
        );

        State idleState = new DroneIdleState(
            this,
            _rb,
            _agent,
            _speed
        );

        _states.Add(
            idleState,
            new List<Transition>
            {
                new Transition(
                    chaseState, () => CanSeePlayer()
                )
            }
        );

        _states.Add(
            chaseState,
            new List<Transition>
            {
                new Transition(
                    attackState, () => CanSeePlayer() && Vector3.Distance(_rb.position, _target.position) <= _attackDistance
                ),
                new Transition(
                    idleState, () => !CanSeePlayer() && Vector3.Distance(_rb.position, _target.position) > _attackDistance
                )
            }
        );

        _states.Add(
            attackState,
            new List<Transition>
            {
                new Transition(
                    chaseState, () => Vector3.Distance(_rb.position, _target.position) > _attackDistance
                )
            }
        );

        SetState(idleState);
    }

    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null -");
            return;
        }

        _currentState.Run();

        if (_states.TryGetValue(_currentState, out List<Transition> transitions))
        {
            foreach (Transition transition in _states[_currentState])
            {
                if (transition.Condition() == true)
                {
                    SetState(transition.TargetState);
                    break;
                }

            }
        }
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.Exit();

        _currentState = state;

        if (_currentState != null)
            _currentState.Enter();
    }

    public bool CanSeePlayer()
    {
        List<Collider> foundColliders = _targetDetector.GetColliders(_attackMask);
        Collider closestTargetCollider = GetClosestCollider(transform.position, foundColliders);

        if (closestTargetCollider == null)
            return false;

        Vector3 dir = closestTargetCollider.transform.position - transform.position;

        bool isPlayerInSight = _rayCastDetector.Check(transform.position, dir, out RaycastHit hit, _viewDistance);

        if (!isPlayerInSight)
            return false;

        return hit.collider.gameObject == closestTargetCollider.gameObject;
    }

    private Collider GetClosestCollider(Vector3 origin, List<Collider> colliders)
    {
        if (colliders == null || colliders.Count == 0)
            return null;

        if (colliders.Count == 1)
            return colliders[0];

        return colliders
            .OrderBy(c => (c.transform.position - origin).sqrMagnitude)
            .FirstOrDefault();
    }
}

