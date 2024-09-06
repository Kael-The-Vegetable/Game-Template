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
    private int _groundLayer;
    public new void Awake()
    {
        base.Awake();
        _groundLayer = 1 << LayerMask.NameToLayer(_nameOfGroundLayer);
    }
    public new void FixedUpdate()
    {
        base.FixedUpdate();

        _isGrounded = Physics2D.Raycast((Vector2)transform.position + _groundCastPosition, _groundCastLength.normalized, _groundCastLength.magnitude, _groundLayer);
        if (Mathf.Abs(_body.velocity.x) > _maxSpeed)
        {
            _body.velocity = new Vector2(Mathf.Sign(_body.velocity.x) * _maxSpeed, _body.velocity.y);
        }
    }

    #region Movement
    public void MoveControl(InputAction.CallbackContext context)
    {
        _moveDir = new Vector2(context.ReadValue<Vector2>().x, 0);
    }
    public void JumpControl(InputAction.CallbackContext context)
    {
        if (context.performed && _isGrounded)
        {
            _body.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
        }
    }
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay((Vector2)transform.position + _groundCastPosition, _groundCastLength);
    }
}
