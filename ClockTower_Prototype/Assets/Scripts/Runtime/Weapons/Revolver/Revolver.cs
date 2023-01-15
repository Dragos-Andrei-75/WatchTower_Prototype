
public class Revolver : Weapon
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootRevolver();

    private void ShootRevolver() => base.Shoot();
}
