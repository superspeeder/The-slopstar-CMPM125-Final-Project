using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LavaSprayCone : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 6f;

    [Header("Positioning")]
    public Transform player;
    public int direction = 1;
    public Vector2 baseOffset = new Vector2(0.8f, -0.2f);

    [Header("Visuals")]
    public float verticalBobAmplitude = 0.1f;
    public float verticalBobSpeed = 4f;

    public float startScale = 0.6f;
    public float endScale = 1.2f;
    public AnimationCurve scaleOverLife = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Gradient colorOverLife;

    [Header("Trail Settings")]
    public GameObject lavaPatchPrefab;      // prefab for the ground lava patch
    public float patchSpawnInterval = 0.2f; // how often to drop patches (seconds)

    private float spawnTime;
    private float lastPatchTime;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        spawnTime = Time.time;
        lastPatchTime = Time.time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, duration, Time.time - spawnTime);

        // Follow player with a little bobbing
        if (player != null)
        {
            float bob = Mathf.Sin(Time.time * verticalBobSpeed) * verticalBobAmplitude;
            Vector3 offset = new Vector3(direction * baseOffset.x, baseOffset.y + bob, 0f);
            transform.position = player.position + offset;
        }

        // Grow slightly over time
        float s = Mathf.Lerp(startScale, endScale, scaleOverLife.Evaluate(t));
        transform.localScale = new Vector3(direction * s, s, 1f);

        // Color/alpha over life
        if (sr != null && colorOverLife != null)
        {
            sr.color = colorOverLife.Evaluate(t);
        }

        // Handle lava trail spawning
        TrySpawnPatch();
    }

    private void TrySpawnPatch()
    {
        if (lavaPatchPrefab == null)
            return;

        if (Time.time - lastPatchTime >= patchSpawnInterval)
        {
            // Drop a patch at the cone’s current position
            Instantiate(lavaPatchPrefab, transform.position, Quaternion.identity);
            lastPatchTime = Time.time;
        }
    }
}
