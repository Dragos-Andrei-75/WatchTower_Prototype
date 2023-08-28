using UnityEngine;
using System;

public class ManagerMod : MonoBehaviour
{
    [Header("Mod Attributes")]
    [SerializeField] private Weapon.Fire fire;

    [Header("Mod References")]
    [SerializeField] private WeaponMod[] weaponMods;
    [SerializeField] private int weaponModsSize;

    private void Awake()
    {
        WeaponMod[] mods = gameObject.transform.GetComponents<WeaponMod>();

        for (int i = 0; i < mods.Length; i++)
        {
            if (mods[i].Fire == fire)
            {
                weaponModsSize++;

                Array.Resize(ref weaponMods, weaponModsSize);

                weaponMods[weaponModsSize - 1] = gameObject.transform.GetComponents<WeaponMod>()[i];
                weaponMods[weaponModsSize - 1].enabled = false;
            }
        }
    }
}
