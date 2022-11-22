using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour
{
    [Header("CannonBall Object and Components References")]
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private Transform cannonBallTransform;
    [SerializeField] private Rigidbody cannonBallRigidbody;

    [Header("CannonBall Attributes")]
    [SerializeField] private Vector3 hitPosition;
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float force = 25.0f;
    [SerializeField] private float damageMax = 10.0f;
    [SerializeField] private float damageMin = 1.0f;
    [SerializeField] private bool hit = false;

    private void Start()
    {
        cannonBall = gameObject;
        cannonBallTransform = cannonBall.transform;
        cannonBallRigidbody = cannonBall.GetComponent<Rigidbody>();

        StartCoroutine(CannonBallBehaviour());
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPosition = cannonBallTransform.position;
        hit = true;
    }

    private IEnumerator CannonBallBehaviour()
    {
        while (hit == false)
        {
            cannonBallRigidbody.AddForce(cannonBallTransform.forward * speed, ForceMode.Impulse);
            yield return null;
        }

        LayerMask layerInteractable = LayerMask.GetMask("Interactable");
        Collider[] collidersInteractable = Physics.OverlapSphere(hitPosition, radius, layerInteractable);

        for (int i = 0; i < collidersInteractable.Length; i++)
        {
            Rigidbody interactableRigidBody = collidersInteractable[i].GetComponent<Rigidbody>();
            Interactable interactableScript = collidersInteractable[i].GetComponent<Interactable>();

            interactableRigidBody.AddExplosionForce(force, hitPosition, radius, 1.0f, ForceMode.Impulse);

            float distance = Vector3.Distance(collidersInteractable[i].transform.position, hitPosition);
            float damage = Mathf.Lerp(damageMax, damageMin, distance / radius);

            interactableScript.TakeDamage(damage);
        }

        Destroy(cannonBall);

        yield break;
    }
}
