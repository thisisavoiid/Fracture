using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(Rigidbody))]
public class BeeBrain : Swarm
{
    [SerializeField] private Swarm _leaderSwarm;
    private OverlapSphereDetector _sphereDetector;
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _sphereDetector = GetComponent<OverlapSphereDetector>();
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
            Debug.Log($"Following leader swarm: {_leaderSwarm.gameObject.name} -");

            Vector3 leaderSwarmPosition = _leaderSwarm.transform.position;
            Vector3 leaderSwarmDir = leaderSwarmPosition - transform.position;
            Vector3 leaderSwarmForward = _leaderSwarm.transform.forward;

            Vector3 perlinNoise = new Vector3(
                Mathf.PerlinNoise(Time.time, transform.position.x),
                Mathf.PerlinNoise(Time.time, transform.position.y),
                Mathf.PerlinNoise(Time.time, transform.position.z)
            );

            float distanceToLeaderSwarm = (leaderSwarmPosition - transform.position).magnitude;

            List<Collider> swarmColliders = _sphereDetector.GetColliders(this.gameObject.layer);

            Vector3 seperationForce = Vector3.zero;

            foreach (Collider swarmCollider in swarmColliders)
            {
                if (swarmCollider.gameObject == this.gameObject)
                    continue;

                if (swarmCollider.GetComponent<Swarm>() == null)
                    continue;

                Vector3 diff = swarmCollider.gameObject.transform.position - transform.position;
                float distance = diff.magnitude;

                seperationForce += diff.normalized / distance;
            }

            Vector3 targetForce = (leaderSwarmDir * 1.25f) + (leaderSwarmForward * 0.5f) + (perlinNoise * 0.35f) + (seperationForce * 1.5f);

            _rb.linearVelocity = targetForce * distanceToLeaderSwarm;
        }
        else
        {

        }

    }
}