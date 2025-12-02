using UnityEngine;

public class MahSwamp : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("If true, this instance moves forward and drops roots patches.")]
    public bool isProjectile = true;

    [Header("Projectile Movement")]
    public float speed = 5f;          // how fast it crawls
    public int direction = 1;         // 1 = right, -1 = left
    public float travelDistance = 5f; // world units, e.g. 5 tiles if each tile = 1 unit
    public float patchSpacing = 1f;   // distance between roots patches in world units

    [Header("Roots Visual")]
    [Tooltip("Prefab to spawn as roots patches.")]
    public GameObject rootsPrefab;
    public float rootsLifetime = 3f;  // how long each roots patch stays

    private Vector3 startPos;
    private float groundY;
    private float distanceTraveled;
    private float nextPatchDistance;

    void Start()
    {
        if (isProjectile)
        {
            // Lock the projectile to the starting Y (this should be at floor height)
            groundY = transform.position.y;

            // Snap to that Y in case we're slightly off
            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;

            startPos = transform.position;
            distanceTraveled = 0f;
            nextPatchDistance = 0f; // first patch immediately at start

            SpawnPatch(); // drop a roots patch at the starting point
        }
        else
        {
            // Static roots patch: just exist for rootsLifetime, then despawn
            Destroy(gameObject, rootsLifetime);
        }
    }

    void Update()
    {
        if (!isProjectile)
            return;

        // Move horizontally, keep Y locked to groundY so it doesn't "fly"
        Vector3 pos = transform.position;
        pos.x += direction * speed * Time.deltaTime;
        pos.y = groundY;
        transform.position = pos;

        // How far has it gone?
        distanceTraveled = Mathf.Abs(transform.position.x - startPos.x);

        // Drop patches every patchSpacing units
        if (distanceTraveled >= nextPatchDistance)
        {
            SpawnPatch();
            nextPatchDistance += patchSpacing;
        }

        // Stop after travelDistance
        if (distanceTraveled >= travelDistance)
        {
            // Final patch at the end
            SpawnPatch();
            Destroy(gameObject);
        }
    }

    private void SpawnPatch()
    {
        if (rootsPrefab == null)
            return;

        Vector3 patchPos = new Vector3(transform.position.x, groundY, transform.position.z);
        GameObject patchObj = Instantiate(rootsPrefab, patchPos, Quaternion.identity);

        // If the roots prefab ALSO has MahSwamp, put it in patch mode so it just sits + despawns
        MahSwamp swampComp = patchObj.GetComponentInChildren<MahSwamp>();
        if (swampComp != null)
        {
            swampComp.isProjectile = false;
            swampComp.rootsLifetime = rootsLifetime;
        }
        else
        {
            // Visual-only roots prefab: just auto-despawn
            Destroy(patchObj, rootsLifetime);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Later: slow / damage enemies here
    }
}
