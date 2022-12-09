using UnityEngine;

public class Cannon : Weapon
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    private void Start()
    {
        CannonBall cannonBall = WeaponData.projectile.GetComponent<CannonBall>();

        cannonBall.Damage = WeaponData.damage;
        cannonBall.Force = WeaponData.force;
        cannonBall.Speed = WeaponData.speed;
    }

    protected override void Shoot() => ShootCannon();

    private void ShootCannon()
    {
        Vector3 cannonBallPosition = gameObject.transform.position;
        Quaternion cannonBallRotation = gameObject.transform.rotation;

        Instantiate(WeaponData.projectile, cannonBallPosition, cannonBallRotation);
    }
}
