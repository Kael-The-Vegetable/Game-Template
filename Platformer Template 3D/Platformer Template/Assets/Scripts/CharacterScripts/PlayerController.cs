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
    [SerializeField, Space] private PlayerMovementSettings movementSettings = new();

    [SerializeField, Space] private PlayerLookSettings lookSettings = new();

    [SerializeField, Space] private PlayerGroundDetectionSettings groundDetectionSettings = new();

    [SerializeField, Space] private PlayerControlSettings controlSettings = new();

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

        if (controlSettings._fallingGravityForce != null)
        {
            controlSettings._fallingGravityForce.enabled = false;
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
        groundDetectionSettings._isGrounded = Physics.Raycast( // A Raycast is a line that detects if something traveled through it.
            transform.position + groundDetectionSettings._groundCastPosition,// this is the position of the raycast
                                                              // (the transform.position makes it
                                                              // relative to the player)
            groundDetectionSettings._groundCastLength.normalized, // This shows direction of where to cast the ray
            out _, // This just shows that we do not care about the output of this Raycast except for the bool
            groundDetectionSettings._groundCastLength.magnitude, // this shows for how long to cast the ray
            groundDetectionSettings._groundLayer); // this shows what layer(s) will be counted as ground.
        #endregion

        #region Horizontal Velocity Clamping
        Vector2 horizontalVelocity = new Vector2(_body.velocity.x, _body.velocity.z);
        horizontalVelocity = Vector2.ClampMagnitude(horizontalVelocity, movementSettings.maxSpeed);
        _body.velocity = new Vector3(horizontalVelocity.x, _body.velocity.y, horizontalVelocity.y);
        #endregion

        if (controlSettings._fallingGravityForce != null)
        {
            // if the body's vertical velocity is less than 0 (the object is falling), enable the falling gravity component.
            controlSettings._fallingGravityForce.enabled = _body.velocity.y < 0;
        }

        if (LandingJumpInputTimer > 0 && groundDetectionSettings._isGrounded)
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
        _body.AddForce(new Vector3(0, movementSettings.jumpForce, 0), ForceMode.Impulse);
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
            LandingJumpInputTimer = controlSettings._landingJumpInputTime;
        }

        if (context.canceled)
        {
            LandingJumpInputTimer = 0;

            if (controlSettings._holdForHigherJumps)
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

        if (lookSettings._lookTarget == null)
        {
            Debug.LogError("You must set a Look target for camera movement to function.");
            return;
        }

        if (lookSettings._playerRotateWithCamera)
        {
            transform.rotation *= Quaternion.AngleAxis(delta.x * lookSettings._rotationPower, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            lookSettings._lookTarget.rotation *= Quaternion.AngleAxis(delta.x * lookSettings._rotationPower, Vector3.up);
        }

        lookSettings._lookTarget.rotation *= Quaternion.AngleAxis(delta.y * lookSettings._rotationPower, Vector3.right);

        //clamp the up/down axis
        Vector3 angles = lookSettings._lookTarget.eulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < 360 + lookSettings._maxLookUpAngle)
        {
            angles.x = 360 + lookSettings._maxLookUpAngle;
        }
        else if (angles.x < 180 && angles.x > lookSettings._maxLookDownAngle)
        {
            angles.x = lookSettings._maxLookDownAngle;
        }
        lookSettings._lookTarget.eulerAngles = angles;
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (_showGroundRaycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + groundDetectionSettings._groundCastPosition, groundDetectionSettings._groundCastLength); 
        }

        if (_showVelocityVector && _body != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_body.position + _body.centerOfMass, _body.velocity);
        }

        if (_showFallingGravityVector && controlSettings._fallingGravityForce != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, controlSettings._fallingGravityForce.force + controlSettings._fallingGravityForce.relativeForce);
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
