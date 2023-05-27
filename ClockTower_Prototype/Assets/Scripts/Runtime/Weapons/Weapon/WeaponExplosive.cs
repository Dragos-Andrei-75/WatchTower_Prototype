using UnityEngine;

public class WeaponExplosive : WeaponProjectile
{
    [Header("Explosive Weapon Attributes")]
    [SerializeField] private WeaponDataExplosive weaponDataExplosive;

    protected override void ShootPrimary()
    {
        base.ShootPrimary();

        ShootExplosive(new DataProjectile(WeaponDataProjectile, 0), new DataExplosive(weaponDataExplosive.radius[0]), WeaponDataProjectile.spreadMax[0],
                       WeaponDataProjectile.spreadMin[0], WeaponDataProjectile.ammount[0], WeaponData.heatMax[0], ref WeaponData.heat[0]);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();

        ShootExplosive(new DataProjectile(WeaponDataProjectile, 0), new DataExplosive(weaponDataExplosive.radius[1]), WeaponDataProjectile.spreadMax[1],
                       WeaponDataProjectile.spreadMin[1], WeaponDataProjectile.ammount[1], WeaponData.heatMax[1], ref WeaponData.heat[1]);
    }

    protected void ShootExplosive(DataProjectile dataProjectile, DataExplosive dataExplosive, float spreadMax, float spreadMin, float ammount, float heatMax, ref float heat)
    {
        base.ShootProjectile(dataProjectile, spreadMax, spreadMin, heatMax, ref heat);

        for (int i = 0; i < ammount; i++) Projectiles[i].GetComponent<Explosive>().ExplosiveData = dataExplosive;
    }
}
