using UnityEngine;

public class MahSwamp : MonoBehaviour
{
    public float duration = 3f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // someBADY ONCE TOLD ME
    }
}
