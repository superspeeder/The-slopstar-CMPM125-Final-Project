using UnityEngine;

public class EnemyStasis : MonoBehaviour
{
    public bool IsFrozen => isFrozen;

    private bool isFrozen = false;
    private float stasisEndTime;

    [Header("What to disable while frozen")]
    public MonoBehaviour[] scriptsToDisableDuringStasis;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 storedVelocity;
    private RigidbodyConstraints2D storedConstraints;
    private float storedAnimatorSpeed = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb != null)
        {
            storedConstraints = rb.constraints;
        }
    }

    public void ApplyStasis(float duration)
    {
        isFrozen = true;
        stasisEndTime = Mathf.Max(stasisEndTime, Time.time + duration);

        // Disable movement/AI scripts while frozen
        foreach (var mb in scriptsToDisableDuringStasis)
        {
            if (mb != null) mb.enabled = false;
        }

        // Freeze physics
        if (rb != null)
        {
            storedVelocity = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Pause animation
        if (animator != null)
        {
            storedAnimatorSpeed = animator.speed;
            animator.speed = 0f;
        }

        // TODO: Play freeze VFX/animation here (ice material, etc.)
    }

    void Update()
    {
        if (isFrozen && Time.time >= stasisEndTime)
        {
            EndStasis();
        }
    }

    public void BreakStasisEarly()
    {
        if (isFrozen)
        {
            EndStasis();
        }
    }

    private void EndStasis()
    {
        isFrozen = false;

        // Re-enable movement/AI
        foreach (var mb in scriptsToDisableDuringStasis)
        {
            if (mb != null) mb.enabled = true;
        }

        // Restore physics
        if (rb != null)
        {
            rb.constraints = storedConstraints;
            rb.linearVelocity = storedVelocity;
        }

        // Resume animation
        if (animator != null)
        {
            animator.speed = storedAnimatorSpeed;
        }

        // TODO: Turn off ice VFX/animation here
    }
}
