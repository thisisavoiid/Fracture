using System;
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
        _basePos = _targetTransform.localPosition;
    }
    
    private void Update()
    {
        if (!_isActive)
            return;
        
        _targetTransform.localPosition = new Vector3(
            _basePos.x, 
            _basePos.y + Mathf.Sin(Time.time * _speed) * _strength, 
            _basePos.z + Mathf.Cos(Time.time * _speed) * _strength
        );
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetStrength(float strength)
    {
        _strength = strength;
    }

}
