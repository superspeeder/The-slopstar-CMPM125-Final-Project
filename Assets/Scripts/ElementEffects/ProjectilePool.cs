using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private BulletAttributes projectilePrefab;
    private Queue<BulletAttributes> pool = new Queue<BulletAttributes>();

    void Awake()
    {
        Instance = this;
    }

    public BulletAttributes GetProjectile()
    {
        if (pool.Count > 0)
        {
            var p = pool.Dequeue();
            p.gameObject.SetActive(true);
            return p;
        }

        return Instantiate(projectilePrefab);
    }

    public void ReturnToPool(BulletAttributes proj)
    {
        proj.gameObject.SetActive(false);
        pool.Enqueue(proj);
    }
}
