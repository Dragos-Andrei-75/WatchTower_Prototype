using UnityEngine;
using System.Collections;

public class CannonBall : Projectile
{
    private void Start() => StartCoroutine(CannonBallBehaviour());

    private void OnCollisionEnter(Collision collision)
    {
        HitPosition = ProjectileTransform.position;
        Hit = true;
    }

    private IEnumerator CannonBallBehaviour()
    {
        LayerMask layerInteractable;
        Collider[] interactableColliders;
        Rigidbody[] interactableRigidBodies;
        Interactable[] interactableScripts;
        float distance;
        float damage;
        float force;

        while (Hit == false && DataProjectile.lifeSpan > 0)
        {
            ProjectileTransform.position += ProjectileTransform.forward * DataProjectile.speed * Time.deltaTime;
            DataProjectile.lifeSpan -= Time.deltaTime;

            yield return null;
        }

        layerInteractable = LayerMask.GetMask("Interactable");
        interactableColliders = Physics.OverlapSphere(HitPosition, DataProjectile.radius, layerInteractable);

        interactableRigidBodies = new Rigidbody[interactableColliders.Length];
        interactableScripts = new Interactable[interactableColliders.Length];

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            interactableRigidBodies[i] = interactableColliders[i].GetComponent<Rigidbody>();
            interactableScripts[i] = interactableColliders[i].GetComponent<Interactable>();
        }

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            distance = Vector3.Distance(interactableColliders[i].transform.position, HitPosition);

            damage = Mathf.Lerp(DataProjectile.damageMax, DataProjectile.damageMin, distance / DataProjectile.radius);
            force = Mathf.Lerp(DataProjectile.forceMax, DataProjectile.forceMin, distance / DataProjectile.radius);

            interactableRigidBodies[i].AddExplosionForce(force, HitPosition, DataProjectile.radius, 1, ForceMode.Impulse);

            interactableScripts[i].TakeDamage(damage);
        }

        Destroy(ProjectileObject);

        yield break;
    }
}
