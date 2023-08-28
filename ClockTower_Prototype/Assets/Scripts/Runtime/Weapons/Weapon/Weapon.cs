using UnityEngine;
using System;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Object and Component References")]
    [SerializeField] private Transform weaponTransform;

    [Header("Character Object and Component References")]
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Weapon Base Attributes")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Fire fire;

    public enum Fire : ushort { Primary, Secondary }

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

        for (int i = 0; i < weapon.Length; i++) if (weapon[i] == this) fire = (Fire)i;
    }

    protected virtual void OnEnable() => characterShoot.OnShoot[Convert.ToInt16(fire)] += Shoot;

    protected virtual void OnDisable() => characterShoot.OnShoot[Convert.ToInt16(fire)] -= Shoot;

    protected virtual void Shoot()
    {
        weaponData.Ammunition--;
        weaponData.FireNext = Time.time + weaponData.FireRate;

        if (WeaponData.OnWeaponShoot != null) WeaponData.OnWeaponShoot();

        for (int i = 0; i < WeaponData.Amount; i++)
        {
            WeaponData.Directions[i] = WeaponTransform.forward;

            WeaponData.Directions[i].x += UnityEngine.Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].y += UnityEngine.Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].z += UnityEngine.Random.Range(-WeaponData.Spread, WeaponData.Spread);
        }
    }
}
