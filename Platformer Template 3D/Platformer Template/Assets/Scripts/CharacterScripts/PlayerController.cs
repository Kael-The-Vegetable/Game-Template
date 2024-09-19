using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Actor
{
    // Serialized simply means that the field will be shown in the inspector. Fields that are public will automatically be shown, without the need for the [SerializeField] attribute.
    // #region allows you to collapse a section of code by clicking the arrow to the left of it.
    #region Serailized Fields
    #region Movement Settings
    [Header("Movement Settings")]
    [SerializeField, Min(0), Tooltip("The total speed allowed.")]
    private float _maxSpeed = 5;

    [SerializeField, Min(0), Tooltip("The amount of force applied upwards when jumping.")]
    private int _jumpForce = 5;
    #endregion

    #region Looking Settings
    [Header("Looking Settings")]
    [SerializeField, Tooltip("This is the target for looking that is attached to the player. This will allow the cinemachine camera to rotate around.")]
    private Transform _lookTarget;

    [SerializeField, Min(0), Tooltip("The amount of rotation power the looking has.")]
    private float _rotationPower = 0.01f;

    [SerializeField, Range(0, 90), Tooltip("The greatest vertical angle allowed to look down.")]
    private float _maxLookDownAngle = 40;

    [SerializeField, Range(-90, 0), Tooltip("The greatest vertical angle allowed to look up.")]
    private float _maxLookUpAngle = -40;

    [SerializeField, Tooltip("Enable this if you want the player to rotate with the mouse as well.")]
    private bool _playerRotateWithCamera = false;
    #endregion

    #region Ground Detection Settings
    [Header("Ground Detection Settings")]
    [SerializeField] private bool _isGrounded;
    
    [SerializeField, Tooltip("Position is relative to the player. This is the starting line of the raycast, where it will draw from.")]
    private Vector3 _groundCastPosition;
    
    [SerializeField, Tooltip("Position is relative to the Cast Position. This is the ending lin of the raycast, where it will draw to.")]
    private Vector3 _groundCastLength;
    
    [SerializeField, Tooltip("The player will be able to jump off any colliders in the selected layer(s).")]
    private LayerMask _groundLayer;
    #endregion

    #region Other Settings
    [Header("Other Settings")]
    [SerializeField, Tooltip("A Constant force component that will only be activated while moving down.")]
    private ConstantForce _fallingGravityForce;
    
    [SerializeField, Min(0), Tooltip("The amount of time in seconds the game will still accept a jump input before touching the ground.")]
    private float _landingJumpInputTime = 0.1f;
    
    [SerializeField, Tooltip("This is a toggle for if you want players jump height to be tied to how long they hold the button for.")]
    private bool _holdForHigherJumps = true;
    #endregion

    #region Debug Settings
    [Header("Debug Settings")]
    [SerializeField]
    private bool _showGroundRaycast = true;

    [SerializeField]
    private bool _showVelocityVector = true;

    [SerializeField]
    private bool _showFallingGravityVector = true;

    [SerializeField]
    private bool _showFacingDirectionVector = true;

    [SerializeField]
    private bool _showMoveDirVector = true;
    #endregion
    #endregion

    private float _landingJumpInputTimer = 0;
    public float LandingJumpInputTimer
    {
        get => _landingJumpInputTimer; set
        {
            if (value < 0)
            { // if the value trying to be set to LandingJumpInputTimer is below 0 it will be replaced with 0. In other words, _landingJumpInputTimer can never be less than 0.
                _landingJumpInputTimer = 0;
            }
            else
            {
                _landingJumpInputTimer = value;
            }
        }
    }

    private Vector3 _originalMoveDir = Vector3.zero;

    public override void Awake()
    {
        base.Awake(); // this calls the Awake method in the base class, "Actor"

        if (_fallingGravityForce != null)
        {
            _fallingGravityForce.enabled = false;
        }
    }

    // Physics-related code, such as adding force, should be executed in FixedUpdate for consistent results.
    public override void FixedUpdate()
    {
        Vector3 trueMoveDir = transform.forward * _originalMoveDir.z + transform.right * _originalMoveDir.x;
        MoveDir = trueMoveDir;

        base.FixedUpdate();

        #region IsGrounded
        // This line of code can be explained as follows
        _isGrounded = Physics.Raycast( // A Raycast is a line that detects if something traveled through it.
            transform.position + _groundCastPosition,// this is the position of the raycast
                                                              // (the transform.position makes it
                                                              // relative to the player)
            _groundCastLength.normalized, // This shows direction of where to cast the ray
            out _, // This just shows that we do not care about the output of this Raycast except for the bool
            _groundCastLength.magnitude, // this shows for how long to cast the ray
            _groundLayer); // this shows what layer(s) will be counted as ground.
        #endregion

        #region Horizontal Velocity Clamping
        Vector2 horizontalVelocity = new Vector2(_body.velocity.x, _body.velocity.z);
        horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, _maxSpeed);
        _body.velocity = new Vector3(horizontalVelocity.x, _body.velocity.y, horizontalVelocity.y);
        #endregion

        if (_fallingGravityForce != null)
        {
            // if the body's vertical velocity is less than 0 (the object is falling), enable the falling gravity component.
            _fallingGravityForce.enabled = _body.velocity.y < 0;
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
        _body.velocity = new Vector3(_body.velocity.x, 0, _body.velocity.z);
        _body.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    #region Movement
    public void MoveControl(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = context.ReadValue<Vector2>();
        _originalMoveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
    }

    public void JumpControl(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LandingJumpInputTimer = _landingJumpInputTime;
        }

        if (context.canceled)
        {
            LandingJumpInputTimer = 0;

            if (_holdForHigherJumps)
            {
                // Set player's vertical velocity to 0 when the button is released.
                if (_body.velocity.y > 0)
                {
                    _body.velocity = new Vector3(_body.velocity.x, 0, _body.velocity.z);
                }
            }
        }
    }
    #endregion

    #region Looking
    public void LookControl(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        

        if (_playerRotateWithCamera)
        {
            transform.rotation *= Quaternion.AngleAxis(delta.x * _rotationPower, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            _lookTarget.rotation *= Quaternion.AngleAxis(delta.x * _rotationPower, Vector3.up);
        }

        _lookTarget.rotation *= Quaternion.AngleAxis(delta.y * _rotationPower, Vector3.right);

        //clamp the up/down axis
        Vector3 angles = _lookTarget.eulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < 360 + _maxLookUpAngle)
        {
            angles.x = 360 + _maxLookUpAngle;
        }
        else if (angles.x < 180 && angles.x > _maxLookDownAngle)
        {
            angles.x = _maxLookDownAngle;
        }
        _lookTarget.eulerAngles = angles;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (_showGroundRaycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + _groundCastPosition, _groundCastLength); 
        }

        if (_showVelocityVector && _body != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_body.position + _body.centerOfMass, _body.velocity);
        }

        if (_showFallingGravityVector && _fallingGravityForce != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _fallingGravityForce.force + _fallingGravityForce.relativeForce);
        }

        if (_showFacingDirectionVector)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 2);
        }

        if (_showMoveDirVector)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawRay(transform.position, MoveDir * 3);
        }
    }
}
