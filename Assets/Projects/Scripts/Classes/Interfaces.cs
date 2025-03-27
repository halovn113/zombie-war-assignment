using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}