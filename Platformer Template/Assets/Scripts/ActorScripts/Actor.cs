using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, IDamageable
{
    [SerializeField] protected Rigidbody2D _body;
    int IDamageable.Health { get; set; }

    [SerializeField] protected Vector2 _moveDir = Vector2.zero;
    [SerializeField] protected float _speed = 0;

    public virtual void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
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