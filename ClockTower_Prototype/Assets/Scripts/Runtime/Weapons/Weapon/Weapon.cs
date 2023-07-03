using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Object and Component References")]
    [SerializeField] private Transform weaponTransform;

    [Header("Character Object and Component References")]
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Weapon Base Attributes")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Mode mode;

    public delegate void WeaponShoot();
    public event WeaponShoot OnWeaponShoot;

    private enum Mode : ushort { Primary, Secondary }

    protected Transform WeaponTransform
    {
        get { return weaponTransform; }
    }

    public WeaponData WeaponData
    {
        get { return weaponData; }
        set { weaponData = value; }
    }

    protected virtual void Awake()
    {
        Weapon[] weapon;

        weaponTransform = gameObject.transform;
        characterShoot = weaponTransform.GetComponentInParent<CharacterShoot>();

        WeaponData.FireNext = 0;
        WeaponData.Heat = 0;

        weapon = weaponTransform.GetComponents<Weapon>();

        for (int i = 0; i < weapon.Length; i++) if (weapon[i] == this) mode = (Mode)i;

        SetDirectionsSize();
    }

    protected virtual void OnEnable()
    {
        if (mode == Mode.Primary) characterShoot.OnShootPrimary += Shoot;
        else if (mode == Mode.Secondary) characterShoot.OnShootSecondary += Shoot;

        WeaponData.OnAmountSet += SetDirectionsSize;
    }

    protected virtual void OnDisable()
    {
        if (mode == Mode.Primary) characterShoot.OnShootPrimary -= Shoot;
        else if (mode == Mode.Secondary) characterShoot.OnShootSecondary -= Shoot;

        WeaponData.OnAmountSet -= SetDirectionsSize;
    }

    private void SetDirectionsSize() => WeaponData.Directions = new Vector3[WeaponData.Amount];

    protected virtual void Shoot()
    {
        weaponData.Ammunition--;
        weaponData.FireNext = Time.time + weaponData.FireRate;

        if (OnWeaponShoot != null) OnWeaponShoot();

        for (int i = 0; i < WeaponData.Amount; i++)
        {
            WeaponData.Directions[i] = WeaponTransform.forward;

            WeaponData.Directions[i].x += Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].y += Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].z += Random.Range(-WeaponData.Spread, WeaponData.Spread);
        }
    }
}
