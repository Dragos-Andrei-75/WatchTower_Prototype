using UnityEngine;

public abstract class WeaponMod : MonoBehaviour
{
    [Header("Weapon Mod Attributes")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Weapon.Fire fire;

    protected WeaponData WeaponData
    {
        get { return weaponData; }
    }

    public Weapon.Fire Fire
    {
        get { return fire; }
    }

    protected virtual void Awake()
    {
        if (fire == Weapon.Fire.Primary) weaponData = GetComponents<Weapon>()[0].WeaponData;
        else if (fire == Weapon.Fire.Secondary) weaponData = GetComponents<Weapon>()[1].WeaponData;
    }
}
