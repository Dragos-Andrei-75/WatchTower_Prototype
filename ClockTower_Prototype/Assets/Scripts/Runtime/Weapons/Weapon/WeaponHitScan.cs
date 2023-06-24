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
                float damage = Mathf.Lerp(WeaponData.DamageMax, WeaponData.DamageMin, distance / WeaponDataHitScan.Range);
                float force = Mathf.Lerp(WeaponData.ForceMax, WeaponData.ForceMin, distance / WeaponDataHitScan.Range);

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    if (hit.transform.GetComponent<Interactable>() != null)
                    {
                        Interactable objectInteractive = hit.transform.GetComponent<Interactable>();
                        objectInteractive.TakeDamage(damage);
                    }

                    if (hit.rigidbody != null)
                    {
                        Vector3 directionPush = (hit.point - CharacterCameraTransform.position).normalized;
                        hit.rigidbody.AddForce(directionPush * force, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
