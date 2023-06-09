using UnityEngine;
using System.Collections;

public struct DataProjectile
{
    public GameObject projectileObject;
    public Vector3 direction;
    public float damageMax;
    public float damageMin;
    public float forceMax;
    public float forceMin;
    public float speed;
    public float lifeSpan;

    public DataProjectile(WeaponData weaponData, WeaponDataProjectile weaponDataProjectile, int index)
    {
        projectileObject = weaponDataProjectile.projectileObject[index];
        damageMax = weaponData.damageMax[index];
        damageMin = weaponData.damageMin[index];
        forceMax = weaponData.forceMax[index];
        forceMin = weaponData.forceMin[index];
        speed = weaponDataProjectile.speed[index];
        lifeSpan = weaponDataProjectile.lifeSpan[index];
        direction = weaponData.direction;
    }
}

public class Projectile : MonoBehaviour
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

    private Coroutine coroutineOnContact;

    protected delegate IEnumerator Contact();
    protected event Contact OnContact;

    protected Rigidbody HitRigidbody
    {
        get { return hitRigidbody; }
        set { hitRigidbody = value; }
    }

    protected Coroutine CoroutineActive
    {
        get { return coroutineOnContact; }
        set { coroutineOnContact = value; }
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

    protected bool Hit
    {
        get { return hit; }
    }

    public DataProjectile ProjectileData
    {
        get { return projectileData; }
        set { projectileData = value; }
    }

    protected virtual void Start()
    {
        projectileObject = gameObject;
        projectileTransform = projectileObject.transform;

        projectileLayer = LayerMask.GetMask("Projectile");
        layerInteractable = LayerMask.GetMask("Interactable");

        StartCoroutine(ProjectileBehaviour());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        hitPosition = projectileTransform.position;
        hit = true;
    }

    private IEnumerator ProjectileBehaviour()
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

            if (Physics.Raycast(positionPrevious, direction, out RaycastHit hit, distancePosition, ~projectileLayer, QueryTriggerInteraction.Ignore) == true)
            {
                hitPosition = hit.point;
                hitRigidbody = hit.rigidbody;

                break;
            }

            lifeSpan -= Time.deltaTime;

            yield return null;
        }

        if (OnContact != null) coroutineOnContact = StartCoroutine(OnContact());

        while (coroutineOnContact != null) yield return null;

        Destroy(projectileObject);

        yield break;
    }
}
