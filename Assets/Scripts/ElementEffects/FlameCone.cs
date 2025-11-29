using UnityEngine;

public class FlameCone : MonoBehaviour
{
    public float duration = 1.2f;
    public Transform player;
    public int direction;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position = player.position + new Vector3(direction, 0, 0);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    void OnTriggerStay2D(Collider2D EnemyCollider)
    {
        // EnemyController.health -= 20; <= do when ready
    }

    void OnTriggerExit2D(Collider2D EnemyCollider)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            // EnemyController.health-= 10 * Time.deltaTime; <= do when ready
        }
    }
}
