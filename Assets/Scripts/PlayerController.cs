using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    [SerializeField] float moveAcceleration = 10f;
    [SerializeField] float maxMoveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] private Rigidbody2D rb;
    private bool isGrounded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        rb.AddForce(new Vector2(moveInput * moveAcceleration, 0f), ForceMode2D.Force);
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -maxMoveSpeed, maxMoveSpeed), rb.linearVelocity.y);
    }

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
