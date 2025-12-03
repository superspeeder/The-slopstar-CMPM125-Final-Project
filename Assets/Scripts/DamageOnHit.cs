using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public int damage = 1;
    public float hitstunDuration = 0.1f;
    public bool destroyOnHit = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponentInParent<Enemy>();
        
        if (enemy != null)
        {
            // Deal damage
            enemy.DecrementHealth(damage, hitstunDuration);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}

