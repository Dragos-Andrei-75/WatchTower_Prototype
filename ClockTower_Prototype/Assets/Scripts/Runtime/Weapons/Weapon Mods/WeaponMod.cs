using UnityEngine;

public abstract class WeaponMod : MonoBehaviour
{
    [Header("Weapon Mod Attributes")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private ModFire fireMod;

    protected enum ModFire : ushort { Primary, Secondary };

    protected WeaponData WeaponData
    {
        get { return weaponData; }
    }

    protected ModFire FireMod
    {
        get { return fireMod; }
    }

    protected void Awake()
    {
        if (fireMod == ModFire.Primary) weaponData = GetComponents<Weapon>()[0].WeaponData;
        else if (fireMod == ModFire.Secondary) weaponData = GetComponents<Weapon>()[1].WeaponData;
    }
}
