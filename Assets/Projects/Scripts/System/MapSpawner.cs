using System.Collections;
using System.Collections.Generic;
using KHCore;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float spawnHeightOffset = 1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioClip[] groanClips;
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 4f;

    [Header("Level Data")]
    [SerializeField] private LevelInformation levelInfo;
    public Transform playerSpawn;

    private Transform targetPlayer;

    private class SpawnTracker
    {
        public ZombieSpawnInfo info;
        public float timer;
        public int spawned;
    }

    private List<SpawnTracker> trackers = new List<SpawnTracker>();
    private int totalZombieCount = 0;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void StartSpawning(Transform player, LevelInformation levelInfo)
    {
        targetPlayer = player;
        this.levelInfo = levelInfo;
        enabled = true;
        StartCoroutine(GroanLoop());

        trackers.Clear();
        foreach (var info in levelInfo.zombieSpawnInfos)
        {
            trackers.Add(new SpawnTracker
            {
                info = info,
                timer = info.firstSpawnDelay,
                spawned = 0
            });
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;

        foreach (var tracker in trackers)
        {
            if (tracker.spawned >= tracker.info.maxSpawnCount)
                continue;

            tracker.timer -= Time.deltaTime;

            if (tracker.timer <= 0f)
            {
                tracker.timer = tracker.info.spawnInterval;
                SpawnZombie(tracker.info.zombiePrefab);
                tracker.spawned++;
            }
        }
    }

    private void SpawnZombie(GameObject prefab)
    {
        Vector3 pos = GetRandomSpawnPosition();

        Zombie zombie = ServiceLocator.Get<ZombiePool>().Get(prefab, pos);

        totalZombieCount++;

        PlayRandomGroan();
        zombie.OnDeath += () =>
        {
            totalZombieCount--;
        };
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 circle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(circle.x, 0, circle.y);

        if (Physics.Raycast(spawnPos + Vector3.up * spawnHeightOffset, Vector3.down, out RaycastHit hit, 10f, groundMask))
        {
            spawnPos.y = hit.point.y;
        }

        return spawnPos;
    }

    IEnumerator GroanLoop()
    {
        while (true)
        {

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            if (ServiceLocator.Get<GameStateManager>().CurrentState == GAME_STATE.GAME)
            {
                PlayRandomGroan();
            }
        }
    }

    void PlayRandomGroan()
    {
        if (totalZombieCount == 0) return;

        int index = Random.Range(0, SFX_ZOMBIE.NUM_ZOM_SFX);
        ServiceLocator.Get<AudioManager>().PlaySFX(SFX_ZOMBIE.ZOMBIE + index);
    }

    public void Clear()
    {
        ServiceLocator.Get<ZombiePool>().ClearAllZombies();
        totalZombieCount = 0;
        trackers.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
#endif
}
