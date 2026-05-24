using System;
using UnityEngine;

[Serializable]
public class FollowerBeeConfig
{
    [Header("Movement & Core")]
    [Tooltip("The movement speed of the follower bee.")]
    public float Speed = 5f;
    
    public float Acceleration = 3.5f;

    [Tooltip("The vertical height offset relative to the leader bee.")]
    public float YOffset = 4.75f;
    
    [Tooltip("How fast the bee rotates towards its current target.")]
    public float TurnToTargetSpeed = 5.0f;
    
    [Tooltip("The distance threshold to the leader bee where noise movement takes over.")]
    public float DeathZoneDistance = 2.0f;

    public float ClampDegrees = 45;

    [Space(10)]
    [Header("Detection & Layers")]
    [Tooltip("The radius used to check for neighboring bees to avoid crowding.")]
    public float SeparationCheckRadius = 2.5f;
    
    [Tooltip("The maximum distance at which the bee can detect a target.")]
    public float TargetCheckRadius = 10.0f;
    
    [Tooltip("The layer mask used to identify other bees in the swarm.")]
    public LayerMask BeeLayers;

    [Space(10)]
    [Header("Swarm Weights (Boids)")]
    [Tooltip("Multiplier applied directly to the separation force calculation.")]
    public float SeparationForceMultiplier = 1.0f;
    
    [Tooltip("How much the random Perlin noise affects the flight path.")]
    public float PerlinNoiseWeight = 0.35f;
    
    [Tooltip("Weight for the pull towards the leader bee's current position.")]
    public float DirectionToLeaderSwarmWeight = 1.25f;
    
    [Tooltip("Weight for matching the forward direction of the leader bee.")]
    public float LeaderSwarmForwardWeight = 0.75f;
    
    [Tooltip("Weight for pushing away from nearby bees to maintain personal space.")]
    public float SeparationForceWeight = 1.5f;

    [Space(10)]
    [Header("References")]
    [Tooltip("The detector component used to scan for neighboring bees.")]
    public OverlapSphereDetector SeparationSphereDetector;
    
    [Tooltip("The detector component used to scan for potential targets.")]
    public OverlapSphereDetector TargetSearchSphereDetector;
    
    [Tooltip("The weapon item assigned to this bee. Can be left empty.")]
    public Item Gun; 
}