using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float minJumpHeight = 2f;
    [SerializeField] private float maxJumpHeight = 3f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float bufferTime = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private bool g;

    private Rigidbody2D _rb;
    [SerializeField] public bool _isGrounded { get; private set; }
    [SerializeField] public bool isDoubleJump {  get; private set; }
    private bool _isJumping;
    private bool _doubleJump;
    private int _doubleJumpCount = 0;
    private float _jumpStartY;
    private float _originalGravity;
    private float _coyoteTimer;
    private float _jumpBufferCounter;
    private float _jumpImpulseTime;


    public void Initialize(Rigidbody2D rigidbody)
    {
        _rb = rigidbody;
        _originalGravity = _rb.gravityScale;
        groundLayers = 1 << 0 | 1 << 6;
        _jumpStartY = transform.position.y;
        jumpForce = Mathf.Sqrt(maxJumpHeight * (Physics2D.gravity.y * _rb.gravityScale) * -2) * _rb.mass;
        groundCheck = GetComponentInChildren<Transform>().Find("GroundCheck");
    }

    private void Update()
    {
        g = _isGrounded;
        DecreaseJumpBufferCounter();
        UpdateCoyoteTimer();
    }

    private void FixedUpdate()
    {
        CheckGroundedStatus();
        AdjustGravityScale();
        HandleJump();
        HandleHoldJump();
        HandleDoubleJump();
        ApplyMinimumJumpHeight();
        JumpImpulseTime();

    }

    private void DecreaseJumpBufferCounter()
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void UpdateCoyoteTimer()
    {
        if (_isGrounded)
        {
            _coyoteTimer = coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void JumpImpulseTime()
    {
        if (_jumpImpulseTime > 0 & _rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y / 1.1f);
        }
    }
    private void CheckGroundedStatus()
    {
        _isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.4f), 0, groundLayers); 
    }

    private void HandleJump()
    {
        if (_jumpBufferCounter > 0 && (_isGrounded || _coyoteTimer > 0))
        {
            ExecuteJump();
        }
        if (_isGrounded)
        {
            _doubleJumpCount = 0;
        }
        if (_rb.velocity.y == 0)
        {
            _isJumping = false;
        }
    }

    private void ExecuteJump()
    {
        _rb.gravityScale = _originalGravity;
        _doubleJumpCount++;
        _jumpStartY = transform.position.y;
        _isGrounded = false;
        _coyoteTimer = 0;
        _jumpBufferCounter = 0;
        _rb.AddForce(Vector2.up * jumpForce*3, ForceMode2D.Impulse);
        _jumpImpulseTime = 0.1f;
    }

    private void HandleHoldJump()
    {
        float currentHeight = transform.position.y;

        if (_isJumping)
        {
            _rb.AddForce(new Vector2(_rb.velocity.x, jumpForce * 2), ForceMode2D.Force);

            if (currentHeight > _jumpStartY + maxJumpHeight)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Lerp(_rb.velocity.y * 1f, 0, Time.fixedDeltaTime));
                _isJumping = false;
            }
        }
    }

    private void HandleDoubleJump()
    {
        if (_doubleJump)
        {
            isDoubleJump = true;
            _doubleJumpCount += 2;
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.velocity = Vector2.up * jumpForce * 2f;
            _doubleJump = false;
        }
        else 
        { 
            isDoubleJump = false;
        }
    }

    private void ApplyMinimumJumpHeight()
    {
        float currentHeight = transform.position.y;
        if (currentHeight > _jumpStartY + minJumpHeight)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Lerp(_rb.velocity.y, 0, Time.fixedDeltaTime * 2));
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_jumpBufferCounter <= 0)
        {
            _jumpBufferCounter = bufferTime;
        }

        if (!_isGrounded && _doubleJumpCount <= 1 && _coyoteTimer <=0)
        {
            _doubleJump = true;
        }
    }

    private void AdjustGravityScale()
    {
        if (_rb.velocity.y < 0 && _rb.gravityScale < 6f)
        {
            _rb.gravityScale += 0.3f;
        }
        else if (_rb.velocity.y > 0 && _rb.gravityScale < 6f)
        {
            _rb.gravityScale += 0.04f;
        }
        else if (_rb.velocity.y == 0)
        {
            _rb.gravityScale = _originalGravity;
        }
    }

    public void HoldJump(bool holdJump)
    {
        _isJumping = holdJump;
    }

}


