using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class BlackHoleField : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 5f;

    [Header("Gravity Pull")]
    public float pullStrength = 25f;
    public float pullRadius = 4f;         // larger than your sprite's radius
    public float stopDistance = 0.3f;     // how close to center enemies should go

    [Header("DoT")]
    public int damagePerTick = 1;
    public float tickInterval = 0.25f;

    [Header("Scale")]
    public float startScale = 0.2f;
    public float peakScale = 1.5f;
    public float endScale = 0.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Spin")]
    public float spinSpeed = 180f;

    [Header("Color")]
    public Gradient colorOverLife;

    private float spawnTime;
    private Vector3 baseScale;
    private SpriteRenderer sr;

    // Track DoT cooldown per enemy
    private Dictionary<Enemy, float> nextDamageTime = new Dictionary<Enemy, float>();

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = Vector3.one;
        spawnTime = Time.time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, duration, Time.time - spawnTime);

        // Spin effect
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Scale effect
        float curveT = scaleCurve.Evaluate(t);
        float targetScale = Mathf.Lerp(startScale, peakScale, curveT);
        targetScale = Mathf.Lerp(targetScale, endScale, t);
        transform.localScale = baseScale * targetScale;

        // Color glow
        if (sr != null && colorOverLife != null)
            sr.color = colorOverLife.Evaluate(t);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        // 🔥 Gravity Pull: drag enemy toward center
        Vector2 direction = (Vector2)transform.position - rb.position;
        float dist = direction.magnitude;

        if (dist > stopDistance) // don't overshoot center
        {
            Vector2 pull = direction.normalized * (pullStrength * Time.deltaTime);
            rb.AddForce(pull, ForceMode2D.Force);
        }
        else
        {
            // Pin enemy at center (optional)
            rb.linearVelocity = Vector2.zero;
        }

        // 🔥 Damage over time logic
        float now = Time.time;
        if (!nextDamageTime.TryGetValue(enemy, out float allowedTime) || now >= allowedTime)
        {
            enemy.DecrementHealth(damagePerTick, 0f, ignoreHitstun: true);
            nextDamageTime[enemy] = now + tickInterval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            nextDamageTime.Remove(enemy);
        }
    }
}
