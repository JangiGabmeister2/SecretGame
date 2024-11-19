using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] float _normalMoveSpeed = 8f;
    private float _moveSpeed;
    [SerializeField] float _jumpSpeed = 20f;
    public Transform orientation;

    [Space(20)] public bool canJump;
    [SerializeField] private int _maxJumps = 3;
    private int _remainingJumps;

    [SerializeField, Space(20)] Transform _groundCheck;
    [SerializeField] float _groundCheckDistance = 1f;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField, Space(20)] private float _wallSlideSpeed = 2f;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckDistance = .2f;
    [SerializeField] private LayerMask _wallLayer;

    private float _coyoteTime = 0.15f;
    private float _coyoteTimeCounter = 0;

    private float _jumpBufferTime = 0.15f;
    private float _jumpBufferCounter = 0;

    private float _horizontal;
    private bool _isFacingRight = true;
    #endregion

    #region Properties
    Rigidbody2D rb => GetComponent<Rigidbody2D>();
    bool moving => _horizontal != 0;
    #endregion

    #region Methods

    #region Unity Methods

    private void Update()
    {
        canJump = IsGrounded();

        _moveSpeed = _normalMoveSpeed;

        GroundedMovement();

        #region Horizontal Movement Handling
        if (_horizontal != 0)
        {
            orientation.localPosition = transform.right;
        }
        else
        {
            orientation.localPosition = transform.up;
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
        //flips the player transform when switching horizontal directions (faces left when moving left)
        if (_isFacingRight && _horizontal < 0f || !_isFacingRight && _horizontal > 0f)
        {
            Flip();
        }
        #endregion

        #region Wall Slide
        //when on contact with a wall
        if (OnWall())
        {
            WallSlide();
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
        return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckDistance, _groundLayer);
    }

    //flips the player transform via x-axis to simulate switching directions when moving
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    #region Wall Sliding/Jumping
    //checks whether the player is facing a wall
    private bool OnWall()
    {
        return Physics2D.OverlapCircle(_wallCheck.position, _wallCheckDistance, _wallLayer);
    }

    //allows the player to 'stick' to the wall and slide down it
    private void WallSlide()
    {
        if (!IsGrounded() && _horizontal != 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -_wallSlideSpeed, float.MaxValue));

            Flip();
        }
    }
    #endregion

    #region Jump + Coyote Jump & Jump Buffer
    public void Jump(InputAction.CallbackContext context)
    {
        //when the player is grounded and presses jump button,
        //sets coyote time counter
        if (IsGrounded() || (context.performed && _remainingJumps > 0))
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        //checks whether the player is on the ground and is not jumping,
        //if so, max jumps to make is reset to max.
        if (IsGrounded() && !context.performed)
        {
            _remainingJumps = _maxJumps;
        }

        //when the jump key is pressed, the player jumps a short height
        //when the jump key is held down, the player jumps higher
        if (context.performed)
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        //when a player jumps, immediately before the player lands, if the jump key is pressed, the player will jump even while not currently on the 'ground'. 
        if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);

            _jumpBufferCounter = 0f;

            _remainingJumps--;
        }

        //when the jump key is let go, the player y velocity increases while falling
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            _coyoteTimeCounter = 0f;
        }
    }
    #endregion

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
    #endregion
}