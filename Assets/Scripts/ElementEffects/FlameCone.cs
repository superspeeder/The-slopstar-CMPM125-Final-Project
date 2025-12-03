using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlameCone : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 1.2f;

    [Header("Positioning")]
    public Transform player;
    public int direction = 1;
    public float offsetDistance = 1.0f;

    [Header("Visuals")]
    public Transform spriteTransform; // child object with SpriteRenderer
    public SpriteRenderer spriteRenderer;

    public float startLength = 0.3f;
    public float endLength = 1.2f;
    public AnimationCurve lengthOverLife = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public Gradient colorOverLife;
    public float flickerAmplitude = 0.1f;
    public float flickerSpeed = 30f;

    private float spawnTime;

    void Awake()
    {
        spawnTime = Time.time;

        // Safety: ensure root collider is trigger
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, duration, Time.time - spawnTime);

        // ---- FOLLOW PLAYER ----
        if (player != null)
        {
            Vector3 offset = new Vector3(direction * offsetDistance, 0f, 0f);
            transform.position = player.position + offset;
        }

        // ---- UPDATE VISUAL SCALE SAFELY ----
        float lengthCurve = lengthOverLife.Evaluate(t);
        float baseLength = Mathf.Lerp(startLength, endLength, lengthCurve);

        float flicker = 1f + Mathf.Sin(Time.time * flickerSpeed) * flickerAmplitude;
        float finalScale = baseLength * flicker;

        // SCALE ONLY THE CHILD (sprite)
        if (spriteTransform != null)
        {
            spriteTransform.localScale = new Vector3(
                direction * finalScale,
                finalScale,
                1f
            );
        }

        // ---- COLOR ----
        if (spriteRenderer != null && colorOverLife != null)
        {
            spriteRenderer.color = colorOverLife.Evaluate(t);
        }
    }
}
