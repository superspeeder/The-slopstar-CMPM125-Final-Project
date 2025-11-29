using UnityEngine;

public class ChargeMovement : MonoBehaviour // Le bonque has arrived
{
    [Header("Charge Settings")]
    public float accelTime = 0.5f;     
    public float maxChargeSpeed = 48f;
    public float maxDuration = 5f;

    [Header("Collision Settings")]
    public float forceMultiplier = 3f;

    private float time;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool active = false;

    public void Init(PlayerController p)
    {
        player = p;
        rb = p.GetComponent<Rigidbody2D>();
        active = true;
        time = 0f;
    }

    void FixedUpdate()
    {
        if (!active || rb == null) 
            return;

        time += Time.fixedDeltaTime;

        if (time >= maxDuration)
        {
            EndCharge();
            return;
        }

        float factor = Mathf.Clamp01(time / accelTime);
        float effectiveSpeed = maxChargeSpeed * factor;
        rb.linearVelocity = new Vector2(player.direction * effectiveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!active || rb == null) 
            return;

        Rigidbody2D otherRb = col.rigidbody;

        if (otherRb != null)
        {
            Vector2 impactForce = rb.linearVelocity * forceMultiplier;

            otherRb.AddForce(impactForce, ForceMode2D.Impulse);
        }
        EndCharge();
    }

    private void EndCharge()
    {
        active = false;
        Destroy(gameObject);
    }
}
