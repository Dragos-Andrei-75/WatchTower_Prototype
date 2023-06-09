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
        ShootHitScan(0);
    }

    protected override void ShootSecondary()
    {
        base.ShootSecondary();
        ShootHitScan(1);
    }

    protected void ShootHitScan(int index)
    {
        RaycastHit hit;
        LayerMask layerDefault;
        float accuracyLimitUpper;
        float accuracyLimitLower;
        bool shot;

        layerDefault = LayerMask.GetMask("Default");

        for (int i = 0; i < WeaponData.amount[index]; i++)
        {
            accuracyLimitUpper = Mathf.Lerp(0, weaponDataHitScan.accuracyMax[index], WeaponData.heat[index] / WeaponData.heatMax[index]);
            accuracyLimitLower = Mathf.Lerp(0, weaponDataHitScan.accuracyMin[index], WeaponData.heat[index] / WeaponData.heatMax[index]);

            WeaponData.direction = CharacterCameraTransform.forward;

            WeaponData.direction.x += Random.Range(accuracyLimitLower, accuracyLimitUpper);
            WeaponData.direction.y += Random.Range(accuracyLimitLower, accuracyLimitUpper);
            WeaponData.direction.z += Random.Range(accuracyLimitLower, accuracyLimitUpper);

            shot = Physics.Raycast(CharacterCameraTransform.position, WeaponData.direction, out hit, weaponDataHitScan.range[index], ~layerDefault, QueryTriggerInteraction.Ignore);

            if (shot == true)
            {
                float distance = Vector3.Distance(WeaponTransform.position, hit.point);
                float damage = Mathf.Lerp(WeaponData.damageMax[index], WeaponData.damageMin[index], distance / WeaponDataHitScan.range[index]);
                float force = Mathf.Lerp(WeaponData.forceMax[index], WeaponData.forceMin[index], distance / WeaponDataHitScan.range[index]);

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
