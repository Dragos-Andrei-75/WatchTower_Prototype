
public class MachineGun : WeaponHitScan
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootMachineGun();

    private void ShootMachineGun()
    {
        base.Shoot();
    }
}
