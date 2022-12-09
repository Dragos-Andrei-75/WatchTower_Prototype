using UnityEngine;

public class Revolver : Weapon
{
    protected override void OnEnable() => base.OnEnable();

    protected override void OnDisable() => base.OnDisable();

    protected override void Shoot() => ShootRevolver();

    private void ShootRevolver()
    {
        RaycastHit hit;
        LayerMask layerDefault = LayerMask.GetMask("Default");
        bool shot = Physics.Raycast(CharacterCameraTransform.position, CharacterCameraTransform.forward, out hit, WeaponData.range, ~layerDefault);

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
}
