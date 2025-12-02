using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BlackHoleField : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 5f;

    [Header("Scale")]
    public float startScale = 0.2f;
    public float peakScale = 1.5f;
    public float endScale = 0.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Spin")]
    public float spinSpeed = 180f; // degrees per second

    [Header("Color")]
    public Gradient colorOverLife;

    private float spawnTime;
    private Vector3 baseScale;
    private SpriteRenderer sr;

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

        // Spin
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Scale over life (grow then shrink)
        float curveT = scaleCurve.Evaluate(t);
        float targetScale = Mathf.Lerp(startScale, peakScale, curveT);
        // Optionally blend to endScale as it finishes
        targetScale = Mathf.Lerp(targetScale, endScale, t);
        transform.localScale = baseScale * targetScale;

        // Color over life
        if (sr != null && colorOverLife != null)
        {
            sr.color = colorOverLife.Evaluate(t);
        }
    }

    // You can still add trigger logic later when enemies exist.
    void OnTriggerEnter2D(Collider2D col)
    {
        // Big succ will go here when things are suckable :)
    }
}
