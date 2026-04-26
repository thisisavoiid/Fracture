using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(Rigidbody))]
public class BeeBrain : Swarm
{
    [Header("Debugging")]
    [SerializeField] private Swarm _leaderSwarm;
    [SerializeField] private bool _showNeighbourBees = false;
    [SerializeField] private LayerMask _beeLayers;

    [Header("Settings")]
    [SerializeField] private float _seperationForceMultiplier = 1.0f;
    [SerializeField] private float _seperationCheckRadius = 2.5f;

    [Header("Force Weighting")]
    [SerializeField] private float _perlinNoiseWeight = 0.35f;
    [SerializeField] private float _directionToLeaderSwarmWeight = 1.25f;
    [SerializeField] private float _leaderSwarmForwardWeight = 0.75f;
    [SerializeField] private float _seperationForceWeight = 1.5f;

    private OverlapSphereDetector _sphereDetector;
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _sphereDetector = GetComponent<OverlapSphereDetector>();
        _sphereDetector.SetRadius(_seperationCheckRadius);
    }
    public override void Init(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public override void InvokeDeath()
    {

    }

    public override void StartAttack()
    {

    }

    public override void SwarmTick()
    {
        if (_leaderSwarm == null)
            return;

        if (_leaderSwarm.gameObject != this.gameObject)
        {
            Vector3 leaderSwarmPosition = _leaderSwarm.transform.position;
            Vector3 leaderSwarmDir = leaderSwarmPosition - transform.position;
            Vector3 leaderSwarmForward = _leaderSwarm.transform.forward;

            Vector3 perlinNoise = new Vector3(
                Mathf.PerlinNoise(Time.time, transform.position.x),
                Mathf.PerlinNoise(Time.time, transform.position.y),
                Mathf.PerlinNoise(Time.time, transform.position.z)
            );

            float distanceToLeaderSwarm = (leaderSwarmPosition - transform.position).magnitude;

            List<Collider> swarmColliders = _sphereDetector.GetColliders(_beeLayers);

            Vector3 seperationForce = Vector3.zero;

            foreach (Collider swarmCollider in swarmColliders)
            {
                if (swarmCollider.gameObject == this.gameObject)
                    continue;

                if (swarmCollider.GetComponent<Swarm>() == null)
                    continue;

                Vector3 diff = swarmCollider.gameObject.transform.position - transform.position;
                float distance = diff.magnitude;

                seperationForce += _seperationForceMultiplier * (diff.normalized * -1) / Mathf.Max(1.0f, distance);
            }

            Vector3 targetForce = (leaderSwarmDir * _directionToLeaderSwarmWeight) + (leaderSwarmForward * _leaderSwarmForwardWeight) + (perlinNoise * _perlinNoiseWeight) + (seperationForce * _seperationForceWeight);

            _rb.linearVelocity = targetForce;

            if (_showNeighbourBees)
                Debug.Log($"[BEE BRAIN] Neighbours of {this.gameObject.name}: {string.Join(", ", swarmColliders.Select(item => item.gameObject.name))}");

        }
        else
        {

        }

    }
}