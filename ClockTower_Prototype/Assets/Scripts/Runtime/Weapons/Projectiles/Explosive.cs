using UnityEngine;

public class Explosive : Projectile
{
    [Header("Explosive Attributes")]
    [SerializeField] private float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    private void OnEnable() => OnContact += ExplosiveBehavior;

    private void OnDisable() => OnContact -= ExplosiveBehavior;

    private void ExplosiveBehavior()
    {
        Collider[] colliders;
        Rigidbody[] rigidBodies;
        ManagerHealth[] healthManagers;
        float distanceObject;
        float damage;
        float force;

        colliders = Physics.OverlapSphere(HitPosition, radius, LayerInteractable);

        rigidBodies = new Rigidbody[colliders.Length];
        healthManagers = new ManagerHealth[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            rigidBodies[i] = colliders[i].GetComponent<Rigidbody>();
            healthManagers[i] = colliders[i].GetComponent<ManagerHealth>();
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            distanceObject = Vector3.Distance(colliders[i].transform.position, HitPosition);

            damage = Mathf.Lerp(ProjectileData.damageMax, ProjectileData.damageMin, distanceObject / radius);
            force = Mathf.Lerp(ProjectileData.forceMax, ProjectileData.forceMin, distanceObject / radius);

            rigidBodies[i].AddExplosionForce(force, HitPosition, radius, 1, ForceMode.Impulse);

            healthManagers[i].TakeDamage(damage);

            if (ProjectileData.OnWeaponHit != null) ProjectileData.OnWeaponHit(healthManagers[i]);
        }
    }
}
