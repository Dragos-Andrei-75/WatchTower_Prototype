using UnityEngine;

public class WeaponExplosive : WeaponProjectile
{
    [Header("Explosive Weapon Attributes")]
    [SerializeField] private WeaponDataExplosive weaponDataExplosive;

    protected override void ShootPrimary()
    {
        base.ShootPrimary();
        ShootExplosive(0);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();
        ShootExplosive(1);
    }

    protected void ShootExplosive(int index)
    {
        base.ShootProjectile(index);

        for (int i = 0; i < WeaponData.amount[index]; i++) Projectiles[i].GetComponent<Explosive>().Radius = weaponDataExplosive.radius[index];
    }
}
