// this script should allow the other scripts to get each current input form
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputManager2 : MonoBehaviour
{
    // ------------------------ variables: ------------------------
    private Vector2 moveDirection = Vector2.zero;   // move button
    private bool jumpPressed = false;               // jump button
    private bool interactPressed = false;           // interact button
    private bool submitPressed = false;             // submit button

    private static InputManager2 instance;           // input manager object


    // ------------------------ functions: ------------------------
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("there's more than one input manager");
        }

        instance = this;
    }

    public static InputManager2 GetInstance() 
    {
        return instance;
    }


    // these functions perform actions based on which controls are pressed
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed) {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled) {
            moveDirection = context.ReadValue<Vector2>();
        } 
    }

    public void JumpPressed(InputAction.CallbackContext context)
    {
        if (context.performed) {
            jumpPressed = true;
        }
        else if (context.canceled) {
            jumpPressed = false;
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed) {
            interactPressed = true;
        }
        else if (context.canceled) {
            interactPressed = false;
        } 
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed) {
            submitPressed = true;
        }
        else if (context.canceled) {
            submitPressed = false;
        } 
    }


    public Vector2 GetMoveDirection() 
    {
        return moveDirection;
    }


    // for any of the below 'Get' methods, if we're getting it then we're also using it,
    // which means we should set it to false so that it can't be used again until actually
    // pressed again.
    public bool GetJumpPressed() 
    {
        bool result = jumpPressed;
        jumpPressed = false;
        return result;
    }

    public bool GetInteractPressed() 
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetSubmitPressed() 
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed() 
    {
        submitPressed = false;
    }

}
// ------------------------ end of file ------------------------