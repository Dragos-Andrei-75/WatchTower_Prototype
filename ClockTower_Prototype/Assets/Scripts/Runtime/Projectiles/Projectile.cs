using UnityEngine;
using System.Collections;

public struct DataProjectile
{
    public GameObject projectileObject;
    public Transform projectileTransform;
    public float damageMax;
    public float damageMin;
    public float forceMax;
    public float forceMin;
    public float speed;
    public float lifeSpan;

    public DataProjectile(GameObject projectileObject, Transform projectileTransform, float damageMax, float damageMin, float forceMax, float forceMin, float speed, float lifeSpan)
    {
        this.projectileObject = projectileObject;
        this.projectileTransform = projectileTransform;
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
    [Header("Projectile Attributes")]
    [SerializeField] private DataProjectile projectileData;
    [SerializeField] private LayerMask projectileLayer;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float lifeSpan;
    [SerializeField] private bool hit;

    [Header("Collision Attributes")]
    [SerializeField] private LayerMask layerInteractable;
    [SerializeField] private Vector3 hitPosition;

    protected delegate void Contact();
    protected event Contact OnContact;

    public DataProjectile ProjectileData
    {
        get { return projectileData; }
        set { projectileData = value; }
    }

    protected LayerMask LayerInteractable
    {
        get { return layerInteractable; }
    }

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    protected Vector3 HitPosition
    {
        get { return hitPosition; }
    }

    protected float LifeSpan
    {
        get { return lifeSpan; }
    }

    protected virtual void Start()
    {
        projectileLayer = LayerMask.GetMask("Projectile");
        layerInteractable = LayerMask.GetMask("Interactable");

        projectileData.projectileTransform.rotation = Quaternion.LookRotation(direction);

        StartCoroutine(ProjectileBehaviour());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        hitPosition = collision.GetContact(0).point;
        hit = true;
    }

    private IEnumerator ProjectileBehaviour()
    {
        Vector3 positionPrevious;
        float distance;

        lifeSpan = projectileData.lifeSpan;

        while (hit == false && lifeSpan > 0)
        {
            positionPrevious = projectileData.projectileTransform.position;

            projectileData.projectileTransform.position += direction * projectileData.speed * Time.fixedDeltaTime;

            distance = Vector3.Distance(projectileData.projectileTransform.position, positionPrevious);

            hit = Physics.Raycast(positionPrevious, direction, out RaycastHit rayCastHit, distance, ~projectileLayer, QueryTriggerInteraction.Ignore);

            if (hit == true)
            {
                hitPosition = rayCastHit.point;
                break;
            }

            lifeSpan -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (OnContact != null) OnContact();

        Destroy(projectileData.projectileObject);

        yield break;
    }
}
