using UnityEngine;

public class PlayerHatSystem : MonoBehaviour
{
    public Transform hatSocket;
    private GameObject currentHat;

    private PlayerController player; // reference to get direction

    public int hatSortingOrder = 10; // Higher = in front


    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (currentHat != null)
        {
            // Flip the hat based on player direction
            currentHat.transform.localScale = new Vector3(
                player.direction,               // uses 1 or -1
                currentHat.transform.localScale.y,
                currentHat.transform.localScale.z
            );
        }
    }

    public void EquipHat(GameObject hatModel)
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(hatModel, hatSocket);
        currentHat.transform.localPosition = Vector3.zero;
        currentHat.transform.localRotation = Quaternion.identity;

        var hatRenderer = currentHat.GetComponent<SpriteRenderer>();
        var playerRenderer = GetComponent<SpriteRenderer>();

        if (hatRenderer != null && playerRenderer != null)
        {
            if (hatModel.name.Contains("Hat3"))
            {
                // Rabbit ears go behind
                hatRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                hatRenderer.sortingOrder = playerRenderer.sortingOrder - 10;
            }
            else
            {
                // All other hats go in front
                hatRenderer.sortingLayerID = playerRenderer.sortingLayerID;
                hatRenderer.sortingOrder = playerRenderer.sortingOrder + 10;
            }
        }
    }

}
