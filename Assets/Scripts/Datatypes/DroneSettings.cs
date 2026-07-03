using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using NaughtyAttributes;

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
    [Required] // Funktioniert in Subklassen, solange das Haupt-Skript von MonoBehaviour erbt
    [Tooltip("The ScriptableObject or variable tracking the target's transform.")]
    public TransformVariable Target;
    
    [Required]
    [Tooltip("The Rigidbody attached to this drone.")]
    public Rigidbody Rb;
    
    [Required]
    [Tooltip("The NavMeshAgent used for pathfinding or reference calculations.")]
    public NavMeshAgent Agent;
    
    [Required]
    [Tooltip("The weapon controller managing weapon firing mechanics.")]
    public GunController GunController;
    
    [Required]
    [Tooltip("The precise spawn point where projectiles leave the drone.")]
    public Transform BulletOrigin;

    public NavMeshPointGenerator NavMeshPointGenerator;
    
    [Header("Movement & Combat")]
    [Min(0f)] 
    [Tooltip("Maximum distance the drone can visually acquire a player.")]
    public float ViewDistance = 75f;
    
    [Min(0f)] 
    [Tooltip("The distance at which the drone transitions from chasing to attacking.")]
    public float AttackDistance = 7.5f;
    
    [Min(0f)] 
    [Tooltip("Radius used to check for nearby flocking partners.")]
    public float FlockingCheckRadius = 15.0f;
    
    [Min(0f)] 
    [Tooltip("Radius used to scan for potential targets.")]
    public float TargetCheckRadius = 30.0f;
    
    [Range(0f, 25f)] 
    [Tooltip("The regular locomotion velocity of the drone.")]
    public float Speed = 12.5f;
    
    [Range(0f, 30f)] 
    [Tooltip("How snappy the drone snaps its orientation to look at the target.")]
    public float RotateToTargetSpeed = 12.5f;

    [Tooltip("Duration the drone will search for a target before giving up.")]
    public TimeMS SearchDuration = new();

    [Label("Search Position Reached Threshold")]
    [Min(0f)]
    [Tooltip("Distance threshold to consider the search position as reached.")]
    public float searchPositionReachedThreshold = 1.25f;

    [Header("Events")]
    [Tooltip("Event fired right after initialization logic runs on spawn.")]
    public UnityEvent OnInitialize; 
}