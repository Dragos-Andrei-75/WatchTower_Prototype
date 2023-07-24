using UnityEngine;

public class Pellet : Projectile
{
    private void OnEnable() => OnContact += PelletBehavior;

    private void OnDisable() => OnContact -= PelletBehavior;

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        HitRigidbody = collision.rigidbody;
    }

    private void PelletBehavior()
    {
        ManagerHealth managerHealth;
        float damage;
        float force;

        damage = Mathf.Lerp(ProjectileData.damageMax, ProjectileData.damageMin, LifeSpan / ProjectileData.lifeSpan);
        force = Mathf.Lerp(ProjectileData.forceMin, ProjectileData.forceMax, LifeSpan / ProjectileData.lifeSpan);

        if (HitRigidbody != null && HitRigidbody.GetComponent<ManagerHealth>() != null)
        {
            managerHealth = HitRigidbody.GetComponent<ManagerHealth>();
            managerHealth.TakeDamage(damage);

            HitRigidbody.AddForce(Direction * force, ForceMode.Impulse);

            if (ProjectileData.OnWeaponHit != null) ProjectileData.OnWeaponHit(managerHealth);
        }
    }
}
