using UnityEngine;
using System.Collections;

public struct DataExplosive
{
    public float radius;

    public DataExplosive(float radius) => this.radius = radius;
}

public class Explosive : Projectile
{
    [Header("Explosive Attributes")]
    [SerializeField] private DataExplosive explosiveData;

    public DataExplosive ExplosiveData
    {
        get { return explosiveData; }
        set { explosiveData = value; }
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

        interactableColliders = Physics.OverlapSphere(HitPosition, explosiveData.radius, LayerInteractable);

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

            damage = Mathf.Lerp(ProjectileData.damageMax, ProjectileData.damageMin, distanceObject / explosiveData.radius);
            force = Mathf.Lerp(ProjectileData.forceMax, ProjectileData.forceMin, distanceObject / explosiveData.radius);

            interactableRigidBodies[i].AddExplosionForce(force, HitPosition, explosiveData.radius, 1, ForceMode.Impulse);

            interactableScripts[i].TakeDamage(damage);
        }

        yield break;
    }
}
