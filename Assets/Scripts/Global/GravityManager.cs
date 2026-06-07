using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [SerializeField] private float _lowGravity;
    private Vector3 _defaultGravity;

    private void Awake()
    {
        _defaultGravity = Physics.gravity;
    }

    public void SetLowGravity()
    {
        Vector3 gravity = Physics.gravity;
        gravity.y = _lowGravity;

        Physics.gravity = gravity;
    }

    public void SetDefaultGravity()
    {
        Vector3 gravity = Physics.gravity;
        gravity.y = _defaultGravity.y;

        Physics.gravity = gravity;
    }
}
