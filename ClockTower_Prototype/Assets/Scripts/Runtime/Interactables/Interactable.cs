using UnityEngine;
using System;
using System.Collections;

public struct CollisionData
{
    public string name;
    public Vector3 normal;
    public Collider collider;
    public Interactable interactable;

    public CollisionData(string name, Vector3 normal, Collider collider, Interactable interactable)
    {
        this.name = name;
        this.collider = collider;
        this.normal = normal;
        this.interactable = interactable;
    }
}

public class Interactable : MonoBehaviour
{
    [Header("Interactable Object and Component References")]
    [SerializeField] private Transform interactableTransform;
    [SerializeField] private Collider interactableCollider;
    [SerializeField] private Rigidbody interactableRigidbody;
    [SerializeField] private Renderer interactableRenderer;

    [Header("Interactable Object Atrributes")]
    [SerializeField] private Material materialOpaque;
    [SerializeField] private Material materialTransparent;
    [SerializeField] private float health = 10.0f;
    [SerializeField] private float velocityTresholdLower = 10.0f;
    [SerializeField] private float velocityTresholdUpper = 25.0f;
    [SerializeField] private float collisionDamage = 5.0f;
    [SerializeField] private bool contact = false;
    [SerializeField] private bool contactKinematic = false;
    [SerializeField] private bool carried = false;

    [Header("Collisions Information")]
    [SerializeField] private CollisionData[] collisions;
    [SerializeField] private CollisionData collisionsSwitch;
    [SerializeField] private int collisionsSize = 0;

    private bool kinematicsAdded = false;
    private bool kinematicsRemoved = false;

    public delegate void InteractableDestroy(Collider collider);
    public event InteractableDestroy OnInteractableDestroy;

    public delegate void interactableCarriedDestroy();
    public event interactableCarriedDestroy OnInteractableCarriedDestroy;

    public delegate IEnumerator interactableGrappleDestroy();
    public event interactableGrappleDestroy OnInteractableGrappleDestroy;

    public bool Contact
    {
        get { return contact; }
    }

    public bool Carried
    {
        get { return carried; }
        set { carried = value; if (carried == true) interactableRenderer.material = materialTransparent; else interactableRenderer.material = materialOpaque; }
    }

    private void Start()
    {
        interactableTransform = gameObject.transform;
        interactableCollider = interactableTransform.GetComponent<Collider>();
        interactableRigidbody = interactableTransform.GetComponent<Rigidbody>();
        interactableRenderer = interactableTransform.GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        contact = true;

        CollisionAdd(collision.gameObject.name, collision.GetContact(0).normal, collision.collider);

        if (collision.transform.GetComponent<Interactable>() != null) collision.transform.GetComponent<Interactable>().OnInteractableDestroy += CollisionRemove;

        if (collision.rigidbody == null || collision.rigidbody.isKinematic == true || collision.transform.GetComponent<Interactable>().contactKinematic == true) Shatter();

        if (collision.transform.gameObject.layer != LayerMask.NameToLayer("Player") && carried == false) CollisionDamage();
    }

    private void OnCollisionExit(Collision collision)
    {
        CollisionRemove(collision.collider);

        if (collision.transform.GetComponent<Interactable>() != null) collision.transform.GetComponent<Interactable>().OnInteractableDestroy -= CollisionRemove;

        if (collisionsSize == 0) contact = false;
    }

    private void CollisionAdd(string hitName, Vector3 hitNormal, Collider hitCollider)
    {
        collisionsSize++;

        Array.Resize(ref collisions, collisionsSize);

        if (hitCollider.transform.GetComponent<Interactable>() == null) collisions[collisionsSize - 1] = new CollisionData(hitName, hitNormal, hitCollider, null);
        else collisions[collisionsSize - 1] = new CollisionData(hitName, hitNormal, hitCollider, hitCollider.transform.GetComponent<Interactable>());

        KinematicsAdd();
    }

    private void CollisionRemove(Collider collider)
    {
        int k = 0;

        KinematicsRemove(collider);

        for (int i = 0; i < collisionsSize - k; i++)
        {
            if (collisions[i].collider == collider || collisions[i].collider == null)
            {
                k++;

                if (collisions[i].collider != collisions[collisionsSize - k].collider)
                {
                    collisionsSwitch = collisions[i];
                    collisions[i] = collisions[collisionsSize - k];
                    collisions[collisionsSize - k] = collisionsSwitch;
                }

                i--;
            }
        }

        collisionsSize -= k;

        Array.Resize(ref collisions, collisionsSize);
    }

    private void KinematicsAdd()
    {
        bool kinematicFound = false;

        kinematicsAdded = false;

        for (int i = 0; i < collisionsSize; i++)
        {
            if (collisions[i].interactable == null)
            {
                CollisionData collisionDataKinematic = collisions[i];

                kinematicFound = true;

                for (int j = 0; j < collisionsSize; j++)
                {
                    if (collisions[j].interactable != null)
                    {
                        CollisionData collisionDataInteractable = collisions[j];
                        Interactable interactable = collisions[j].interactable;
                        bool found = false;

                        if (Mathf.Round(Vector3.Dot(collisionDataKinematic.normal, -collisionDataInteractable.normal) * 10) / 10 > 0)
                        {
                            for (int k = 0; k < interactable.collisionsSize; k++)
                            {
                                if (interactable.collisions[k].collider == collisionDataKinematic.collider)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found == false)
                            {
                                string collisionName = collisionDataKinematic.name + " < " + gameObject.name;
                                interactable.CollisionAdd(collisionName, collisionDataKinematic.normal, collisionDataKinematic.collider);
                            }

                            if (interactable.kinematicsAdded == true) interactable.KinematicsAdd();
                        }
                    }
                }
            }
        }

        contactKinematic = kinematicFound;

        kinematicsAdded = true;
    }

    private void KinematicsRemove(Collider collider)
    {
        kinematicsRemoved = false;

        for (int i = 0; i < collisionsSize; i++)
        {
            if (collisions[i].interactable == null) { if (collisions[i].name.Contains(" < " + collider.name) == true) collisions[i].collider = null; }
            else { if (collisions[i].interactable.kinematicsRemoved == true) collisions[i].interactable.CollisionRemove(collider); }
        }

        kinematicsRemoved = true;
    }

    private void CollisionDamage()
    {
        if (interactableRigidbody.velocity.magnitude > velocityTresholdLower)
        {
            float velocity = interactableRigidbody.velocity.magnitude;
            float damage = Mathf.Lerp(0, collisionDamage, velocity / velocityTresholdUpper);

            TakeDamage(damage);
        }
    }

    private void Shatter()
    {
        for (int i = 0; i < collisionsSize; i++)
        {
            if (collisions[i].interactable == null)
            {
                for (int j = i; j < collisionsSize; j++)
                {
                    if (collisions[j].interactable == null)
                    {
                        if (Mathf.Round(Vector3.Dot(collisions[i].normal, collisions[j].normal) * 10) / 10 < 0) TakeDamage(health);
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Brake();
    }

    public void Brake()
    {
        Destroy(gameObject);

        if (OnInteractableDestroy != null) OnInteractableDestroy(interactableCollider);
        if (OnInteractableCarriedDestroy != null) OnInteractableCarriedDestroy();
        if (OnInteractableGrappleDestroy != null) OnInteractableGrappleDestroy();
    }
}
