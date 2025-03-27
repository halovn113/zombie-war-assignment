using KHCore;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponInformation weaponData;
    public Transform firePoint;
    public ParticleSystem gunFlash;

    private float lastShotTime;

    [SerializeField] private float laserMaxDistance = 5f;
    [SerializeField] private LayerMask hitMask;

    [SerializeField] private LineRenderer lr;

    void Awake()
    {
        lr.positionCount = 2;
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;
        Vector3 end = start + direction * laserMaxDistance;

        if (Physics.Raycast(start, direction, out RaycastHit hit, laserMaxDistance, hitMask))
        {
            end = hit.point;
        }

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public void TryFire()
    {
        if (Time.time - lastShotTime < weaponData.fireRate)
        {
            return;
        }
        ServiceLocator.Get<AudioManager>().PlaySFX(weaponData.sfxName);
        if (gunFlash != null)
        {
            gunFlash.gameObject.SetActive(true);
            gunFlash.Play();
        }
        lastShotTime = Time.time;
        Projectile bullet = ServiceLocator.Get<BulletPool>().GetBullet(weaponData.bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent(out Projectile proj))
        {
            proj.Init(weaponData);
        }
    }

    public void StopFire()
    {
        if (weaponData.autoTurnOffParticle)
        {
            return;
        }
        if (gunFlash != null)
        {
            gunFlash.Stop();
            gunFlash.gameObject.SetActive(false);
        }
    }
}
