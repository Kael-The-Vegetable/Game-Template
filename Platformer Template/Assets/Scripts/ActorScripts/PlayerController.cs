using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : Actor
{
    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private int _jumpForce = 5;
    [Space]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Vector2 _groundCastPosition;
    [SerializeField] private Vector2 _groundCastLength;
    [SerializeField] private string _nameOfGroundLayer = "Ground";
    [SerializeField, Min(0)] private float _fallingGravityMultiplier = 2;
    [SerializeField, Min(0), Tooltip("The amount of time in seconds the game will still accept a jump input before touching the ground.")] private float _landingJumpInputTime = 0.1f;
    [SerializeField] private bool _holdForHigherJumps = true;
    private int _groundLayer;

    private float _defaultGravityScale;
    private float _landingJumpInputTimer = 0;

    public float LandingJumpInputTimer
    {
        get => _landingJumpInputTimer; set
        {
            if (value < 0)
            {
                _landingJumpInputTimer = 0;
            }
            else
            {
                _landingJumpInputTimer = value;
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        _defaultGravityScale = _body.gravityScale;
        _groundLayer = 1 << LayerMask.NameToLayer(_nameOfGroundLayer);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        _isGrounded = Physics2D.Raycast((Vector2)transform.position + _groundCastPosition, _groundCastLength.normalized, _groundCastLength.magnitude, _groundLayer);
        if (Mathf.Abs(_body.velocity.x) > _maxSpeed)
        {
            _body.velocity = new Vector2(Mathf.Sign(_body.velocity.x) * _maxSpeed, _body.velocity.y);
        }

        if (_body.velocity.y < 0)
        {
            _body.gravityScale = _defaultGravityScale * _fallingGravityMultiplier;
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }


        if (LandingJumpInputTimer > 0 && _isGrounded)
        {
            Jump();
        }
        LandingJumpInputTimer -= Time.fixedDeltaTime;
    }

    private void Jump()
    {
        LandingJumpInputTimer = 0;
        _body.velocity = new Vector2(_body.velocity.x, 0);
        _body.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
    }

    #region Movement
    public void MoveControl(InputAction.CallbackContext context)
    {
        _moveDir = new Vector2(context.ReadValue<Vector2>().x, 0);
    }
    public void JumpControl(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LandingJumpInputTimer = _landingJumpInputTime;
        }

        if (context.canceled && _holdForHigherJumps)
        {
            LandingJumpInputTimer = 0;
            if (_body.velocity.y > 0)
            {
                _body.velocity = new Vector2(_body.velocity.x, 0);
            }
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay((Vector2)transform.position + _groundCastPosition, _groundCastLength);
    }
}
