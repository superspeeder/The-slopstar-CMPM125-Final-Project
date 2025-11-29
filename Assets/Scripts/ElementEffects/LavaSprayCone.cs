using UnityEngine;

public class LavaSprayCone : MonoBehaviour
{
    public float duration = 6f;
    public Transform player;
    public int direction;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position = player.position + new Vector3(direction * 0.8f, -0.2f, 0);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Ground DoT here, please gib enemies to roast
    }
}
