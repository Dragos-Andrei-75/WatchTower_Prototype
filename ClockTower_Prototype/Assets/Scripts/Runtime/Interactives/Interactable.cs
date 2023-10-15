using UnityEngine;
using System;

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
    [SerializeField] private ManagerHealth healthManager;

    [Header("Interactable Object Atrributes")]
    [SerializeField] private Material materialOpaque;
    [SerializeField] private Material materialTransparent;
    [SerializeField] private Vector3 velocityBeforeCollision;
    [SerializeField] private float velocityTresholdLower = 5.0f;
    [SerializeField] private float velocityTresholdUpper = 25.0f;
    [SerializeField] private float collisionDamageMax = 5.0f;
    [SerializeField] private bool contact = false;
    [SerializeField] private bool contactKinematic = false;
    [SerializeField] private bool held = false;

    [Header("Collisions Information")]
    [SerializeField] private CollisionData[] collisions;
    [SerializeField] private CollisionData collisionsSwitch;
    [SerializeField] private int collisionsSize = 0;

    private bool kinematicsAdded = false;
    private bool kinematicsRemoved = false;

    public delegate void InteractableDestroy(Collider collider);
    public event InteractableDestroy OnInteractableDestroy;

    public bool Contact
    {
        get { return contact; }
    }

    public bool Held
    {
        get
        {
            return held;
        }
        set
        {
            held = value;

            if (held == true) interactableRenderer.material = materialTransparent;
            else interactableRenderer.material = materialOpaque;
        }
    }

    private void Awake()
    {
        interactableTransform = gameObject.transform;
        interactableCollider = interactableTransform.GetComponent<Collider>();
        interactableRigidbody = interactableTransform.GetComponent<Rigidbody>();
        interactableRenderer = interactableTransform.GetComponent<Renderer>();
        healthManager = interactableTransform.GetComponent<ManagerHealth>();

        materialOpaque = Resources.Load<Material>("Materials/Interactables/InteractableOpaqueMat");
        materialTransparent = Resources.Load<Material>("Materials/Interactables/InteractableTransparentMat");
    }

    private void FixedUpdate() => velocityBeforeCollision = interactableRigidbody.velocity;

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionsSize == 0) contact = true;

        if (collision.transform.gameObject.layer != LayerMask.NameToLayer("Player") && collision.transform.gameObject.layer != LayerMask.NameToLayer("Projectile"))
        {
            CollisionAdd(collision.gameObject.name, collision.GetContact(0).normal, collision.collider);

            if (collision.transform.GetComponent<Interactable>() != null) collision.transform.GetComponent<Interactable>().OnInteractableDestroy += CollisionRemove;

            if (collision.rigidbody == null || collision.rigidbody.isKinematic == true || collision.transform.GetComponent<Interactable>().contactKinematic == true) Shatter();

            CollisionDamage(collision.GetContact(0).normal);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        CollisionRemove(collision.collider);

        if (collision.transform.GetComponent<Interactable>() != null) collision.transform.GetComponent<Interactable>().OnInteractableDestroy -= CollisionRemove;

        if (collisionsSize == 0) contact = false;
    }

    private void OnDestroy()
    {
        if (OnInteractableDestroy != null) OnInteractableDestroy(interactableCollider);
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

    private void CollisionDamage(Vector3 collisionNormal)
    {
        if (velocityBeforeCollision.magnitude > velocityTresholdLower)
        {
            Vector3 direction = -velocityBeforeCollision.normalized;
            float magnitude = velocityBeforeCollision.magnitude;
            float damage = Mathf.Lerp(0, collisionDamageMax, magnitude / velocityTresholdUpper);
            float cosine = Mathf.Cos(Vector3.Angle(direction, collisionNormal) * Mathf.Deg2Rad);

            healthManager.TakeDamage(damage * cosine);
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
                        if (Mathf.Round(Vector3.Dot(collisions[i].normal, collisions[j].normal) * 10) / 10 < 0) healthManager.TakeDamage(healthManager.Health);
                    }
                }
            }
        }
    }
}
