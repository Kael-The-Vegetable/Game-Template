using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Actor
{
    [SerializeField] int _jumpForce = 5;

    #region Movement
    public void MoveControl(InputAction.CallbackContext context)
    {
        _moveDir = new Vector2(context.ReadValue<Vector2>().x, 0);
    }
    public void JumpControl(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _body.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
        }
    }
    #endregion
}
