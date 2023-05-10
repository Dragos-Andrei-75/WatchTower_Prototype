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

        weaponDataProjectile.projectilePosition = WeaponTransform.GetChild(0);
    }

    protected override void ShootPrimary()
    {
        base.ShootPrimary();

        projectiles = new GameObject[weaponDataProjectile.ammount[0]];

        ShootProjectile(new DataProjectile(weaponDataProjectile.projectileObject[0], weaponDataProjectile.damageMax[0], weaponDataProjectile.damageMin[0],
                                           weaponDataProjectile.forceMax[0], weaponDataProjectile.forceMin[0], weaponDataProjectile.speed[0], weaponDataProjectile.spread[0],
                                           weaponDataProjectile.lifeSpan[0]), weaponDataProjectile.ammount[0]);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();

        projectiles = new GameObject[weaponDataProjectile.ammount[1]];

        ShootProjectile(new DataProjectile(weaponDataProjectile.projectileObject[1], weaponDataProjectile.damageMax[1], weaponDataProjectile.damageMin[1],
                                           weaponDataProjectile.forceMax[1], weaponDataProjectile.forceMin[1], weaponDataProjectile.speed[1], weaponDataProjectile.spread[1],
                                           weaponDataProjectile.lifeSpan[1]), weaponDataProjectile.ammount[1]);
    }

    protected virtual void ShootProjectile(DataProjectile projectileData, float ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            projectiles[i] = Instantiate(projectileData.projectileObject, projectileData.projectileObject.transform.position, projectileData.projectileObject.transform.rotation);

            projectiles[i].GetComponent<Projectile>().ProjectileData = projectileData;

            projectiles[i].transform.position = weaponDataProjectile.projectilePosition.position;
            projectiles[i].transform.rotation = weaponDataProjectile.projectilePosition.rotation;

            projectiles[i].transform.rotation = Quaternion.RotateTowards(projectiles[i].transform.rotation, Random.rotation, projectileData.spread);
        }
    }
}
