using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    public bool isBlinded { get; private set; }
    private float blindEndTime;

    [Header("Perception")]
    public float normalSightRange = 5f;
    public float blindedSightRange = 0.5f;

    public float CurrentSightRange
    {
        get
        {
            return isBlinded ? blindedSightRange : normalSightRange;
        }
    }

    public void ApplyBlind(float duration)
    {
        isBlinded = true;
        blindEndTime = Mathf.Max(blindEndTime, Time.time + duration);
        // You can trigger a VFX / animation here, e.g. eyes covered, ? mark, etc.
    }

    void Update()
    {
        if (isBlinded && Time.time >= blindEndTime)
        {
            isBlinded = false;
            // Turn off blind visuals here if you added any
        }

        // Your AI / detection code would use CurrentSightRange
        // e.g. Physics2D.OverlapCircle(transform.position, CurrentSightRange, playerMask)
    }
}
