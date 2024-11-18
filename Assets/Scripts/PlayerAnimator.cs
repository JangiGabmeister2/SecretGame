using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;

    [HideInInspector] public bool isMoving = false, isJumping = false, isFalling = false;

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isJumping", isJumping);
            animator.SetBool("isFalling", isFalling);
        }
    }
}