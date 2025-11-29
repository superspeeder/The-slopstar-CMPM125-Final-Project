using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public enum PlayerState {
    Idle,
    Jump,
    Run,
    Fall,
    Updraft
}

public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] float walkSp = 5f;
    [SerializeField] float runJumpVelocity = 5f;
    [SerializeField] float idleJumpVelocity = 4.3f;
    [SerializeField] float idleSlowdownScalar = 5f;
    [SerializeField] float groundedDownVelocity = -0.1f;
    [SerializeField] float gravity = -0.2f;
    [SerializeField] float gravityArcPeak = -0.1f;

    [SerializeField] float downGravity = -0.3f;

    [SerializeField] float coyoteTime = 3.0f;
    [SerializeField] float jumpBufferTime = 3.0f;
    [SerializeField] float gVelScalarIntended = 1.0f;
    [SerializeField] float gVelScalarPrevious = 4.0f;
    [SerializeField] float aVelScalarIntended = 1.0f;
    [SerializeField] float aVelScalarPrevious = 8.0f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Updraft Settings")]
    [SerializeField] private float maxVerticalSpeedInUpdraft = 10f;
    [SerializeField] private float maxUpdraftHeight = 4f;
    private float updraftStartY;

    private Vector2 vel;
    private PlayerState pState = PlayerState.Idle;
    private bool jumpBuffer = false;
    private bool inCoyoteTime = false;
    private bool didCoyoteTime = false;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private PlayerInput _playerInput;

    private bool inUpdraft = false;
    private float updraftStrength = 0f;

    [Header("Lightning Speed Boost")]
    [SerializeField] private float defaultSpeedMultiplier = 1f;
    private float speedMultiplier = 1f;
    public int direction = 1;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        rb.freezeRotation = true;
        speedMultiplier = defaultSpeedMultiplier;
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

    public void SetInUpdraft(bool active, float strength)
    {
        inUpdraft = active;
        updraftStrength = strength;
        if (active && (pState == PlayerState.Fall || pState == PlayerState.Jump)){
            pState = PlayerState.Updraft;
            updraftStartY = transform.position.y;
        }
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        StartCoroutine(ResetSpeedBoost(duration));
    }

    IEnumerator ResetSpeedBoost(float d){
        yield return new WaitForSeconds(d);
        speedMultiplier = defaultSpeedMultiplier;
    }

    void Update() {
        if (_jumpAction.triggered)
            StartCoroutine(JumpBufferTimer());
    }

    IEnumerator CustomFixedUpdate() {
        while (true) {
            vel = rb.linearVelocity;

            var groundedCheckLinePos = Vector2.up * (transform.position.y - 1.05f) + Vector2.right * transform.position.x;
            var hitOut = Physics2D.Linecast(groundedCheckLinePos + Vector2.left * 0.3f, groundedCheckLinePos + Vector2.right * 0.3f);
            var isGrounded = hitOut ? hitOut.transform.tag == "Wall" : false;

            float moveInput = _moveAction.ReadValue<float>();
            bool kj = _jumpAction.IsPressed();

            if (moveInput > 0f)
            {
                direction = 1;
            }
            else if (moveInput < 0f)
            {
                direction = -1;
            }

            switch (pState) {
                case PlayerState.Idle:
                    vel.x /= idleSlowdownScalar;
                    vel.y = groundedDownVelocity;
                    if (jumpBuffer) {
                        vel.y = idleJumpVelocity;
                        pState = PlayerState.Jump;
                    }
                    else if (moveInput != 0)
                        pState = PlayerState.Run;
                    else if (!didCoyoteTime && !isGrounded)
                        StartCoroutine(CoyoteTimeTimer());
                    else if (!inCoyoteTime)
                        pState = PlayerState.Fall;
                break;
                case PlayerState.Run:
                    vel.x = (moveInput * gVelScalarIntended * walkSp * speedMultiplier + vel.x * gVelScalarPrevious) /
                            (gVelScalarIntended + gVelScalarPrevious);
                    vel.y = groundedDownVelocity;
                    if (jumpBuffer) {
                        vel.y = runJumpVelocity;
                        pState = PlayerState.Jump;
                    }
                    else if (!didCoyoteTime && !isGrounded)
                        StartCoroutine(CoyoteTimeTimer());
                    else if (moveInput == 0.0f)
                        pState = PlayerState.Idle;
                    else if (!inCoyoteTime)
                        pState = PlayerState.Fall;
                break;
                case PlayerState.Jump:
                    jumpBuffer = false;
                    if (!kj && vel.y > 0) {
                        vel.y *= 0.6f;
                    }
                    vel.x = (moveInput * aVelScalarIntended * walkSp * speedMultiplier + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += (vel.y < 1 && kj) ? gravityArcPeak : gravity;
                    if (vel.y < -1)
                        pState = PlayerState.Fall;
                    else if (moveInput == 0 && isGrounded && vel.y <= 0)
                        pState = PlayerState.Idle;
                    else if (isGrounded && vel.y <= 0)
                        pState = PlayerState.Run;
                break;
                case PlayerState.Fall:
                    vel.x = (moveInput * aVelScalarIntended * walkSp * speedMultiplier + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += downGravity;
                    if (moveInput == 0 && isGrounded)
                        pState = PlayerState.Idle;
                    else if (isGrounded)
                        pState = PlayerState.Run;
                break;
                case PlayerState.Updraft:
                    float currentHeightAboveStart = transform.position.y - updraftStartY;

                    if (currentHeightAboveStart < maxUpdraftHeight && vel.y < maxVerticalSpeedInUpdraft)
                        vel.y += updraftStrength;
                    
                    goto case PlayerState.Fall;
            }
            
            rb.linearVelocity = vel;
            yield return new WaitForSeconds(1.0f / 60.0f);
        }
    }
}
