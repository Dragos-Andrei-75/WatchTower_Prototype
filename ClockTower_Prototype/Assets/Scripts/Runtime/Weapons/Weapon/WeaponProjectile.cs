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

        ShootProjectile(new DataProjectile(weaponDataProjectile, 0), weaponDataProjectile.spreadMax[0], weaponDataProjectile.spreadMin[0],
                        WeaponData.heatMax[0], ref WeaponData.heat[0]);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();

        projectiles = new GameObject[weaponDataProjectile.ammount[1]];

        ShootProjectile(new DataProjectile(weaponDataProjectile, 1), weaponDataProjectile.spreadMax[1], weaponDataProjectile.spreadMin[1],
                        WeaponData.heatMax[1], ref WeaponData.heat[1]);
    }

    protected virtual void ShootProjectile(DataProjectile projectileData, float spreadMax, float spreadMin, float heatMax, ref float heat)
    {
        float spread = Mathf.Lerp(spreadMin, spreadMax, heat / heatMax);

        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectileData.projectileObject, projectileData.projectileObject.transform.position, projectileData.projectileObject.transform.rotation);

            projectiles[i].GetComponent<Projectile>().ProjectileData = projectileData;

            projectiles[i].transform.position = weaponDataProjectile.projectilePosition.position;
            projectiles[i].transform.rotation = weaponDataProjectile.projectilePosition.rotation;

            projectiles[i].transform.rotation = Quaternion.RotateTowards(projectiles[i].transform.rotation, Random.rotation, spread);
        }
    }
}
