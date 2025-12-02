using UnityEngine;

public class IceCube : MonoBehaviour
{
    [Header("Lifetime")]
    public float duration = 8f;

    [Header("Stasis Settings")]
    public float stasisDuration = 3f;
    public bool destroyOnFirstHit = true;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Try to find an enemy that can be put into stasis
        EnemyStasis stasis = other.GetComponent<EnemyStasis>();
        if (stasis != null)
        {
            stasis.ApplyStasis(stasisDuration);

            if (destroyOnFirstHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
