using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveAcceleration = 10f;
    [SerializeField] float maxMoveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Updraft Settings")]
    [SerializeField] private float maxVerticalSpeedInUpdraft = 10f;
    [SerializeField] private float maxUpdraftHeight = 4f;   // How high the updraft lifts the player
    private float updraftStartY;

    // set by UpdraftZone2D
    private bool inUpdraft = false;
    private float updraftStrength = 0f;

    [Header("Lightning Speed Boost")]
    [SerializeField] private float defaultSpeedMultiplier = 1f;  // usually 1
    private float speedMultiplier = 1f;
    private float speedBoostTimer = 0f;

    private bool isGrounded = false;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.freezeRotation = true;
        speedMultiplier = defaultSpeedMultiplier;
    }

    // Called by UpdraftZone2D
    public void SetInUpdraft(bool active, float strength)
    {
        inUpdraft = active;
        updraftStrength = strength;

        if (active)
        {
            // remember the height where the player entered the updraft
            updraftStartY = transform.position.y;
        }
    }

    // 👇 Called by Lightning ability
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        speedBoostTimer = duration;
    }

    void Update()
    {
        // Handle lightning speed boost timer
        if (speedBoostTimer > 0f)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                speedMultiplier = defaultSpeedMultiplier;
            }
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Vector2 v = rb.linearVelocity;
            v.y = 0f; // reset vertical before jump
            rb.linearVelocity = v;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // Horizontal input
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        float currentAcceleration = moveAcceleration * speedMultiplier;
        float currentMaxMoveSpeed = maxMoveSpeed * speedMultiplier;

        // Apply horizontal force
        rb.AddForce(new Vector2(moveInput * currentAcceleration, 0f), ForceMode2D.Force);

        // Read current velocity
        Vector2 v = rb.linearVelocity;

        // Clamp horizontal speed with boosted max
        v.x = Mathf.Clamp(v.x, -currentMaxMoveSpeed, currentMaxMoveSpeed);

        // Updraft logic — height-limited
        if (inUpdraft)
        {
            float currentHeightAboveStart = transform.position.y - updraftStartY;

            if (currentHeightAboveStart < maxUpdraftHeight &&
                v.y < maxVerticalSpeedInUpdraft)
            {
                v.y += updraftStrength * Time.fixedDeltaTime;
            }
        }

        // Apply final velocity
        rb.linearVelocity = v;
    }

    // Ground detection

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}