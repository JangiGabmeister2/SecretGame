using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] float _normalMoveSpeed = 8f;
    private float _moveSpeed;
    [SerializeField] float _jumpSpeed = 20f;
    public Transform orientation;

    [Space(20)] public bool canJump;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundCheckDistance = 1f;
    [SerializeField] LayerMask _groundLayer;

    private float _coyoteTime = 0.15f;
    private float _coyoteTimeCounter = 0;

    private float _jumpBufferTime = 0.15f;
    private float _jumpBufferCounter = 0;

    private float _horizontal;
    private bool _isFacingRight = true;
    #endregion

    #region Properties
    Rigidbody2D rb => GetComponent<Rigidbody2D>();
    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    bool moving => _horizontal != 0;
    #endregion

    #region Methods

    #region Unity Methods

    private void Update()
    {
        canJump = IsGrounded();

        _moveSpeed = _normalMoveSpeed;
        GroundedMovement();

        //when player walks off a surface, there is a small moment of time they can still jump despite being off the platform.
        //after this time they cannot jump again until they've landed
        #region Jumping and Coyote Time
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;

            anim.isFalling = false;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;

            if (_coyoteTimeCounter <= 0)
                anim.isFalling = true;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _jumpBufferCounter = _jumpBufferTime;

            anim.isJumping = true;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;

            anim.isJumping = false;
        }

        if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);

            _jumpBufferCounter = 0f;
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            _coyoteTimeCounter = 0f;

            anim.isFalling = true;
        }
        #endregion

        #region Horizontal Movement Handling
        _horizontal = Input.GetAxis("Horizontal");

        if (_horizontal != 0)
        {
            orientation.localPosition = transform.right;
            anim.isMoving = true;
        }
        else
        {
            orientation.localPosition = transform.up;
            anim.isMoving = false;
        }
        #endregion

        //prevents player from moving horizontally, despite no horizontal input
        #region Prevent Sliding Movement
        if (!moving)
        {
            Vector2 velo = rb.velocity;
            velo.x = 0;
            rb.velocity = velo;
        }
        #endregion

        #region Transform Flipping
        if (_isFacingRight && _horizontal < 0f || !_isFacingRight && _horizontal > 0f)
        {
            Flip();
        }
        #endregion
    }
    #endregion

    //moves player upon giving control inputs
    private void GroundedMovement()
    {
        float speed = _moveSpeed * _horizontal;

        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    //checks whether player transform is above a collider in the ground layer
    private bool IsGrounded()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(_groundCheck.position, _groundCheckDistance, _groundLayer);

        if (cols.Length > 0)
        {
            return true;
        }

        return false;
    }

    //flips the player transform via x-axis to simulate switching directions when moving
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    #endregion
}