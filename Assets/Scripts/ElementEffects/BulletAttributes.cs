using UnityEngine;

public class BulletAttributes : MonoBehaviour // Bullet base object. Some attacks are just projectiles, but others are projectiles which spawn an AoE on impact.
{
    private float speed;
    private float maxRange;
    private float turnSpeed;
    private bool homing;
    private Transform target;
    private int direction;
    private Vector3 startPos;

    private Vector2 moveDir;
    private bool initialized = false;

    private void OnEnable()
    {
        startPos = transform.position;
        initialized = false;
    }

    public void Init(
        float speed,
        float maxRange,
        int direction,
        bool homing,
        Transform target,
        float turnSpeed
    )
    {
        this.speed = speed;
        this.maxRange = maxRange;
        this.direction = direction;
        this.homing = homing;
        this.target = target;
        this.turnSpeed = turnSpeed;

        startPos = transform.position;
        moveDir = new Vector2(direction, 0).normalized;

        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        if (homing && target != null)
        {
            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;

            moveDir = Vector2.Lerp(
                moveDir,
                toTarget,
                turnSpeed * Time.deltaTime
            ).normalized;
        }

        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= maxRange)
        {
            ProjectilePool.Instance.ReturnToPool(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 🔹 IMPORTANT FIX:
        // Ignore enemy trigger colliders (like your outer "aggro" ring)
        // so bullets don't detonate on them.
        // Got this from help from GPT (This took me so long :(((( //)
        if (col.isTrigger && col.GetComponentInParent<Enemy>() != null)
        {
            // Just entered an enemy's trigger (aggro zone etc.) – keep flying.
            return;
        }

        // At this point, we've hit something "real":
        // - enemy body collider (non-trigger)
        // - wall/ground
        // - or any other collider you want to count as an impact

        AoEEffectSpawner spawner = GetComponent<AoEEffectSpawner>();
        if (spawner != null)
        {
            spawner.SpawnImpactEffect();
        }

        ProjectilePool.Instance.ReturnToPool(this);
    }
}
