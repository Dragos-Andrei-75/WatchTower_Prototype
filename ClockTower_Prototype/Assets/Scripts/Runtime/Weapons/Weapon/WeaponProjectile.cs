using UnityEngine;

public class WeaponProjectile : Weapon
{
    [Header("Projectile Weapon Attributes")]
    [SerializeField] private GameObject[] projectiles;
    [SerializeField] private WeaponDataProjectile weaponDataProjectile;

    protected GameObject[] Projectiles
    {
        get { return projectiles; }
        set { projectiles = value; }
    }

    protected WeaponDataProjectile WeaponDataProjectile
    {
        get { return weaponDataProjectile; }
        set { weaponDataProjectile = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        weaponDataProjectile.ProjectilePosition = WeaponTransform.GetChild(0);
        projectiles = new GameObject[WeaponData.Amount];
    }

    protected override void Shoot()
    {
        base.Shoot();
        ShootProjectile();
    }

    protected virtual void ShootProjectile()
    {
        for (int i = 0; i < WeaponData.Amount; i++)
        {
            DataProjectile projectileData = new DataProjectile(weaponDataProjectile.ProjectileObject, WeaponData.Directions[i], WeaponData.DamageMax, WeaponData.DamageMin,
                                                               WeaponData.ForceMax, WeaponData.ForceMin, weaponDataProjectile.Speed, weaponDataProjectile.LifeSpan);

            projectiles[i] = Instantiate(projectileData.projectileObject, projectileData.projectileObject.transform.position, projectileData.projectileObject.transform.rotation);

            projectiles[i].GetComponent<Projectile>().ProjectileData = projectileData;

            projectiles[i].transform.position = weaponDataProjectile.ProjectilePosition.position;
            projectiles[i].transform.rotation = weaponDataProjectile.ProjectilePosition.rotation;
        }
    }
}
