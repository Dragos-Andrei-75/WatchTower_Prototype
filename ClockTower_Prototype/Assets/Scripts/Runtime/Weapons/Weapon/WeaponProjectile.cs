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
        ShootProjectile(0);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();
        ShootProjectile(1);
    }

    protected virtual void ShootProjectile(int index)
    {
        DataProjectile projectileData;
        float spread;

        spread = Mathf.Lerp(weaponDataProjectile.spreadMin[index], weaponDataProjectile.spreadMax[index], WeaponData.heat[index] / WeaponData.heatMax[index]);

        projectiles = new GameObject[WeaponData.amount[index]];

        for (int i = 0; i < projectiles.Length; i++)
        {
            WeaponData.direction = WeaponTransform.forward;
            WeaponData.direction += i % 2 == 0 ? WeaponTransform.up * Random.Range(-spread, 0) : WeaponTransform.up * Random.Range(0, spread);
            WeaponData.direction += i < projectiles.Length / 2 ? WeaponTransform.right * Random.Range(-spread, 0) : WeaponTransform.right * Random.Range(0, spread);

            projectileData = new DataProjectile(WeaponData, weaponDataProjectile, index);

            projectiles[i] = Instantiate(projectileData.projectileObject, projectileData.projectileObject.transform.position, projectileData.projectileObject.transform.rotation);

            projectiles[i].GetComponent<Projectile>().ProjectileData = projectileData;

            projectiles[i].transform.position = weaponDataProjectile.projectilePosition.position;
            projectiles[i].transform.rotation = weaponDataProjectile.projectilePosition.rotation;
        }
    }
}
