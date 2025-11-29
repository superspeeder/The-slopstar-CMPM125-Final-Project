using UnityEngine;

public class BlackHoleField : MonoBehaviour
{
    public float duration = 5f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Luc can I have uhhhhhh the big succ (large size)
    }
}
