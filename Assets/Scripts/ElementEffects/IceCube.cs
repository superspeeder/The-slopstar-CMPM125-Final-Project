using UnityEngine;

public class IceCube : MonoBehaviour
{
    public float duration = 8f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D EnemyCollider)
    {
        // Please man I need an enemy to freeze (or you guys can be super nice and do it :)))
    }
}
