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

    private void ShootExplosive()
    {
        //for (int i = 0; i < WeaponData.Amount; i++) WeaponDataProjectile.Projectiles[i].GetComponent<Explosive>().Radius = weaponDataExplosive.Radius;
    }
}
