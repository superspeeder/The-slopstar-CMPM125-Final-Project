using UnityEngine;
using System.Collections;


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

    [Header("Damage Scaling")]
    public float speedToDamageMultiplier = 0.5f;   // tweak this value


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

        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponent<Collider2D>();

        if (myCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(myCol, playerCol, true);
        }

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

        Enemy enemy = col.collider.GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.Log("[Charge] Hit NON-enemy: " + col.collider.name);
            return;
        }

        Rigidbody2D enemyRb = col.collider.GetComponentInParent<Rigidbody2D>();
        Debug.Log($"[Charge] HIT ENEMY: {enemy.name}, rb found? {enemyRb != null}");

        //
        // 🔥 DAMAGE BASED ON CURRENT SPEED
        //
        float speed = rb.linearVelocity.magnitude;
        int damage = Mathf.Min(20, Mathf.RoundToInt(speed * speedToDamageMultiplier));
        Debug.Log($"[Charge] Speed={speed}, Damage={damage}");

        enemy.DecrementHealth(damage, 0.05f, ignoreHitstun: true);

        //
        // 🔥 APPLY KNOCKBACK
        //
        if (enemyRb != null)
        {
            Vector2 knockback = rb.linearVelocity * 2.5f;

            // scale height by speed (capped)
            float verticalBoost = Mathf.Min(10f, speed * 0.5f);

            knockback.y += verticalBoost;

            enemyRb.AddForce(knockback, ForceMode2D.Impulse);
        }


        //
        // 🔥 TEMPORARY PHYSICS CONTROL FOR ENEMY
        //
        enemy.isKnockedBack = true;
        StartCoroutine(ResetEnemyKnockback(enemy));

        EndCharge();
    }

    private IEnumerator ResetEnemyKnockback(Enemy e)
    {
        yield return new WaitForSeconds(0.25f);
        e.isKnockedBack = false;
    }



    private void EndCharge()
    {
        Debug.Log("[ChargeMovement] EndCharge: destroying charge effect object.");
        active = false;
        Destroy(gameObject);
    }
}