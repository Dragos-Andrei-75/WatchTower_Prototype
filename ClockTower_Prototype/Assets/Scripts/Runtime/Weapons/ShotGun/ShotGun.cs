
public class ShotGun : WeaponHitScan
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootLeverActionShotGun();

    private void ShootLeverActionShotGun()
    {
        base.Shoot();
    }
}
