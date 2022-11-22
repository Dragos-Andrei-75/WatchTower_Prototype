using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Weapon Attributes")]
    [SerializeField] private WeaponData weaponData;

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

    private void Awake()
    {
        characterCameraTransform = gameObject.transform.root.GetChild(0).GetComponent<Transform>();
        characterCamera = characterCameraTransform.GetComponent<Camera>();
        characterShoot = characterCameraTransform.GetComponent<CharacterShoot>();
    }

    protected virtual void OnEnable()
    {
        CharacterShoot.OnShoot += Shoot;
    }

    protected virtual void OnDisable()
    {
        CharacterShoot.OnShoot -= Shoot;
        weaponData.fireNext = 0;
    }

    protected abstract void Shoot();
}
