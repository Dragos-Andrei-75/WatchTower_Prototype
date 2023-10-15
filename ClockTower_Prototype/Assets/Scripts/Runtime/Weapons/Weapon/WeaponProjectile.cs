using UnityEngine;

public class WeaponProjectile : Weapon
{
    [Header("Projectile Weapon Attributes")]
    [SerializeField] private WeaponDataProjectile weaponDataProjectile;
    [SerializeField] private Projectile projectile;

    protected override void Awake()
    {
        base.Awake();
        SetupProjectile();
    }

    private void SetupProjectile()
    {
        projectile = weaponDataProjectile.ProjectileObject.GetComponent<Projectile>();

        projectile.ProjectileData = new DataProjectile(weaponDataProjectile.ProjectileObject, weaponDataProjectile.ProjectileObject.transform,
                                                       WeaponData.DamageMax, WeaponData.DamageMin, WeaponData.ForceMax, WeaponData.ForceMin,
                                                       weaponDataProjectile.Speed, weaponDataProjectile.LifeSpan);

        weaponDataProjectile.ProjectilePosition = WeaponTransform.GetChild(0);
    }

    protected override void Shoot()
    {
        base.Shoot();
        ShootProjectile();
    }

    private void ShootProjectile()
    {
        for (int i = 0; i < WeaponData.Amount; i++)
        {
            projectile.Direction = WeaponData.Directions[i];
            Instantiate(weaponDataProjectile.ProjectileObject, weaponDataProjectile.ProjectilePosition.position, weaponDataProjectile.ProjectilePosition.rotation);
        }
    }
}
