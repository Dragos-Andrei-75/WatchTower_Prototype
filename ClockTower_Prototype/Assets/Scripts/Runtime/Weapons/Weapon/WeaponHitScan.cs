using UnityEngine;

public class WeaponHitScan : Weapon
{
    [Header("HitScan Weapon Attributes")]
    [SerializeField] private WeaponDataHitScan weaponDataHitScan;

    protected WeaponDataHitScan WeaponDataHitScan
    {
        get { return weaponDataHitScan; }
        set { weaponDataHitScan = value; }
    }

    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot()
    {
        base.Shoot();
        ShootHitScan();
    }

    protected void ShootHitScan()
    {
        RaycastHit hit;
        LayerMask layerDefault;
        Vector3 directionShot;
        bool shot;

        layerDefault = LayerMask.GetMask("Default");

        for (int i = 0; i < weaponDataHitScan.ammount; i++)
        {
            directionShot = CharacterCameraTransform.forward;
            directionShot.x += Random.Range(weaponDataHitScan.accuracyMin, weaponDataHitScan.accuracyMax);
            directionShot.y += Random.Range(weaponDataHitScan.accuracyMin, weaponDataHitScan.accuracyMax);
            directionShot.z += Random.Range(weaponDataHitScan.accuracyMin, weaponDataHitScan.accuracyMax);

            shot = Physics.Raycast(CharacterCameraTransform.position, directionShot, out hit, WeaponData.range, ~layerDefault, QueryTriggerInteraction.Ignore);

            if (shot == true)
            {
                float distance = Vector3.Distance(WeaponTransform.position, hit.point);

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    if (hit.rigidbody != null)
                    {
                        Vector3 directionPush = (hit.point - CharacterCameraTransform.position).normalized;
                        float force = Mathf.Lerp(WeaponData.forceMax, WeaponData.forceMin, distance / WeaponData.range);

                        hit.rigidbody.AddForce(directionPush * force, ForceMode.Impulse);
                    }

                    if (hit.transform.GetComponent<Interactable>() != null)
                    {
                        Interactable objectInteractive = hit.transform.GetComponent<Interactable>();
                        objectInteractive.TakeDamage(WeaponData.damageMax);
                    }
                }
            }
        }
    }
}
