using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FootstepSFX : MonoBehaviour
{
    [SerializeField] private List<PhysicsMaterialSoundPair> _surfaceFootstepSounds;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _distanceTreshold = 2.5f;
    private Dictionary<PhysicsMaterial, Sound> _materialSoundDict = new();
    private PhysicsMaterial _currentPhysicsMaterial;
    private float _distanceTraveledDelta = 0.0f;
    private Vector3 _lastPosition;

    private void Awake()
    {
        if (_surfaceFootstepSounds.Count == 0)
            return;

        foreach (PhysicsMaterialSoundPair pair in _surfaceFootstepSounds)
        {
            if (pair.Sound == null || pair.PhysicsMaterial == null)
                continue;

            _materialSoundDict.Add(pair.PhysicsMaterial, pair.Sound);
        }

        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (_audioSource == null)
            return;

        if (_currentPhysicsMaterial == null)
            return;
        
        if (_materialSoundDict.Keys.Count == 0)
            return;

        if (!_materialSoundDict.ContainsKey(_currentPhysicsMaterial))
            return;

        Sound footstepSound = _materialSoundDict[_currentPhysicsMaterial];

        if (footstepSound == null)
            return;

        UpdateTravelDistanceDelta();

        if (_distanceTraveledDelta < _distanceTreshold)
            return;

        footstepSound.Config.ApplyTo(_audioSource);

        _audioSource.Play();

        ResetTravelDistanceDelta();
    }

    private void ResetTravelDistanceDelta()
    {
        _distanceTraveledDelta = 0.0f;
    }

    private void UpdateTravelDistanceDelta()
    {
        Vector3 currentPosition = transform.position;

        if (_lastPosition == currentPosition)
            return;

        _distanceTraveledDelta += GetDistanceToLastPoint();
        _lastPosition = currentPosition;
    }

    private float GetDistanceToLastPoint()
    {
        return (_lastPosition - transform.position).magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PhysicsMaterial colliderMaterial = collision.collider.sharedMaterial;

        if (colliderMaterial == null)
            return;

        if (colliderMaterial == _currentPhysicsMaterial)
            return;

        _currentPhysicsMaterial = colliderMaterial;
    }

    private void OnCollisionExit(Collision collision)
    {
        _currentPhysicsMaterial = null;
    }
}
