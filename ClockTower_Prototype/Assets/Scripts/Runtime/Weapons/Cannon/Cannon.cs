using UnityEngine;

public class Cannon : WeaponProjectile
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootCannon();

    private void ShootCannon()
    {
        base.Shoot();

        Vector3 cannonBallPosition = WeaponTransform.position + WeaponTransform.forward * 0.55f;
        Quaternion cannonBallRotation = WeaponTransform.rotation;

        Instantiate(WeaponDataProjectile.projectile, cannonBallPosition, cannonBallRotation);
    }
}
