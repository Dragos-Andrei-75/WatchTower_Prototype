using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour
{
    [Header("CannonBall Object and Components References")]
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private Transform cannonBallTransform;

    [Header("CannonBall Attributes")]
    [SerializeField] private Vector3 hitPosition;
    [SerializeField] private float lifeSpan = 10.0f;
    [SerializeField] private float speed = 100f;
    [SerializeField] private float force = 50.0f;
    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float damageMax = 10.0f;
    [SerializeField] private float damageMin = 1.0f;
    [SerializeField] private bool hit = false;

    public float Damage
    {
        get { return damageMax; }
        set
        {
            damageMax = value;
            damageMin = (10 / 100) * value;
        }
    }

    public float Force
    {
        get { return force; }
        set { force = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    private void Start()
    {
        cannonBall = gameObject;
        cannonBallTransform = cannonBall.transform;

        StartCoroutine(CannonBallBehaviour());
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPosition = cannonBallTransform.position;
        hit = true;
    }

    private IEnumerator CannonBallBehaviour()
    {
        LayerMask layerInteractable;
        Collider[] interactableColliders;
        Rigidbody[] interactableRigidBodies;
        Interactable[] interactableScripts;
        float distance;
        float damage;

        while (hit == false && lifeSpan > 0)
        {
            cannonBallTransform.position += cannonBallTransform.forward * speed * Time.deltaTime;
            lifeSpan -= Time.deltaTime;

            yield return null;
        }

        layerInteractable = LayerMask.GetMask("Interactable");
        interactableColliders = Physics.OverlapSphere(hitPosition, radius, layerInteractable);

        interactableRigidBodies = new Rigidbody[interactableColliders.Length];
        interactableScripts = new Interactable[interactableColliders.Length];

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            interactableRigidBodies[i] = interactableColliders[i].GetComponent<Rigidbody>();
            interactableScripts[i] = interactableColliders[i].GetComponent<Interactable>();
        }

        for (int i = 0; i < interactableColliders.Length; i++)
        {
            interactableRigidBodies[i].AddExplosionForce(force, hitPosition, radius, 1, ForceMode.Impulse);

            distance = Vector3.Distance(interactableColliders[i].transform.position, hitPosition);
            damage = Mathf.Lerp(damageMax, damageMin, distance / radius);

            interactableScripts[i].TakeDamage(damage);
        }

        Destroy(cannonBall);

        yield break;
    }
}
