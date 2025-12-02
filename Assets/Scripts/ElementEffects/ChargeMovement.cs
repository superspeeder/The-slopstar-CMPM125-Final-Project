using UnityEngine;

public class ChargeMovement : MonoBehaviour // Le bonque has arrived
{
    [Header("Charge Settings")]
    public float accelTime = 0.5f;
    public float maxChargeSpeed = 48f;
    public float maxDuration = 5f;

    [Header("Collision Settings")]
    public float forceMultiplier = 3f;

    [Header("Visual Offset")]
    public Vector2 localOffset = Vector2.zero;
    // e.g. (0, -0.5) if you want it at the player's feet

    private float time;
    private PlayerController player;
    private Rigidbody2D rb;
    private Transform followTransform;  // the transform we actually follow
    private bool active = false;

    public void Init(PlayerController p)
    {
        player = p;
        rb = p.GetComponent<Rigidbody2D>();
        time = 0f;
        active = true;

        if (player == null || rb == null)
        {
            Debug.LogError("[ChargeMovement] Init called but player or rb is NULL!");
            return;
        }

        // This is the object that is really moving (usually the rb's transform)
        followTransform = rb.transform;

        // Snap effect to the player/rb position at start
        transform.position = followTransform.position + (Vector3)localOffset;

        Debug.Log($"[ChargeMovement] Init: follow={followTransform.name}, localOffset={localOffset}, worldPos={transform.position}");
    }

    void FixedUpdate()
    {
        if (!active || rb == null || player == null || followTransform == null)
            return;

        time += Time.fixedDeltaTime;

        if (time >= maxDuration)
        {
            EndCharge();
            return;
        }

        // Apply the charge velocity to the PLAYER
        float factor = Mathf.Clamp01(time / accelTime);
        float effectiveSpeed = maxChargeSpeed * factor;
        rb.linearVelocity = new Vector2(player.direction * effectiveSpeed, rb.linearVelocity.y);

        // Make this effect FOLLOW the player/rb every physics frame
        transform.position = followTransform.position + (Vector3)localOffset;
        // Optional debug:
        // Debug.Log($"[ChargeMovement] Following. PlayerPos={followTransform.position}, EffectPos={transform.position}");
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
        Debug.Log("[ChargeMovement] EndCharge: destroying charge effect object.");
        active = false;
        Destroy(gameObject);
    }
}
