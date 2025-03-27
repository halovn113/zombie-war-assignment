using System.Collections;
using KHCore;
using PrimeTween;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float fuseTime = 2f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float damage = 50f;
    public LayerMask damageMask;
    public float jumpHeight = 1f;

    [Header("References")]
    public ParticleSystem explosionVFX;

    private Rigidbody rb;
    private bool exploded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 targetPos)
    {
        exploded = false;
        Vector3 startPos = transform.position;
        float jumpHeight = 1f;
        float durationUp = 0.25f;
        float durationDown = 0.25f;

        Vector3 peakPos = Vector3.Lerp(startPos, targetPos, 0.5f);
        peakPos.y += jumpHeight;

        Sequence.Create().Group(Tween.Position(transform, peakPos, durationUp, Ease.OutQuad)).
        Group(Tween.Position(transform, targetPos, durationDown, Ease.InQuad)).OnComplete(() =>
        {
            Explode();
        });
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;
        explosionVFX.gameObject.SetActive(true);
        explosionVFX.Play();
        ServiceLocator.Get<AudioManager>().PlaySFX(SFX_EFFECT.GRENADE);

        Tween.Delay(1.5f, () =>
        {
            ServiceLocator.Get<GrenadePool>().Return(this);
        });

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        foreach (var col in hits)
        {
            var hp = col.GetComponent<Zombie>();
            if (hp != null)
                hp.TakeDamage(damage);

            Rigidbody enemyRb = col.attachedRigidbody;
            if (enemyRb != null)
                enemyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // ServiceLocator.Get<GrenadePool>().Return(this);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
#endif
}