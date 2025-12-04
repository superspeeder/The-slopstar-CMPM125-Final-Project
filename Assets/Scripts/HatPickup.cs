using UnityEngine;

public class HatPickup : MonoBehaviour
{
    public GameObject hatModel;

    void Awake()
    {
        var r = GetComponent<SpriteRenderer>();
        if (r != null)
        {
            // Make sure hat is above background & visible in world
            r.sortingLayerName = "Player";      // Match your player's layer
            r.sortingOrder = -1;            // Behind player, above background
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHatSystem hatSystem = other.GetComponent<PlayerHatSystem>();
            if (hatSystem != null)
            {
                hatSystem.EquipHat(hatModel);
                Destroy(gameObject);
            }
        }
    }
}
