using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Object and Component References")]
    [SerializeField] private Transform weaponTransform;

    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Camera characterCamera;
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

    protected Camera CharacterCamera
    {
        get { return characterCamera; }
    }

    protected CharacterShoot CharacterShoot
    {
        get { return characterShoot; }
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
        characterCamera = characterCameraTransform.GetComponent<Camera>();
        characterShoot = characterCameraTransform.GetComponent<CharacterShoot>();
    }

    protected virtual void OnEnable()
    {
        CharacterShoot.OnShootPrimary += ShootPrimary;
        CharacterShoot.OnShootSecondary -= ShootSecondary;
    }

    protected virtual void OnDisable()
    {
        CharacterShoot.OnShootPrimary -= ShootPrimary;
        CharacterShoot.OnShootSecondary -= ShootSecondary;

        weaponData.fireNext[0] = 0;
        weaponData.fireNext[1] = 0;
    }

    protected virtual void ShootPrimary()
    {
        weaponData.ammunition[0]--;
        weaponData.fireNext[0] = Time.time + weaponData.fireRate[0];
    }

    protected virtual void ShootSecondary()
    {
        weaponData.ammunition[1]--;
        weaponData.fireNext[1] = Time.time + weaponData.fireRate[1];
    }
}
