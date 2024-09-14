using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Actor : MonoBehaviour, IDamageable
{
    [Header("Actor Settings")]
    [SerializeField] internal Rigidbody _body;
    int IDamageable.Health { get; set; }

    [SerializeField] internal Vector3 _moveDir = Vector3.zero;
    [SerializeField] internal float _speed = 0;

    public virtual void Awake()
    {
        // If the body field isn't set manually, it will be set to the component attached to this object.
        if (_body == null)
        {
            _body = GetComponent<Rigidbody>();
        }
    }

    public virtual void FixedUpdate()
    {
        _body.AddForce(_moveDir * _speed);
    }

    void IDamageable.TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
