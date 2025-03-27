using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(prefab, parent);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);

        if (obj.TryGetComponent(out IPoolable p)) p.OnSpawn();

        return obj;
    }

    public void ReturnToPool(T obj)
    {
        if (obj.TryGetComponent(out IPoolable p)) p.OnDespawn();

        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
