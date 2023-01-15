
public class LeverActionShotGun : Weapon
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootLeverActionShotGun();

    private void ShootLeverActionShotGun()
    {
    }
}
