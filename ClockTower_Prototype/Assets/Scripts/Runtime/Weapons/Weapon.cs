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

    private void Awake()
    {
        weaponTransform = gameObject.transform;

        characterCameraTransform = WeaponTransform.root.GetChild(0).GetComponent<Transform>();
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

    protected virtual void Shoot()
    {
        ShootInteractable();
        ShootEnemy();
    }

    protected void ShootInteractable()
    {
        RaycastHit hit;
        LayerMask layerDefault = LayerMask.GetMask("Default");
        bool shot = Physics.Raycast(CharacterCameraTransform.position, CharacterCameraTransform.forward, out hit, WeaponData.range, ~layerDefault, QueryTriggerInteraction.Ignore);

        if (shot == true)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                if (hit.rigidbody != null)
                {
                    Vector3 direction = hit.point - CharacterCameraTransform.position;

                    direction.Normalize();

                    hit.rigidbody.AddForce(direction * WeaponData.force, ForceMode.Impulse);
                }

                if (hit.transform.GetComponent<Interactable>() != null)
                {
                    Interactable objectInteractive = hit.transform.GetComponent<Interactable>();
                    objectInteractive.TakeDamage(WeaponData.damage);
                }
            }
        }
    }

    private void ShootEnemy()
    {
    }
}
