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

    protected override void ShootPrimary()
    {
        base.ShootPrimary();

        ShootHitScan(weaponDataHitScan.damageMax[0], weaponDataHitScan.damageMin[0], weaponDataHitScan.forceMax[0], weaponDataHitScan.forceMin[0], weaponDataHitScan.accuracyMax[0],
                     weaponDataHitScan.accuracyMin[0], weaponDataHitScan.range[0], weaponDataHitScan.ammount[0]);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();

        ShootHitScan(weaponDataHitScan.damageMax[1], weaponDataHitScan.damageMin[1], weaponDataHitScan.forceMax[1], weaponDataHitScan.forceMin[1], weaponDataHitScan.accuracyMax[1],
                     weaponDataHitScan.accuracyMin[1], weaponDataHitScan.range[1], weaponDataHitScan.ammount[1]);
    }

    protected void ShootHitScan(float damageMax, float damageMin, float forceMax, float forceMin, float accuracyMax, float accuracyMin, float range, float ammount)
    {
        RaycastHit hit;
        LayerMask layerDefault;
        Vector3 directionShot;
        bool shot;

        layerDefault = LayerMask.GetMask("Default");

        for (int i = 0; i < ammount; i++)
        {
            directionShot = CharacterCameraTransform.forward;
            directionShot.x += Random.Range(accuracyMin, accuracyMax);
            directionShot.y += Random.Range(accuracyMin, accuracyMax);
            directionShot.z += Random.Range(accuracyMin, accuracyMax);

            shot = Physics.Raycast(CharacterCameraTransform.position, directionShot, out hit, range, ~layerDefault, QueryTriggerInteraction.Ignore);

            if (shot == true)
            {
                float distance = Vector3.Distance(WeaponTransform.position, hit.point);
                float damage = Mathf.Lerp(damageMax, damageMin, distance / range);
                float force = Mathf.Lerp(forceMax, forceMin, distance / range);

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
