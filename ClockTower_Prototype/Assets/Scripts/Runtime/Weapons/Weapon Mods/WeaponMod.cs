using UnityEngine;

public abstract class WeaponMod : MonoBehaviour
{
    [Header("Weapon Mod Attributes")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Mode mode;

    private enum Mode : ushort { Primary, Secondary };

    protected WeaponData WeaponData
    {
        get { return weaponData; }
    }

    protected virtual void Awake()
    {
        if (mode == Mode.Primary) weaponData = GetComponents<Weapon>()[0].WeaponData;
        else if (mode == Mode.Secondary) weaponData = GetComponents<Weapon>()[1].WeaponData;
    }

    protected void OnEnable()
    {
        if (mode == Mode.Primary) GetComponents<Weapon>()[0].OnWeaponShoot += Mod;
        else if (mode == Mode.Secondary) GetComponents<Weapon>()[1].OnWeaponShoot += Mod;
    }

    protected void OnDisable()
    {
        if (mode == Mode.Primary) GetComponents<Weapon>()[0].OnWeaponShoot -= Mod;
        else if (mode == Mode.Secondary) GetComponents<Weapon>()[1].OnWeaponShoot -= Mod;
    }

    protected abstract void Mod();
}
