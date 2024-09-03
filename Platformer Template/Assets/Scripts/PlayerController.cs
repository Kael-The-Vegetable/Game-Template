using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Actor
{
    public void MoveControl(InputAction.CallbackContext context)
    {
        _moveDir = context.ReadValue<Vector2>();
    }

    
}
