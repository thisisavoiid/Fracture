using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[Serializable]
public class DroneSettings
{
    [Header("Detectors")]
    [Tooltip("Detector used to find neighboring drones for swarm behavior calculations.")]
    public OverlapSphereDetector SwarmDetector;
    
    [Tooltip("Detector used to scan for potential targets.")]
    public OverlapSphereDetector TargetDetector;
    
    [Tooltip("Raycaster to check for line-of-sight blocks between the drone and the player.")]
    public RayCastDetector RayCastDetector;

    [Header("Layer Masks")]
    [Tooltip("Layer mask that defines what counts as another drone in the swarm.")]
    public LayerMask SelfMask;
    
    [Tooltip("Layer mask used to filter target detection (e.g., Player).")]
    public LayerMask AttackMask;

    [Header("References")]
    [Tooltip("The ScriptableObject or variable tracking the target's transform.")]
    public TransformVariable Target;
    
    [Tooltip("The Rigidbody attached to this drone.")]
    public Rigidbody Rb;
    
    [Tooltip("The NavMeshAgent used for pathfinding or reference calculations.")]
    public NavMeshAgent Agent;
    
    [Tooltip("The weapon controller managing weapon firing mechanics.")]
    public GunController GunController;
    
    [Tooltip("The precise spawn point where projectiles leave the drone.")]
    public Transform BulletOrigin;

    [Header("Movement & Combat")]
    [Tooltip("Maximum distance the drone can visually acquire a player.")]
    [Min(0f)] public float ViewDistance = 75f;
    
    [Tooltip("The distance at which the drone transitions from chasing to attacking.")]
    [Min(0f)] public float AttackDistance = 7.5f;
    
    [Min(0)] public float FlockingCheckRadius = 15.0f;
    [Min(0)] public float TargetCheckRadius = 30.0f;
    
    [Tooltip("The regular locomotion velocity of the drone.")]
    [Range(0f, 25f)] public float Speed = 12.5f;
    
    [Tooltip("How snappy the drone snaps its orientation to look at the target.")]
    [Range(0f, 30f)] public float RotateToTargetSpeed = 12.5f;

    [Header("Events")]
    [Tooltip("Event fired right after initialization logic runs on spawn.")]
    public UnityEvent OnInitialize;
}