using UnityEngine;

public class EnemyStasis : MonoBehaviour
{
    public bool IsFrozen => isFrozen;

    private bool isFrozen = false;
    private float stasisEndTime;

    // Optional: references to other enemy systems you want to pause
    // (fill these in once you have movement/AI)
    public MonoBehaviour[] scriptsToDisableDuringStasis;

    public void ApplyStasis(float duration)
    {
        isFrozen = true;
        stasisEndTime = Mathf.Max(stasisEndTime, Time.time + duration);

        // Disable movement/AI scripts while frozen
        foreach (var mb in scriptsToDisableDuringStasis)
        {
            if (mb != null) mb.enabled = false;
        }

        // TODO: Play freeze VFX/animation here
        // e.g. show an ice sprite, change material, etc.
    }

    void Update()
    {
        if (isFrozen && Time.time >= stasisEndTime)
        {
            EndStasis();
        }
    }

    public void BreakStasisEarly()
    {
        if (isFrozen)
        {
            EndStasis();
        }
    }

    private void EndStasis()
    {
        isFrozen = false;

        // Re-enable movement/AI
        foreach (var mb in scriptsToDisableDuringStasis)
        {
            if (mb != null) mb.enabled = true;
        }

        // TODO: Turn off ice VFX/animation here
    }
}
