using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField]
    private ObjectPool<Projectile> bulletPool;

    private Dictionary<GameObject, ObjectPool<Projectile>> bulletPools = new Dictionary<GameObject, ObjectPool<Projectile>>();


    void Awake()
    {
        ServiceLocator.Register(this);
        bulletPool = new ObjectPool<Projectile>(bulletPrefab, 30);
    }

    public Projectile GetBullet(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!bulletPools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<Projectile>(prefab.GetComponent<Projectile>(), 20);
            bulletPools[prefab] = pool;
        }

        return pool.Get(position, rotation);
    }

    public void Return(GameObject prefab, Projectile bullet)
    {
        if (bulletPools.TryGetValue(prefab, out var pool))
        {
            pool.ReturnToPool(bullet);
        }
    }
}
