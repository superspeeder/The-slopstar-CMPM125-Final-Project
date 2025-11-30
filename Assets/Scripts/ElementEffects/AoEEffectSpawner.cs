using UnityEngine;

public class AoEEffectSpawner : MonoBehaviour // AoE spawner. It spawns AoEs.
{
    private GameObject effectPrefab;

    public void Setup(GameObject prefab)
    {
        effectPrefab = prefab;
    }

    public void SpawnImpactEffect()
    {
        if (effectPrefab == null)
            return;

        Instantiate(effectPrefab, transform.position, Quaternion.identity);
        effectPrefab = null;
    }

    private void OnDisable()
    {
        effectPrefab = null;
    }
}
