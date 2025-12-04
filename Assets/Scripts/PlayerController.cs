using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public enum PlayerState {
    Idle,
    Jump,
    Run,
    Fall,
    Updraft,
    Die
}

public class PlayerController : MonoBehaviour {
    [Header("Movement")] [SerializeField] float walkSp = 5f;
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

    [Header("References")] [SerializeField]
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    [Header("Updraft Settings")] [SerializeField]
    private float maxVerticalSpeedInUpdraft = 10f;

    [SerializeField] private float maxUpdraftHeight = 4f; // How high the updraft lifts the player
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

    private GameObject _targetPickup;

    [Header("Lightning Speed Boost")] [SerializeField]
    private float defaultSpeedMultiplier = 1f; // usually 1

    [SerializeField] private TrailRenderer lightningTrailRenderer;

    private float speedMultiplier = 1f;
    public int direction = 1;

    private InputAction _grabAction;

    void Awake() {
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
        _grabAction = _playerInput.actions["Grab"];

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

    // Called by UpdraftZone2D
    public void SetInUpdraft(bool active, float strength) {
        inUpdraft = active;
        updraftStrength = strength;
        if (active && (pState == PlayerState.Fall || pState == PlayerState.Jump)) {
            pState = PlayerState.Updraft;
            updraftStartY = transform.position.y;
        }
    }

    // 👇 Called by Lightning ability
    public void ApplySpeedBoost(float multiplier, float duration) {
        speedMultiplier = multiplier;
        lightningTrailRenderer.emitting = true;
        StartCoroutine(ResetSpeedBoost(duration));
    }

    IEnumerator ResetSpeedBoost(float d) {
        yield return new WaitForSeconds(d);
        speedMultiplier = defaultSpeedMultiplier;
        lightningTrailRenderer.emitting = false;
    }

    IEnumerator DeathTimer(){
        if (pState == PlayerState.Die)
            yield break;
        pState = PlayerState.Die;
        for (int i = 0; i < 100; i++){
            transform.position += Vector3.up/100;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < 90; i++){
            transform.localEulerAngles += Vector3.forward;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.60000002384185791015625f);
    }

    void Update() {
        if (_jumpAction.triggered)
            StartCoroutine(JumpBufferTimer());

        if (_grabAction.triggered && _targetPickup) {
            var pickup = _targetPickup.GetComponent<Pickup>();
            var element = pickup.elementType;
            if (element != ElementType.None) {
                AchievementState.GiveAchievement(element switch {
                    ElementType.Fire => Achievement.PickupFire,
                    ElementType.Water => Achievement.PickupWater,
                    ElementType.Air => Achievement.PickupAir,
                    ElementType.Earth => Achievement.PickupEarth,
                    ElementType.Lightning => Achievement.PickupLightning,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }

            pickup.SetElement(ArmCycler.instance.GetActiveArmElement());
            ArmCycler.instance.SetActiveArmElement(element);
        }
    }

    IEnumerator CustomFixedUpdate() {
        while (true) {
            vel = rb.linearVelocity;

            var groundedCheckLinePos = Vector2.up * (transform.position.y - 0.6f) + Vector2.right * transform.position.x;
            var isGrounded = false;
            RaycastHit2D[] hitOut = new RaycastHit2D[100];
            var len = Physics2D.Linecast(groundedCheckLinePos + Vector2.left * 0.3f, groundedCheckLinePos + Vector2.right * 0.3f, ContactFilter2D.noFilter, hitOut);
            for (int i = 0; i < len && i < 100; i++)
                isGrounded = hitOut[i].transform.tag == "Wall" || isGrounded;

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
            //Debug.Log(pState);

            switch (pState) {
                case PlayerState.Idle:
                    animator.SetBool("isWalking", false);
                    //slow down player if they're still moving
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
                    else if (!inCoyoteTime && !isGrounded)
                        pState = PlayerState.Fall;

                    break;
                case PlayerState.Run:
                    animator.SetBool("isWalking", true);
                    //move player to desired direction
                    vel.x = (moveInput * gVelScalarIntended * walkSp * speedMultiplier + vel.x * gVelScalarPrevious) /
                            (gVelScalarIntended + gVelScalarPrevious);
                    if (vel.x < 0) spriteRenderer.flipX = true;
                    else spriteRenderer.flipX = false;
                    //helps with slopes; push player to meet ground
                    vel.y = groundedDownVelocity;
                    if (jumpBuffer) {
                        vel.y = runJumpVelocity;
                        pState = PlayerState.Jump;
                    }
                    else if (!didCoyoteTime && !isGrounded)
                        StartCoroutine(CoyoteTimeTimer());
                    else if (moveInput == 0.0f) {
                        pState = PlayerState.Idle;
                    }
                    else if (!inCoyoteTime && !isGrounded)
                        pState = PlayerState.Fall;

                    break;
                case PlayerState.Jump:
                    jumpBuffer = false;
                    if (!kj && vel.y > 0) {
                        vel.y *= 0.6f;
                    }
                    if (vel.y < -1)
                        pState = PlayerState.Fall;
                    else if (moveInput == 0 && isGrounded && vel.y <= 0.01f && vel.y >= -0.01f)
                        pState = PlayerState.Idle;
                    else if (isGrounded && vel.y <= 0.01f && vel.y >= -0.01f)
                        pState = PlayerState.Run;

                    //floatier movement in air
                    vel.x = (moveInput * aVelScalarIntended * walkSp * speedMultiplier + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += (vel.y < 1 && kj) ? gravityArcPeak : gravity;
                    break;
                case PlayerState.Fall:
                    if (moveInput == 0 && isGrounded && vel.y <= 0.01f && vel.y >= -0.01f)
                        pState = PlayerState.Idle;
                    else if (isGrounded && vel.y <= 0.01f && vel.y >= -0.01f)
                        pState = PlayerState.Run;
                    vel.x = (moveInput * aVelScalarIntended * walkSp * speedMultiplier + vel.x * aVelScalarPrevious) /
                            (gVelScalarIntended + aVelScalarPrevious);
                    vel.y += downGravity;
                    break;
                // Updraft logic — height-limited
                case PlayerState.Updraft:
                    float currentHeightAboveStart = transform.position.y - updraftStartY;

                    if (currentHeightAboveStart < maxUpdraftHeight && vel.y < maxVerticalSpeedInUpdraft)
                        vel.y += updraftStrength;

                    goto case PlayerState.Fall;
                case PlayerState.Die:
                    vel = Vector2.zero;
                    break;
            }

            // Apply final velocity
            rb.linearVelocity = vel;
            yield return new WaitForSeconds(1.0f / 60.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Pickup")) {
            _targetPickup = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject == _targetPickup) {
            _targetPickup = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.GetComponent<Enemy>())
            StartCoroutine(DeathTimer());
    }
}