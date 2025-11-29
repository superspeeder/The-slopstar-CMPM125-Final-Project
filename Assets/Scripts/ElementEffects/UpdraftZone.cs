using UnityEngine;

public class UpdraftZone2D : MonoBehaviour
{
    [SerializeField] private float liftStrength = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInUpdraft(true, liftStrength);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInUpdraft(false, 0f);
        }
    }
}
