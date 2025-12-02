using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlameCone : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 1.2f;

    [Header("Positioning")]
    public Transform player;
    public int direction = 1;
    public float offsetDistance = 1.0f;

    [Header("Visuals")]
    public float startLength = 0.3f;
    public float endLength = 1.2f;
    public AnimationCurve lengthOverLife = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Gradient colorOverLife;
    public float flickerAmplitude = 0.1f;
    public float flickerSpeed = 30f;

    private float spawnTime;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, duration, Time.time - spawnTime);

        // Follow player & face direction
        if (player != null)
        {
            Vector3 baseOffset = new Vector3(direction * offsetDistance, 0f, 0f);
            transform.position = player.position + baseOffset;
        }

        // Scale length over lifetime with a little flicker
        float lengthFactor = lengthOverLife.Evaluate(t);
        float baseLength = Mathf.Lerp(startLength, endLength, lengthFactor);
        float flicker = 1f + Mathf.Sin(Time.time * flickerSpeed) * flickerAmplitude;

        // Assuming the cone extends along +X in sprite space
        transform.localScale = new Vector3(direction * baseLength * flicker, baseLength * flicker, 1f);

        // Color/alpha over lifetime
        if (sr != null && colorOverLife != null)
        {
            sr.color = colorOverLife.Evaluate(t);
        }
    }
}
