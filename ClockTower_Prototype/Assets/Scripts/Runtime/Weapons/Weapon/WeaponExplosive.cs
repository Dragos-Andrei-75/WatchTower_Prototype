using UnityEngine;

public class WeaponExplosive : WeaponProjectile
{
    [Header("Explosive Weapon Attributes")]
    [SerializeField] private WeaponDataExplosive weaponDataExplosive;

    protected override void ShootPrimary()
    {
        base.ShootPrimary();

        ShootExplosive(new DataProjectile(WeaponDataProjectile.projectileObject[0], WeaponDataProjectile.damageMax[0], WeaponDataProjectile.damageMin[0],
                                          WeaponDataProjectile.forceMax[0], WeaponDataProjectile.forceMin[0], WeaponDataProjectile.speed[0], WeaponDataProjectile.spread[0],
                                          WeaponDataProjectile.lifeSpan[0]), new DataExplosive(weaponDataExplosive.radius[0]), WeaponDataProjectile.ammount[0]);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();

        ShootExplosive(new DataProjectile(WeaponDataProjectile.projectileObject[1], WeaponDataProjectile.damageMax[1], WeaponDataProjectile.damageMin[1],
                                          WeaponDataProjectile.forceMax[1], WeaponDataProjectile.forceMin[1], WeaponDataProjectile.speed[1], WeaponDataProjectile.spread[1],
                                          WeaponDataProjectile.lifeSpan[1]), new DataExplosive(weaponDataExplosive.radius[0]), WeaponDataProjectile.ammount[1]);
    }

    protected void ShootExplosive(DataProjectile dataProjectile, DataExplosive dataExplosive, float ammount)
    {
        base.ShootProjectile(dataProjectile, ammount);

        for (int i = 0; i < ammount; i++) Projectiles[i].GetComponent<Explosive>().ExplosiveData = dataExplosive;
    }
}
