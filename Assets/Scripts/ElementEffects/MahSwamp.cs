using UnityEngine;

public class MahSwamp : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("If true, this object crawls and drops patches. If false, this is a static root patch.")]
    public bool isProjectile = true;

    [Header("Projectile Movement")]
    public float speed = 5f;
    public int direction = 1;
    public float travelDistance = 5f;
    public float patchSpacing = 1f;

    [Header("Roots Patch Settings")]
    public GameObject rootsPrefab;
    public float rootsLifetime = 3f;
    public float slowMultiplier = 0.4f;    // enemy speed multiplier while inside patch

    private float spawnTime;
    private float groundY;
    private Vector3 startPos;
    private float distanceTraveled;
    private float nextPatchDistance;

    void Start()
    {
        if (isProjectile)
        {
            // SNAP TO FLOOR
            groundY = transform.position.y;
            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;

            startPos = transform.position;
            nextPatchDistance = 0f;

            SpawnPatch(); // first roots spot
        }
        else
        {
            // STATIC ROOT PATCH BEHAVIOR
            spawnTime = Time.time;
        }
    }

    void Update()
    {
        if (!isProjectile)
        {
            HandleStaticPatch();
            return;
        }

        // PROJECTILE MOVEMENT
        Vector3 pos = transform.position;
        pos.x += direction * speed * Time.deltaTime;
        pos.y = groundY;
        transform.position = pos;

        distanceTraveled = Mathf.Abs(transform.position.x - startPos.x);

        // PATCH DROPPING
        if (distanceTraveled >= nextPatchDistance)
        {
            SpawnPatch();
            nextPatchDistance += patchSpacing;
        }

        // END OF TRAVEL
        if (distanceTraveled >= travelDistance)
        {
            SpawnPatch();
            Destroy(gameObject);
        }
    }

    // STATIC ROOT PATCH LOGIC
    private void HandleStaticPatch()
    {
        if (Time.time - spawnTime >= rootsLifetime)
        {
            Destroy(gameObject);
        }
    }

    // SPAWN A ROOT PATCH BELOW THIS POINT
    private void SpawnPatch()
    {
        if (rootsPrefab == null)
            return;

        Vector3 patchPos = new Vector3(transform.position.x, groundY, transform.position.z);
        GameObject patchObj = Instantiate(rootsPrefab, patchPos, Quaternion.identity);

        // If rootsPrefab has MahSwamp, configure it as a STATIC PATCH
        MahSwamp patch = patchObj.GetComponent<MahSwamp>();
        if (patch != null)
        {
            patch.isProjectile = false;
            patch.rootsLifetime = rootsLifetime;
            patch.slowMultiplier = slowMultiplier;
        }
    }

    // RETURN TRIGGERED WHEN PROJECTILE HITS ENEMY
    public void BeginReturn(int newDirection)
    {
        isProjectile = true;
        direction = newDirection;

        startPos = transform.position;
        distanceTraveled = 0f;
        nextPatchDistance = 0f;
    }

    // SLOW ENEMIES WHILE THEY ARE INSIDE ROOT PATCH
    private void OnTriggerStay2D(Collider2D col)
    {
        if (isProjectile)
            return; // projectile does not slow enemies

        Enemy enemy = col.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity *= slowMultiplier;
        }
    }
}
