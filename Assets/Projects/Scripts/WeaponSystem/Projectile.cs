using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private float maxDistance;
    private Vector3 startPos;
    private Rigidbody rb;

    public void Init(WeaponInformation data)
    {
        speed = data.speed;
        damage = data.damage;
        maxDistance = data.maxDistance;
        startPos = transform.position;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
            }
        }
        gameObject.SetActive(false);
    }
}
