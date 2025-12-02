using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]   // Make sure there's a trigger collider for AoE
public class SteamCloud : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 3f;

    [Header("Blind Settings")]
    public float blindDuration = 3f;          // how long enemies stay blind
    public LayerMask enemyMask;               // optional if you use physics checks instead of trigger tags

    [Header("Movement (subtle)")]
    public float riseSpeed = 0.1f;            // slower than before to feel more static
    public float horizontalDriftAmplitude = 0.05f;
    public float horizontalDriftSpeed = 1f;

    [Header("Visuals")]
    public Gradient colorOverLife;

    private float spawnTime;
    private Vector3 startPos;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // Record our starting position when spawned
        startPos = transform.position;
        spawnTime = Time.time;
    }

    void Start()
    {
        // Auto-destroy cloud after its duration
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float elapsed = Time.time - spawnTime;
        float lifeT = Mathf.InverseLerp(0f, duration, elapsed);

        // Very subtle motion so it FEELS like fog but is basically static
        float y = riseSpeed * elapsed;
        float xOffset = Mathf.Sin(Time.time * horizontalDriftSpeed) * horizontalDriftAmplitude;

        transform.position = startPos + new Vector3(xOffset, y, 0f);

        // Color/alpha fade over life
        if (sr != null && colorOverLife != null)
        {
            sr.color = colorOverLife.Evaluate(lifeT);
        }
    }

    // === BLINDING LOGIC HOOKS ===
    // This assumes your enemy will eventually have some "perception" script we can call.

    void OnTriggerEnter2D(Collider2D other)
    {
        TryBlindEnemy(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Optional: re-apply / refresh blind while they're inside
        TryBlindEnemy(other);
    }

    private void TryBlindEnemy(Collider2D other)
    {
        // Replace "EnemyPerception" with whatever script your enemies eventually use
        EnemyPerception enemy = other.GetComponent<EnemyPerception>();
        if (enemy != null)
        {
            enemy.ApplyBlind(blindDuration);
        }
    }
}
