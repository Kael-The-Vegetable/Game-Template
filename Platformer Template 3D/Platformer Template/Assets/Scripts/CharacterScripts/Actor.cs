using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Actor : MonoBehaviour, IDamageable
{
    [SerializeField] internal Rigidbody _body;
    int IDamageable.Health { get; set; }

    [SerializeField] internal Vector3 _moveDir = Vector3.zero;
    [SerializeField] internal float _speed = 0;

    public virtual void Awake()
    {
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
