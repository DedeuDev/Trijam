using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Jump")]
    [SerializeField] private float jumpStartSpeed = 16f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float riseGravityMultiplier = 3f;
    [SerializeField] private float fallGravityMultiplier = 3.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 4f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;

    private float moveInput;
    private float lastMoveDirection = 1f;

    private int jumpCount;
    private bool jumpHeld;

    private bool isDashing;
    private bool airDashAvailable;
    private float dashTimer;
    private float dashDirection;

    public AudioSource audioSource;
    public AudioSource audioJump;
    public AudioSource audioDash;


    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
         animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        UpdateGroundedState();

        if (isDashing)
        {
            HandleDash();
            return;
        }

        MovePlayer();
        ApplyJumpGravity();
    }

    private void UpdateGroundedState()
    {
        if (IsGrounded())
        {
            jumpCount = 0;
            airDashAvailable = true;
        }
    }

    void PlaySound(AudioClip song)
    {
        audioSource.clip = song;
        audioSource.Play();
    }


    private void MovePlayer()
    {
        Vector3 velocity = GetVelocity();

        velocity.x = moveInput * moveSpeed;
        velocity.z = 0f;

        SetVelocity(velocity);
    }

    private void TryJump()
    {
        if (IsGrounded())
        {
            jumpCount = 0;
        }

        if (jumpCount >= maxJumpCount) return;

        Vector3 velocity = GetVelocity();

        velocity.y = jumpStartSpeed;

        SetVelocity(velocity);

        jumpCount++;
    }

    private void ApplyJumpGravity()
    {
        Vector3 velocity = GetVelocity();

        if (velocity.y > 0f)
        {
            if (jumpHeld)
            {
                rb.AddForce(Physics.gravity * (riseGravityMultiplier - 1f), ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(Physics.gravity * (lowJumpGravityMultiplier - 1f), ForceMode.Acceleration);
            }
        }
        else if (velocity.y < 0f)
        {
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void CutJump()
    {
        Vector3 velocity = GetVelocity();

        if (velocity.y > 0f)
        {
            velocity.y *= jumpCutMultiplier;
            SetVelocity(velocity);
        }
    }

    private void TryStartDash()
    {
        if (IsGrounded()) return;
        if (!airDashAvailable) return;

        isDashing = true;
        airDashAvailable = false;
        dashTimer = dashDuration;

        if (moveInput != 0f)
        {
            dashDirection = Mathf.Sign(moveInput);
        }
        else
        {
            dashDirection = lastMoveDirection;
        }

        Vector3 velocity = GetVelocity();

        velocity.x = dashDirection * dashSpeed;
        velocity.y = 0f;
        velocity.z = 0f;

        SetVelocity(velocity);
    }

    private void HandleDash()
    {
        dashTimer -= Time.fixedDeltaTime;

        Vector3 velocity = GetVelocity();

        velocity.x = dashDirection * dashSpeed;
        velocity.y = 0f;
        velocity.z = 0f;

        SetVelocity(velocity);

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();

        Debug.Log(moveInput);
        
        if (moveInput != 0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            animator.Play("PlayerWalk");
            audioSource.Play();

            lastMoveDirection = Mathf.Sign(moveInput);
        }
            else 
            {
                
                animator.Play("Idle");
                audioSource.Stop();
            
            }

            if (moveInput > 0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        
        }

        else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

    
    }

    public void OnJump(InputValue value)
    {
        jumpHeld = value.isPressed;

        if (value.isPressed)
        {
            TryJump();
            audioSource.Stop();
            audioJump.Play();
        }
        else
        {
            CutJump();

        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            TryStartDash();
            audioDash.Play();
        }
    }

    private Vector3 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    private void SetVelocity(Vector3 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}