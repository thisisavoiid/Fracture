using System;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Rendering;

public class HeadBob : MonoBehaviour
{
    [SerializeField] private float _strength;
    [SerializeField] private float _speed;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private bool _isActive = false;

    private Vector3 _basePos;

    private void Awake()
    {
        _basePos = _targetTransform.position;
    }

    void Update()
    {
        if (!_isActive)
            return;
        
        _targetTransform.position = new Vector3(
            _basePos.x, 
            Mathf.Sin(Time.time * _speed) * _strength, 
            _basePos.z
        );
    }
}
