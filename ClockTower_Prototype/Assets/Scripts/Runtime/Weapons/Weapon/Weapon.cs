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
        weaponTransform = gameObject.transform;
        characterShoot = weaponTransform.GetComponentInParent<CharacterShoot>();

        WeaponData.Directions = new Vector3[WeaponData.Amount];
    }

    protected virtual void Start()
    {
        WeaponData.Spread = weaponData.SpreadMin;
        WeaponData.FireRate = weaponData.FireRateMin;
        WeaponData.FireNext = 0;
        WeaponData.Amount = WeaponData.AmountMin;
    }

    protected void OnEnable()
    {
        if (mode == Mode.Primary) characterShoot.OnShootPrimary += Shoot;
        else if (mode == Mode.Secondary) characterShoot.OnShootSecondary += Shoot;
    }

    protected void OnDisable()
    {
        if (mode == Mode.Primary) characterShoot.OnShootPrimary -= Shoot;
        else if (mode == Mode.Secondary) characterShoot.OnShootSecondary -= Shoot;
    }

    protected virtual void Shoot()
    {
        weaponData.Ammunition--;
        weaponData.FireNext = Time.time + weaponData.FireRate;
        weaponData.FireRate = Mathf.Lerp(WeaponData.FireRateMin, WeaponData.FireRateMax, WeaponData.Heat / WeaponData.HeatMax);
        weaponData.Spread = Mathf.Lerp(WeaponData.SpreadMin, WeaponData.SpreadMax, WeaponData.Heat / WeaponData.HeatMax);

        for (int i = 0; i < WeaponData.Directions.Length; i++)
        {
            WeaponData.Directions[i] = WeaponTransform.forward;

            WeaponData.Directions[i].x += Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].y += Random.Range(-WeaponData.Spread, WeaponData.Spread);
            WeaponData.Directions[i].z += Random.Range(-WeaponData.Spread, WeaponData.Spread);
        }
    }
}
