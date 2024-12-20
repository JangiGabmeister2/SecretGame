using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] float _walkSpeed = 8f;
    private float _moveSpeed;
    [SerializeField] float _jumpSpeed = 20f;
    public Transform orientation;

    [Space(20)] public bool canJump;
    private bool _isJumping;
    private bool _jumpPress;
    [SerializeField] private int _maxJumps = 3;
    [SerializeField] private int _jumpsMade;

    private float _coyoteTime = 0.15f;
    private float _coyoteTimeCounter = 0;

    private float _jumpBufferTime = 0.15f;
    private float _jumpBufferCounter = 0;

    [SerializeField, Space(20)] private bool _canDash;
    private bool _isDashing;
    [SerializeField] private bool _dashActivated = false;
    [SerializeField] private float _dashSpeed = 30f;
    [SerializeField] private float _dashTime = .2f;
    [SerializeField] private float _dashCooldown = 1f;

    [SerializeField, Space(20)] Transform _groundCheck;
    [SerializeField] float _groundCheckDistance = 1f;
    [SerializeField] LayerMask _groundLayer;

    [SerializeField, Space(20)] private float _wallSlideSpeed = 2f;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckDistance = .2f;
    [SerializeField] private LayerMask _wallLayer;

    private float _horizontal;
    private bool _isFacingRight = true;

    private KeyCode[] _jumpKeys = new KeyCode[] { KeyCode.W, KeyCode.Space, KeyCode.UpArrow };
    #endregion

    #region Properties
    Rigidbody2D rb => GetComponent<Rigidbody2D>();
    bool moving => _horizontal != 0;
    #endregion

    #region Methods

    #region Unity Methods
    private void FixedUpdate()
    {
        GroundedMovement();
    }

    private void Update()
    {
        canJump = IsGrounded();

        _moveSpeed = _walkSpeed;

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

        #region Prevent Sliding Movement
        //prevents player from moving horizontally, despite no horizontal input
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

        #region Multiple Jumping + Coyote/Jump Buffer Timing
        //when the player is grounded and presses jump button, sets coyote time counter
        //so when player runs off a platform, even while the player is no longer on the ground, player can still jump
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        //checks whether the player is on the ground and is not jumping,
        //if so, jumps made is reset to 0.
        if (IsGrounded() && !CheckButtonsDown(_jumpKeys))
        {
            _isJumping = false;
            _jumpsMade = 0;
        }

        //when the jump key is pressed, sets jump buffer time
        //so when falling, while player is just above the ground but isn't grounded, pressing jump, still allows player to jump
        if (CheckButtonsDown(_jumpKeys))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        //when the jump key is pressed...
        if (CheckButtonsDown(_jumpKeys))
        {
            //player will jump, so long as they're grounded and jump buffer and coyote times are above 0
            //while not grounded, can perform extra jump/s, so long as they're able to
            if (!_isJumping && _jumpBufferCounter > 0 && _coyoteTimeCounter > 0 || _jumpsMade < _maxJumps)
            {
                _jumpsMade++;

                _isJumping = true;

                rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);

                _jumpBufferCounter = 0f;
            }

            rb.gravityScale = 4f;
            _canDash = true;
        }

        //when the jump key is released...
        if (CheckButtonsUp(_jumpKeys))
        {
            rb.gravityScale = 8f;

            //player jump speed is clamped and falls, making a small jump
            if (!_jumpPress && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

                _coyoteTimeCounter = 0f;
            }
        }
        #endregion
    }
    #endregion

    //moves player upon giving control inputs
    private void GroundedMovement()
    {
        if (!_isDashing)
        {
            float speed = _moveSpeed * _horizontal;

            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(transform.localScale.x * _dashSpeed, 0f);
        }
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

    //when called, the player performs a dashing ability
    //where the player zooms forward for a short amount of time, ignoring gravity and previous y velocity.
    private IEnumerator ExecuteDash()
    {
        _canDash = false;

        float grav = rb.gravityScale;
        rb.gravityScale = 0f;

        _isDashing = true;

        yield return new WaitForSeconds(_dashTime);

        rb.gravityScale = grav;
        _isDashing = false;

        yield return new WaitForSeconds(_dashCooldown);

        _canDash = true;
    }

    private bool CheckButtonHold(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
        {
            if (Input.GetKey(key))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckButtonsDown(KeyCode[] keys)
    {
        foreach (KeyCode keyCode in keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckButtonsUp(KeyCode[] keys)
    {
        foreach (KeyCode keyCode in keys)
        {
            if (Input.GetKeyUp(keyCode))
            {
                return true;
            }
        }

        return false;
    }

    public void EnableDash()
    {
        _dashActivated = true;
    }

    #region Wall Sliding
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

    #region InputSystem Actions
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && _canDash && _dashActivated)
            StartCoroutine(ExecuteDash());
    }
    #endregion

    public void IncreaseMaxJumps()
    {
        _maxJumps++;
    }
    #endregion
}