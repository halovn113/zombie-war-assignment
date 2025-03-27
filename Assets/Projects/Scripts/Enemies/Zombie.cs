using System;
using System.Collections;
using KHCore;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamageable
{
    [SerializeField] private ZombieInformation data;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private SkinnedMeshRenderer[] renderers;
    [SerializeField] private float dissolveDuration = 1.5f;

    private float currentHP;
    private float attackTimer;

    private ZombieAnimation anim;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float knockCooldown = 1f;

    public event Action OnDeath;

    private ENEMY_STATE state = ENEMY_STATE.IDLE;
    private MaterialPropertyBlock propertyBlock;
    private Coroutine dissolveRoutine;

    public GameObject OriginalPrefab { get; private set; }

    public void Init()
    {
        currentHP = data.health;
        state = ENEMY_STATE.IDLE;
        attackTimer = 0;
        col.enabled = true;
        anim.PlayIdle();

    }

    void Awake()
    {
        anim = GetComponent<ZombieAnimation>();
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {

    }

    void Update()
    {
        if (state == ENEMY_STATE.DEAD || ServiceLocator.Get<Player>() == null || ServiceLocator.Get<GameStateManager>().CurrentState != GAME_STATE.GAME)
        {
            return;
        }

        if (state == ENEMY_STATE.GET_HIT)
        {
            knockCooldown -= Time.deltaTime;
            if (knockCooldown <= 0f)
            {
                state = ENEMY_STATE.IDLE;
                knockCooldown = 0;
            }
        }

        if (state == ENEMY_STATE.GET_HIT)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        HandleAttack();

        if (state == ENEMY_STATE.ATTACK)
        {
            return;
        }

        state = ENEMY_STATE.MOVING;

        HandleMoving();
    }

    private void HandleAttack()
    {
        var target = ServiceLocator.Get<Player>().transform;
        float distToPlayer = Vector3.Distance(transform.position, target.position);

        if (distToPlayer <= attackRange)
        {
            if (state != ENEMY_STATE.ATTACK)
            {
                anim.PlayWalk(0);

                Vector3 toPlayer = (target.position - transform.position).normalized;
                if (Vector3.Dot(transform.forward, toPlayer) > 0.5f && attackTimer <= 0f)
                {
                    Attack();
                }
            }

            return;
        }
    }

    private void HandleMoving()
    {
        var target = ServiceLocator.Get<Player>().transform;
        Vector3 dir = (target.position - transform.position);
        Vector3 moveDir = dir.normalized;

        moveDir += AvoidOtherZombies();
        moveDir.Normalize();

        Vector3 nextPos = transform.position + moveDir * data.moveSpeed * Time.deltaTime;

        if (Physics.Raycast(nextPos + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 3f, groundMask))
        {
            nextPos.y = hit.point.y;
        }
        else
        {
            nextPos.y = transform.position.y;
        }

        rb.MovePosition(nextPos);

        if (moveDir != Vector3.zero)
        {
            Vector3 lookDir = new Vector3(moveDir.x, 0, moveDir.z);
            if (lookDir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
            anim.PlayWalk(10f);
        }

    }

    private Vector3 AvoidOtherZombies()
    {
        Vector3 avoidance = Vector3.zero;
        int count = 0;

        Collider[] hits = Physics.OverlapSphere(transform.position, 1f, ServiceLocator.Get<ZombiePool>().avoidObject);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector3 away = transform.position - hit.transform.position;
            avoidance += away.normalized / away.magnitude;
            count++;
        }

        return count > 0 ? avoidance / count : Vector3.zero;
    }

    private void Attack()
    {
        state = ENEMY_STATE.ATTACK;
        attackTimer = data.attackCooldown;
        anim.PlayAttack();
        Invoke(nameof(DamageApply), 0.5f);
        Invoke(nameof(EndAttack), 0.8f);
    }

    private void DamageApply()
    {
        var target = ServiceLocator.Get<Player>().transform;
        float distToPlayer = Vector3.Distance(transform.position, target.position);

        if (distToPlayer <= attackRange)
        {
            target.GetComponent<IDamageable>().TakeDamage(data.damage);
        }
    }

    private void EndAttack()
    {
        if (state == ENEMY_STATE.DEAD)
            return;
        state = ENEMY_STATE.MOVING;
    }

    public void TakeDamage(float dmg)
    {
        if (state == ENEMY_STATE.DEAD)
            return;
        state = ENEMY_STATE.GET_HIT;
        int index = UnityEngine.Random.Range(0, SFX_ZOMBIE_HIT.NUM_ZOM_SFX);
        ServiceLocator.Get<AudioManager>().PlaySFX(SFX_ZOMBIE_HIT.ZOMBIE_HIT + index);

        currentHP -= dmg;
        if (currentHP <= 0f)
        {
            Die();
        }
        else
        {
            anim.PlayHurt();
        }
    }

    private void Die()
    {
        ServiceLocator.Get<GameManager>().UpdateGameScore();
        state = ENEMY_STATE.DEAD;
        col.enabled = false;
        anim.PlayDeath(() =>
        {
            StartDissolve();
        });
    }

    public void StartDissolve()
    {
        if (dissolveRoutine != null)
            StopCoroutine(dissolveRoutine);

        dissolveRoutine = StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        float timer = 0f;

        while (timer < dissolveDuration)
        {
            float height = Mathf.Lerp(-1f, 2f, timer / dissolveDuration);

            foreach (var r in renderers)
            {
                propertyBlock.Clear();
                r.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_CutoffHeight", height);
                r.SetPropertyBlock(propertyBlock);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        OnDeath?.Invoke();
        OnDeath = null;
        ServiceLocator.Get<ZombiePool>().ReturnToPool(this);
    }

    public void SetOrigin(GameObject prefab)
    {
        OriginalPrefab = prefab;
    }

    void OnEnable()
    {
        foreach (var r in renderers)
        {
            propertyBlock.Clear();
            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_CutoffHeight", -1);
            r.SetPropertyBlock(propertyBlock);
        }
    }
}
