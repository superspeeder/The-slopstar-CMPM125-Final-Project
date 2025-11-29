using UnityEngine;

public class SteamCloud : MonoBehaviour
{
    public float duration = 3f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerStay2D(Collider2D EnemyCollider)
    {
        // lmao get steamed nerd (still need enemies to steam)
    }
}
