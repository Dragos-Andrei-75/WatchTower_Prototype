using UnityEngine;

public class WeaponProjectile : Weapon
{
    [Header("Projectile Weapon Attributes")]
    [SerializeField] private WeaponDataProjectile weaponDataProjectile;

    protected WeaponDataProjectile WeaponDataProjectile
    {
        get { return weaponDataProjectile; }
        set { weaponDataProjectile = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        weaponDataProjectile.ProjectilePosition = WeaponTransform.GetChild(0);

        SetProjectilesSize();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        WeaponData.OnAmountSet += SetProjectilesSize;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        WeaponData.OnAmountSet -= SetProjectilesSize;
    }

    private void SetProjectilesSize() => weaponDataProjectile.Projectiles = new GameObject[WeaponData.Amount];

    protected override void Shoot()
    {
        base.Shoot();
        ShootProjectile();
    }

    protected virtual void ShootProjectile()
    {
        for (int i = 0; i < WeaponData.Amount; i++)
        {
            DataProjectile projectileData = new DataProjectile(WeaponData.OnWeaponHit, weaponDataProjectile.ProjectileObject, WeaponData.Directions[i],
                                                               WeaponData.DamageMax, WeaponData.DamageMin, WeaponData.ForceMax, WeaponData.ForceMin,
                                                               weaponDataProjectile.Speed, weaponDataProjectile.LifeSpan);

            weaponDataProjectile.Projectiles[i] = Instantiate(projectileData.projectileObject, projectileData.projectileObject.transform.position, projectileData.projectileObject.transform.rotation);

            weaponDataProjectile.Projectiles[i].GetComponent<Projectile>().ProjectileData = projectileData;

            weaponDataProjectile.Projectiles[i].transform.position = weaponDataProjectile.ProjectilePosition.position;
            weaponDataProjectile.Projectiles[i].transform.rotation = weaponDataProjectile.ProjectilePosition.rotation;
        }
    }
}
