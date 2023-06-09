using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Object and Component References")]
    [SerializeField] private Transform weaponTransform;

    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Weapon Attributes")]
    [SerializeField] private WeaponData weaponData;

    protected Transform WeaponTransform
    {
        get { return weaponTransform; }
    }

    protected Transform CharacterCameraTransform
    {
        get { return characterCameraTransform; }
    }

    public WeaponData WeaponData
    {
        get { return weaponData; }
        set { weaponData = value; }
    }

    protected virtual void Awake()
    {
        weaponTransform = gameObject.transform;

        characterCameraTransform = WeaponTransform.root.GetChild(0).GetComponent<Transform>();
        characterShoot = characterCameraTransform.GetComponent<CharacterShoot>();
    }

    protected virtual void OnEnable()
    {
        characterShoot.OnShootPrimary += ShootPrimary;
        characterShoot.OnShootSecondary += ShootSecondary;
    }

    protected virtual void OnDisable()
    {
        characterShoot.OnShootPrimary -= ShootPrimary;
        characterShoot.OnShootSecondary -= ShootSecondary;

        for (int i = 0; i < weaponData.heat.Length; i++)
        {
            weaponData.fireNext[i] = 0;
            weaponData.heat[i] = 0;
        }
    }

    protected virtual void ShootPrimary() => Shoot(0);

    protected virtual void ShootSecondary() => Shoot(1);

    private void Shoot(int index)
    {
        weaponData.ammunition[index]--;

        weaponData.fireNext[index] = Time.time + weaponData.fireRate[index];
        weaponData.fireRate[index] = Mathf.Lerp(weaponData.fireRateMin[index], weaponData.fireRateMax[index], weaponData.heat[index] / weaponData.heatMax[index]);
    }
}
