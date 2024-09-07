using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : Actor
{
    [SerializeField, Min(0), Tooltip("The total speed allowed.")] private float _maxSpeed = 5;
    [SerializeField, Min(0), Tooltip("The amount of force applied upwards when jumping.")] private int _jumpForce = 5;
    [Space]
    private bool _isGrounded;
    [SerializeField, Tooltip("Position is relative to the player. This is the starting line of the raycast, where it will draw from.")] private Vector2 _groundCastPosition;
    [SerializeField, Tooltip("Position is relative to the Cast Position. This is the ending lin of the raycast, where it will draw to.")] private Vector2 _groundCastLength;
    [SerializeField, Tooltip("This uses the layers on every object. Any object on a layer, that is the same name as this, will trigger the player to think that object is able to be jumped off.")] private string _nameOfGroundLayer = "Ground";
    [SerializeField, Min(0), Tooltip("When the player falls the gravity is multiplied to reduce the 'floaty' behaviour of Unity.")] private float _fallingGravityMultiplier = 2;
    [SerializeField, Min(0), Tooltip("The amount of time in seconds the game will still accept a jump input before touching the ground.")] private float _landingJumpInputTime = 0.1f;
    [SerializeField, Tooltip("This is a toggle for if you want players jump height to be tied to how long they hold the button for.")] private bool _holdForHigherJumps = true;
    private int _groundLayer;

    private float _defaultGravityScale;
    private float _landingJumpInputTimer = 0;

    public float LandingJumpInputTimer
    {
        get => _landingJumpInputTimer; set
        {
            if (value < 0)
            { // if the value trying to be set to LandingJumpInputTimer is below 0 it will be replaced with 0
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
        base.Awake(); // this calls the Awake method in the Actor script
        _defaultGravityScale = _body.gravityScale;
        _groundLayer = 1 << LayerMask.NameToLayer(_nameOfGroundLayer); // this moves the bit 1 to the 2s place then the 4s place and so on. Used for unique ids for the layers.
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // This line of code can be explained as follows
        _isGrounded = Physics2D.Raycast( // A Raycast is a line that detects if something traveled through it.
            (Vector2)transform.position + _groundCastPosition,// this is the position of the raycast
                                                              // (the transform.position makes it
                                                              // relative to the player)
            _groundCastLength.normalized, // This shows direction of where to cast the ray
            _groundCastLength.magnitude, // this shows for how long to cast the ray
            _groundLayer); // this shows what layer of stuff is allows to be detected


        if (Mathf.Abs(_body.velocity.x) > _maxSpeed)
        { // If the total value of x is greater than the max speed then clamp it to max speed
            _body.velocity = new Vector2(Mathf.Sign(_body.velocity.x) * _maxSpeed, _body.velocity.y);
        }

        if (_body.velocity.y < 0)
        { // If the velocity of y is downward, then make it falling
            _body.gravityScale = _defaultGravityScale * _fallingGravityMultiplier;
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }


        if (LandingJumpInputTimer > 0 && _isGrounded)
        { // This is to ensure that inputs just before a jump are not wasted
            Jump();
        }

        // The timer goes down the same time that this method is called.
        // This will count down in seconds.
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
