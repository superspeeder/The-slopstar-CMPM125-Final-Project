using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public int damage = 1;
    public float hitstunDuration = 0.1f;
    public bool destroyOnHit = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This only fires when we hit a NON-trigger collider.
        // So it will NOT fire when touching the outer aggro trigger,
        // only the inner body collider.

        Enemy enemy = collision.collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            // Deal damage
            enemy.DecrementHealth(damage, hitstunDuration);

            // Optionally spawn your black hole here instead of on the agro ring
            // Example:
            // Instantiate(blackHolePrefab, transform.position, Quaternion.identity);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}

