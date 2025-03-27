using System.Collections.Generic;
using UnityEngine;

public class GrenadePool : MonoBehaviour
{
    [SerializeField] private Grenade grenadePrefab;
    [SerializeField] private int initialSize = 10;

    private Queue<Grenade> pool = new Queue<Grenade>();

    void Awake()
    {
        ServiceLocator.Register(this);
        for (int i = 0; i < initialSize; i++)
            AddNewToPool();
    }

    private void AddNewToPool()
    {
        var g = Instantiate(grenadePrefab, transform);
        g.gameObject.SetActive(false);
        pool.Enqueue(g);
    }

    public Grenade Get()
    {
        if (pool.Count == 0) AddNewToPool();

        var g = pool.Dequeue();
        g.gameObject.SetActive(true);
        return g;
    }

    public void Return(Grenade g)
    {
        g.gameObject.SetActive(false);
        pool.Enqueue(g);
    }
}