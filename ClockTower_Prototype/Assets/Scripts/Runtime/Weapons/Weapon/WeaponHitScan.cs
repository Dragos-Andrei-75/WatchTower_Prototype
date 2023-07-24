using UnityEngine;

public class WeaponHitScan : Weapon
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;

    [Header("HitScan Weapon Attributes")]
    [SerializeField] private WeaponDataHitScan weaponDataHitScan;
    [SerializeField] LayerMask layerDefault;

    protected Transform CharacterCameraTransform
    {
        get { return characterCameraTransform; }
    }

    protected WeaponDataHitScan WeaponDataHitScan
    {
        get { return weaponDataHitScan; }
        set { weaponDataHitScan = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        characterCameraTransform = WeaponTransform.root.GetChild(0).GetComponent<Transform>();
        layerDefault = LayerMask.GetMask("Default");
    }

    protected override void Shoot()
    {
        base.Shoot();
        ShootHitScan();
    }

    private void ShootHitScan()
    {
        RaycastHit hit;
        bool shot;

        for (int i = 0; i < WeaponData.Amount; i++)
        {
            shot = Physics.Raycast(CharacterCameraTransform.position, WeaponData.Directions[i], out hit, weaponDataHitScan.Range, ~layerDefault, QueryTriggerInteraction.Ignore);

            if (shot == true)
            {
                float distance = Vector3.Distance(WeaponTransform.position, hit.point);

                if (hit.transform.GetComponent<ManagerHealth>() != null)
                {
                    ManagerHealth managerHealth = hit.transform.GetComponent<ManagerHealth>();
                    float damage = Mathf.Lerp(WeaponData.DamageMax, WeaponData.DamageMin, distance / WeaponDataHitScan.Range);

                    managerHealth.TakeDamage(damage);

                    if (WeaponData.OnWeaponHit != null) StartCoroutine(WeaponData.OnWeaponHit(managerHealth));
                }

                if (hit.rigidbody != null)
                {
                    Vector3 direction = (hit.point - CharacterCameraTransform.position).normalized;
                    float force = Mathf.Lerp(WeaponData.ForceMax, WeaponData.ForceMin, distance / WeaponDataHitScan.Range);

                    hit.rigidbody.AddForce(direction * force, ForceMode.Impulse);
                }
            }
        }
    }
}
