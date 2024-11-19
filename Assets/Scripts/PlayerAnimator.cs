using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    [HideInInspector] public bool isMoving = false, isJumping = false, isFalling = false;

    Rigidbody2D rb => GetComponent<Rigidbody2D>();

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isJumping", isJumping);
            animator.SetBool("isFalling", isFalling);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        float horizontal = context.ReadValue<Vector2>().x;

        if (horizontal != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;  
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
        }
        else if (context.canceled)
        {
            isJumping = false;

            Falling();
        }
    }

    public void Falling()
    {
        if (rb.velocity.y != 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }
}