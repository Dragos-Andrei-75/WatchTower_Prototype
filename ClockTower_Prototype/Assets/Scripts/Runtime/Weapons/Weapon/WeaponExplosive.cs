using UnityEngine;

public class WeaponExplosive : WeaponProjectile
{
    [Header("Explosive Weapon Attributes")]
    [SerializeField] private WeaponDataExplosive weaponDataExplosive;

    protected override void Shoot()
    {
        base.Shoot();
        ShootExplosive();
    }

    protected void ShootExplosive()
    {
        base.ShootProjectile();
        for (int i = 0; i < WeaponDataProjectile.Projectiles.Length; i++) WeaponDataProjectile.Projectiles[i].GetComponent<Explosive>().Radius = weaponDataExplosive.Radius;
    }
}
