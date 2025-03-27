using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Action onDeathComplete;
    [SerializeField] private Animator animator;

    public void PlayWalk()
    {
        animator.SetInteger("State", 1);
    }

    public void PlayIdle()
    {
        animator.SetInteger("State", 0);
    }

    public void PlayAttack(bool isFiring, float speed)
    {
        animator.applyRootMotion = false;
        if (isFiring)
        {
            animator.SetBool("Shooting", true);
            animator.SetFloat("ShootSpeed", speed);
        }
        else
        {
            animator.SetBool("Shooting", false);
            animator.SetFloat("ShootSpeed", speed);
        }
    }

    public void PlayDeath(Action onComplete)
    {
        animator.SetInteger("State", 4);
        Invoke(nameof(CallOnDeathComplete), 1.5f);
        this.onDeathComplete = onComplete;
    }

    private void CallOnDeathComplete()
    {
        animator.applyRootMotion = false;
        onDeathComplete?.Invoke();
    }

}
