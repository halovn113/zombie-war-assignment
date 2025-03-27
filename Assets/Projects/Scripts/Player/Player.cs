using System;
using KHCore;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    public PlayerInformation stats;
    [SerializeField]
    public PlayerShooting shooting;
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private PlayerAnimation anim;
    public event Action OnDeath;

    public PLAYER_STATE state = PLAYER_STATE.ALIVE;

    [SerializeField]
    private float currentHP = 100f;
    public float grenadeCooldown = 30f;

    public bool useGrenade = false;

    private float grenadeTimer = 0f;

    void Awake()
    {
        ServiceLocator.Register(this);
        stats = new PlayerInformation();
        ResetStatus();
    }

    public void TakeDamage(float damage)
    {
        if (state == PLAYER_STATE.DEAD)
            return;

        currentHP -= damage;
        var uiGameplay = ServiceLocator.Get<UIManager>().GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>();
        uiGameplay.UpdateHealthBar(currentHP / stats.health);
        int index = UnityEngine.Random.Range(0, SFX_HUMAN_HIT.NUM_ZOM_SFX);
        ServiceLocator.Get<AudioManager>().PlaySFX(SFX_HUMAN_HIT.HUMAN_HIT + index);

        if (currentHP <= 0f)
        {
            Die();
        }
        else
        {
            uiGameplay.TriggerFlash();
        }
    }

    public void ResetStatus()
    {
        currentHP = stats.health;
        anim.PlayIdle();
        grenadeTimer = 0f;
        useGrenade = false;
        var uiGameplay = ServiceLocator.Get<UIManager>().GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>();
        uiGameplay.grenadeCooldown.SetProgress(0);
        uiGameplay.UpdateHealthBar(currentHP / stats.health);
    }

    public void UseGrenade()
    {
        grenadeTimer = grenadeCooldown;
        useGrenade = true;
    }

    private void Die()
    {
        state = PLAYER_STATE.DEAD;
        anim.PlayDeath(() =>
        {
            OnDeath?.Invoke();
            ServiceLocator.Get<GameStateManager>().ChangeState(GAME_STATE.GAME_OVER);
        });
    }


    void Update()
    {
        if (useGrenade)
        {
            grenadeTimer -= Time.deltaTime;
            if (grenadeTimer <= 0f)
            {
                useGrenade = false;
                grenadeTimer = grenadeCooldown;
            }
            ServiceLocator.Get<UIManager>().GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().grenadeCooldown.SetProgress(grenadeTimer / grenadeCooldown);
        }
    }
}
