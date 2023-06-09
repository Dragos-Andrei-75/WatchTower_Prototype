using UnityEngine;
using System.Collections;

public class Explosive : Projectile
{
    [Header("Explosive Attributes")]
    [SerializeField] private float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    private void OnEnable() => OnContact += ExplosiveBehaviour;

    private void OnDisable() => OnContact -= ExplosiveBehaviour;

    private IEnumerator ExplosiveBehaviour()
    {
        Collider[] interactableColliders;
        Rigidbody[] interactableRigidBodies;
        Interactable[] interactableScripts;
        float distanceObject;
        float damage;
        float force;

        interactableColliders = Physics.OverlapSphere(HitPosition, radius, LayerInteractable);

        interactableRigidBodies = new Rigidbody[interactableColliders.Length];
        interactableScripts = new Interactable[interactableColliders.Length];

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            interactableRigidBodies[i] = interactableColliders[i].GetComponent<Rigidbody>();
            interactableScripts[i] = interactableColliders[i].GetComponent<Interactable>();
        }

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            distanceObject = Vector3.Distance(interactableColliders[i].transform.position, HitPosition);

            damage = Mathf.Lerp(ProjectileData.damageMax, ProjectileData.damageMin, distanceObject / radius);
            force = Mathf.Lerp(ProjectileData.forceMax, ProjectileData.forceMin, distanceObject / radius);

            interactableRigidBodies[i].AddExplosionForce(force, HitPosition, radius, 1, ForceMode.Impulse);

            interactableScripts[i].TakeDamage(damage);
        }

        yield break;
    }
}
