using UnityEngine;

public class Pellet : Projectile
{
    [Header("Pellet Attributes")]
    [SerializeField] private Transform hitTransform;

    private void OnEnable() => OnContact += PelletBehaviour;

    private void OnDisable() => OnContact -= PelletBehaviour;

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        hitTransform = collision.transform;
    }

    private void PelletBehaviour()
    {
        float ratio = LifeSpan / ProjectileData.lifeSpan;

        if (hitTransform.GetComponent<ManagerHealth>() != null)
        {
            ManagerHealth managerHealth = hitTransform.GetComponent<ManagerHealth>();
            float damage = Mathf.Lerp(ProjectileData.damageMax, ProjectileData.damageMin, ratio);

            managerHealth.TakeDamage(damage);
        }

        if (hitTransform.GetComponent<Rigidbody>() != null)
        {
            Rigidbody hitRigidbody = hitTransform.GetComponent<Rigidbody>();
            float force = Mathf.Lerp(ProjectileData.forceMin, ProjectileData.forceMax, ratio);
            hitRigidbody.AddForce(Direction * force, ForceMode.Impulse);
        }
    }
}
