using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public enum PlayerState {
    Idle,
    Jump,
    Run,
    Fall,
}

public class PlayerController : MonoBehaviour {
    //[SerializeField] float moveAcceleration = 10f;
    [SerializeField] float walkSp = 5f;
    [SerializeField] float runJumpVelocity = 5f;
    [SerializeField] float idleJumpVelocity = 4.3f;
    [SerializeField] float idleSlowdownScalar = 5f;
    [SerializeField] float groundedDownVelocity = -0.1f;
    [SerializeField] float gravity = -0.2f;
    [SerializeField] float gravityArcPeak = -0.1f;

    [SerializeField] float downGravity = -0.3f;

    //number of 60th of a second periods where the player may jump after walking off a platform
    [SerializeField] float coyoteTime = 3.0f;

    //number of 60th of a second periods after when the player presses jump where a jump will still register given proper criteria
    [SerializeField] float jumpBufferTime = 3.0f;
    [SerializeField] float gVelScalarIntended = 1.0f;
    [SerializeField] float gVelScalarPrevious = 4.0f;
    [SerializeField] float aVelScalarIntended = 1.0f;
    [SerializeField] float aVelScalarPrevious = 8.0f;

    [SerializeField] private Rigidbody2D rb;

    //less verbose linearVelocity
    private Vector2 vel;
    private PlayerState state = PlayerState.Idle;
    private bool jumpBuffer = false;
    private bool inCoyoteTime = false;
    private bool didCoyoteTime = false;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private PlayerInput _playerInput;

    void Start() {
        rb.freezeRotation = true;
        vel = new Vector2(0, 0);

        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];

        StartCoroutine(CustomFixedUpdate());
    }

    IEnumerator CoyoteTimeTimer() {
        inCoyoteTime = true;
        didCoyoteTime = true;
        yield return new WaitForSeconds(coyoteTime / 60);
        inCoyoteTime = false;
    }

    IEnumerator JumpBufferTimer() {
        jumpBuffer = true;
        yield return new WaitForSeconds(jumpBufferTime / 60);
        jumpBuffer = false;
    }

    void Update() {
        if (_jumpAction.triggered)
            StartCoroutine(JumpBufferTimer());
    }

    IEnumerator CustomFixedUpdate() {
        while (true) {
            vel = rb.linearVelocity;

            var temp = Vector2.up * (transform.position.y - 1.05f) + Vector2.right * transform.position.x;
            var isGrounded = Physics2D.Linecast(temp + Vector2.left * 0.3f, temp + Vector2.right * 0.3f);

            float moveInput = _moveAction.ReadValue<float>();
            bool kj = _jumpAction.IsPressed();

            switch (state) {
                case PlayerState.Idle:
                    vel.x /= idleSlowdownScalar;
                    vel.y = groundedDownVelocity;
                    if (jumpBuffer) {
                        vel.y = idleJumpVelocity;
                        state = PlayerState.Jump;
                    }
                    else if (moveInput != 0)
                        state = PlayerState.Run;
                    else if (!didCoyoteTime && !isGrounded)
                        StartCoroutine(CoyoteTimeTimer());
                    else if (!inCoyoteTime)
                        state = PlayerState.Fall;

                    break;
                case PlayerState.Run:
                    vel.x = (moveInput * gVelScalarIntended * walkSp + vel.x * gVelScalarPrevious) /
                            (gVelScalarIntended + gVelScalarPrevious);
                    vel.y = groundedDownVelocity;
                    if (jumpBuffer) {
                        vel.y = runJumpVelocity;
                        state = PlayerState.Jump;
                    }
                    else if (!didCoyoteTime && !isGrounded)
                        StartCoroutine(CoyoteTimeTimer());
                    else if (moveInput == 0.0f)
                        state = PlayerState.Idle;
                    else if (!inCoyoteTime)
                        state = PlayerState.Fall;

                    break;
                case PlayerState.Jump:
                    jumpBuffer = false;
                    if (!kj && vel.y > 0) {
                        vel.y *= 0.6f;
                    }

                    vel.x = (moveInput * aVelScalarIntended * walkSp + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += (vel.y < 1 && kj) ? gravityArcPeak : gravity;
                    if (vel.y < -1)
                        state = PlayerState.Fall;
                    else if (moveInput == 0 && isGrounded && vel.y <= 0)
                        state = PlayerState.Idle;
                    else if (isGrounded && vel.y <= 0)
                        state = PlayerState.Run;
                    break;
                case PlayerState.Fall:
                    vel.x = (moveInput * aVelScalarIntended * walkSp + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += downGravity;
                    if (moveInput == 0 && isGrounded)
                        state = PlayerState.Idle;
                    else if (isGrounded)
                        state = PlayerState.Run;
                    break;
            }

            rb.linearVelocity = vel;
            yield return new WaitForSeconds(1.0f / 60.0f);
        }
    }
}