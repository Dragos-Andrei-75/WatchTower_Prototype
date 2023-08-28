using UnityEngine;
using System.Collections;

using static WeaponData;

public struct DataProjectile
{
    public WeaponHit OnWeaponHit;
    public GameObject projectileObject;
    public Vector3 direction;
    public float damageMax;
    public float damageMin;
    public float forceMax;
    public float forceMin;
    public float speed;
    public float lifeSpan;

    public DataProjectile(WeaponHit onWeaponHit, GameObject projectileObject, Vector3 direction, float damageMax, float damageMin, float forceMax, float forceMin, float speed, float lifeSpan)
    {
        OnWeaponHit = onWeaponHit;

        this.projectileObject = projectileObject;
        this.direction = direction;
        this.damageMax = damageMax;
        this.damageMin = damageMin;
        this.forceMax = forceMax;
        this.forceMin = forceMin;
        this.speed = speed;
        this.lifeSpan = lifeSpan;
    }
}

public abstract class Projectile : MonoBehaviour
{
    [Header("Projectile Object and Components References")]
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform projectileTransform;

    [Header("Projectile Attributes")]
    [SerializeField] private DataProjectile projectileData;
    [SerializeField] private LayerMask projectileLayer;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float lifeSpan;
    [SerializeField] private bool hit;

    [Header("Collision Attributes")]
    [SerializeField] private Rigidbody hitRigidbody;
    [SerializeField] private LayerMask layerInteractable;
    [SerializeField] private Vector3 hitPosition;

    protected delegate void Contact();
    protected event Contact OnContact;

    public DataProjectile ProjectileData
    {
        get { return projectileData; }
        set { projectileData = value; }
    }

    protected Rigidbody HitRigidbody
    {
        get { return hitRigidbody; }
        set { hitRigidbody = value; }
    }

    protected LayerMask LayerInteractable
    {
        get { return layerInteractable; }
    }

    protected Vector3 HitPosition
    {
        get { return hitPosition; }
    }

    protected Vector3 Direction
    {
        get { return direction; }
    }

    protected float LifeSpan
    {
        get { return lifeSpan; }
    }

    protected virtual void Start()
    {
        projectileObject = gameObject;
        projectileTransform = projectileObject.transform;

        projectileLayer = LayerMask.GetMask("Projectile");
        layerInteractable = LayerMask.GetMask("Interactable");

        projectileTransform.rotation = Quaternion.LookRotation(projectileData.direction);

        StartCoroutine(ProjectileBehavior());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        hitPosition = projectileTransform.position;
        hitRigidbody = collision.rigidbody;
        hit = true;
    }

    private IEnumerator ProjectileBehavior()
    {
        Vector3 positionPrevious;
        float distancePosition;

        lifeSpan = projectileData.lifeSpan;

        while (hit == false && lifeSpan > 0)
        {
            positionPrevious = projectileTransform.position;

            projectileTransform.position += projectileData.direction * projectileData.speed * Time.deltaTime;

            direction = (projectileTransform.position - positionPrevious).normalized;
            distancePosition = Vector3.Distance(positionPrevious, projectileTransform.position);

            hit = Physics.Raycast(positionPrevious, direction, out RaycastHit rayCastHit, distancePosition, ~projectileLayer, QueryTriggerInteraction.Ignore);

            if (hit == true)
            {
                hitPosition = rayCastHit.point;
                hitRigidbody = rayCastHit.rigidbody;

                break;
            }

            lifeSpan -= Time.deltaTime;

            yield return null;
        }

        if (OnContact != null) OnContact();

        Destroy(projectileObject);

        yield break;
    }
}
