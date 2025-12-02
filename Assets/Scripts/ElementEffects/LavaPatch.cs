using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LavaPatch : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifetime = 1.5f; // 🔥 how long a patch stays alive

    [Header("Visuals")]
    public Gradient colorOverLife; // optional fade effect

    private float spawnTime;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        float t = Mathf.InverseLerp(0f, lifetime, Time.time - spawnTime);

        // Fade color if gradient is assigned
        if (sr != null && colorOverLife != null)
        {
            sr.color = colorOverLife.Evaluate(t);
        }

        // Destroy when time is up
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
