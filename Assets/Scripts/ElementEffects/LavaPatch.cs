using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LavaPatch : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifetime = 1.5f; // How long a patch stays alive

    [Header("Visuals")]
    public Gradient colorOverLife; // Fade effect

    [Header("Damage Over Time")]
    public int damagePerTick = 1;
    public float tickInterval = 0.4f;
    private float nextTickTime = 0f;


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

    private void OnTriggerStay2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        if (Time.time >= nextTickTime)
        {
            enemy.DecrementHealth(damagePerTick, 0f, ignoreHitstun: true);
            nextTickTime = Time.time + tickInterval;
        }
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
