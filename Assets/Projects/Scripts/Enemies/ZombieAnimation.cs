using System;
using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
    private Action onDeathComplete;
    [SerializeField] public Animator animator;

    public void PlayWalk(float speed)
    {
        animator.SetFloat("speed", speed);
    }

    public void PlayIdle()
    {
        animator.SetFloat("speed", 0);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("attack");
    }

    public void PlayDeath(Action onComplete)
    {
        animator.SetTrigger("death");
        Invoke(nameof(CallOnDeathComplete), 2f);
        this.onDeathComplete = onComplete;
    }

    public void PlayHurt()
    {
        animator.SetTrigger("damage");
    }

    private void CallOnDeathComplete() => onDeathComplete?.Invoke();
}
