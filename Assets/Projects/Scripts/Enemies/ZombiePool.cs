using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour
{
    [SerializeField] public ZombiePoolSO zombiePoolSO;
    [SerializeField] public LayerMask avoidObject;
    private List<Zombie> activeZombies = new();

    private Dictionary<GameObject, ObjectPool<Zombie>> poolMap = new();

    void Awake()
    {
        ServiceLocator.Register(this);

        foreach (var data in zombiePoolSO.zombiePoolDatas)
        {
            if (data.zombiePrefab == null) continue;

            var pool = new ObjectPool<Zombie>(data.zombiePrefab.GetComponent<Zombie>(), data.initialSize, transform);
            poolMap[data.zombiePrefab] = pool;
        }
    }

    public Zombie Get(GameObject prefab, Vector3 position)
    {
        if (!poolMap.TryGetValue(prefab, out var pool))
        {
            Debug.LogWarning("No pool found for , " + prefab.name + " creating new pool dynamically.");
            pool = new ObjectPool<Zombie>(prefab.GetComponent<Zombie>(), 10, transform);
            poolMap[prefab] = pool;
        }

        var zombie = pool.Get(position, Quaternion.identity);
        zombie.Init();
        activeZombies.Add(zombie);

        return zombie;
    }

    public void ReturnToPool(Zombie zombie)
    {
        if (zombie == null)
            return;
        GameObject prefab = zombie.OriginalPrefab;
        if (prefab != null && poolMap.TryGetValue(prefab, out var pool))
        {
            pool.ReturnToPool(zombie);
            activeZombies.Remove(zombie);
        }
        else
        {
            Destroy(zombie.gameObject);
        }
    }

    public void ClearAllZombies()
    {
        foreach (var zombie in activeZombies.ToArray())
        {
            ReturnToPool(zombie);
        }
        activeZombies.Clear();
    }

}