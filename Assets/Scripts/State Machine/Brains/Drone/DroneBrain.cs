using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class DroneBrain : Spawnable, ICollectionMember
{
    [SerializeField]
    [BoxGroup("Settings")]
    private DroneSettings _settings;

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
        Vector3 currentAgentPosition = _settings.Agent.transform.position;
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
        _settings.SwarmDetector.SetRadius(10);
        List<Collider> colliders = _settings.SwarmDetector.GetColliders(_settings.SelfMask);
        return colliders.Select(collider => collider.gameObject.transform).ToList();
    }

    public override void Spawn()
    {
        Subscribe();
        SetDetectorValues();
        ConfigureStateMachine(); // temporary!!
        _settings.OnInitialize?.Invoke();
    }

    private void SetDetectorValues()
    {
        if (_settings.TargetDetector != null)
            _settings.TargetDetector.SetRadius(_settings.TargetCheckRadius);

        if (_settings.SwarmDetector != null)
            _settings.SwarmDetector.SetRadius(_settings.FlockingCheckRadius);
    }

    private void ConfigureStateMachine()
    {
        State chaseState = new DroneChaseState(
            this,
            _settings.Rb,
            _settings.Agent,
            _settings.Target,
            _settings.Speed
        );

        State attackState = new DroneAttackState(
            this,
            _settings.Rb,
            _settings.Agent,
            _settings.Target,
            _settings.BulletOrigin,
            _settings.GunController
        );

        State idleState = new DroneIdleState(
            this,
            _settings.Rb,
            _settings.Agent,
            _settings.Speed
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
                    attackState, () => CanSeePlayer() && Vector3.Distance(
                        _settings.Rb.position,
                        _settings.Target.Value.position
                    ) <= _settings.AttackDistance
                ),
                new Transition(
                    idleState, () => !CanSeePlayer()
                )
            }
        );

        _states.Add(
            attackState,
            new List<Transition>
            {
                new Transition(
                    chaseState, () => Vector3.Distance(_settings.Rb.position, _settings.Target.Value.position) > _settings.AttackDistance
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
        List<Collider> foundColliders = _settings.TargetDetector.GetColliders(_settings.AttackMask);
        Collider closestTargetCollider = GetClosestCollider(transform.position, foundColliders);

        if (closestTargetCollider == null)
            return false;

        Vector3 dir = closestTargetCollider.transform.position - transform.position;

        bool isPlayerInSight = _settings.RayCastDetector.Check(transform.position, dir, out RaycastHit hit, _settings.ViewDistance);

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

    public void RotateTowardsTarget()
    {
        if (_settings.Target == null)
            return;

        if (_settings.Target.Value == null)
            return;

        if (_settings.Agent == null)
            return;

        if (_settings.Rb == null)
            return;

        Vector3 dir = _settings.Target.Value.position - _settings.Rb.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        _settings.Rb.rotation = Quaternion.Lerp(
            _settings.Rb.rotation,
            targetRotation,
            Time.deltaTime * _settings.RotateToTargetSpeed
        );
    }

    public void ResetRotation()
    {
        if (_settings.Rb == null)
            return;

        _settings.Rb.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }

    public void Subscribe()
    {
        EnemyCollectionManager.Instance?.Subscribe(this);
    }

    public void Unsubscribe()
    {
        EnemyCollectionManager.Instance?.Unsubscribe(this);
    }
}

