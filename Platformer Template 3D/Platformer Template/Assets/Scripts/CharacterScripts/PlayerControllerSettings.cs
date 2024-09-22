using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds all information about player movement
[Serializable]
public class PlayerMovementSettings
{
    [Min(0), Tooltip("The total speed allowed.")]
    public float maxSpeed = 5;

    [Min(0), Tooltip("The amount of force applied upwards when jumping.")]
    public int jumpForce = 5;
}

// This class holds all information about player looking
[Serializable]
public class PlayerLookSettings
{
    [Tooltip("This is the target for looking that is attached to the player. This will allow the cinemachine camera to rotate around.")]
    public Transform _lookTarget;

    [Min(0), Tooltip("The amount of rotation power the looking has.")]
    public float _rotationPower = 0.01f;

    [Range(0, 90), Tooltip("The greatest vertical angle allowed to look down.")]
    public float _maxLookDownAngle = 40;

    [Range(-90, 0), Tooltip("The greatest vertical angle allowed to look up.")]
    public float _maxLookUpAngle = -40;

    [Tooltip("Enable this if you want the player to rotate with the mouse as well.")]
    public bool _playerRotateWithCamera = false;
}

// this class holds all information about the player's ground detection
[Serializable]
public class PlayerGroundDetectionSettings
{
    public bool _isGrounded;

    [Tooltip("Position is relative to the player. This is the starting line of the raycast, where it will draw from.")]
    public Vector3 _groundCastPosition;

    [Tooltip("Position is relative to the Cast Position. This is the ending lin of the raycast, where it will draw to.")]
    public Vector3 _groundCastLength;

    [Tooltip("The player will be able to jump off any colliders in the selected layer(s).")]
    public LayerMask _groundLayer;
}

// This class holds all information about the player controls
[Serializable]
public class PlayerControlSettings
{
    [Tooltip("A Constant force component that will only be activated while moving down.")]
    public ConstantForce _fallingGravityForce;

    [Min(0), Tooltip("The amount of time in seconds the game will still accept a jump input before touching the ground.")]
    public float _landingJumpInputTime = 0.1f;

    [Tooltip("This is a toggle for if you want players jump height to be tied to how long they hold the button for.")]
    public bool _holdForHigherJumps = true;
}