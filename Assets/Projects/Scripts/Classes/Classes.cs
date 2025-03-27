using UnityEngine;

[System.Serializable]
public class ZombieInformation
{
    public float moveSpeed = 1.5f;
    public float health = 30f;
    public float damage = 10f;
    public float attackCooldown = 1.2f;
    public float attackRange = 1.5f;
    public float knockCooldown = 1f;
}

[System.Serializable]
public class WeaponInformation
{
    public string weaponName;
    public GameObject bulletPrefab;
    public float fireRate = 0.2f;
    public float damage = 10f;
    public float bulletSpeed = 30f;
    public float range = 20f;
    public float speed = 20f;
    public float maxDistance = 200f;
    public string sfxName;
    public float animShootSpeed = 1;
    public bool autoTurnOffParticle = true;
}

[System.Serializable]
public class PlayerInformation
{
    public float moveSpeed = 5f;
    public float health = 100f;
    public float rotationSpeed = 10f;
}

[System.Serializable]
public class ZombieSpawnInfo
{
    public GameObject zombiePrefab;
    public int maxSpawnCount;
    public float spawnInterval;
    public float firstSpawnDelay;
}

[System.Serializable]
public class LevelInformation
{
    public float time = 180f;
    public ZombieSpawnInfo[] zombieSpawnInfos;
    public GameObject mapPrefab;
}

[System.Serializable]
public class ZombiePoolInfo
{
    public GameObject zombiePrefab;
    public int initialSize = 10;
}