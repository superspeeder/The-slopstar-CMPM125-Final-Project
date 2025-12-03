using UnityEngine;

public class FlameConeDamage : MonoBehaviour
{
    public int damagePerTick = 1;
    public float tickInterval = 0.25f;
    public float hitstunDuration = 0f; // no stun lock

    private float nextTickTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Make sure this only hurts enemies
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        if (Time.time >= nextTickTime)
        {
            // Deal DoT damage without hitstun lock
            enemy.DecrementHealth(damagePerTick, hitstunDuration, ignoreHitstun: true);

            nextTickTime = Time.time + tickInterval;
        }
    }
}
