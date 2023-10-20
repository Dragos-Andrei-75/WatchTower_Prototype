using UnityEngine;

public class WeaponExplosive : WeaponProjectile
{
    [Header("Explosive Weapon Attributes")]
    [SerializeField] private WeaponDataExplosive weaponDataExplosive;
    [SerializeField] private Explosive explosive;

    protected override void SetupProjectile()
    {
        base.SetupProjectile();

        explosive = WeaponDataProjectile.ProjectileObject.GetComponent<Explosive>();
        explosive.Radius = weaponDataExplosive.Radius;
    }
}
