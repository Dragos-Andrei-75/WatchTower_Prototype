using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Object and Components References")]
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform projectileTransform;

    [Header("Projectile Attributes")]
    [SerializeField] private WeaponDataProjectile dataProjectile;
    [SerializeField] private Vector3 hitPosition;
    [SerializeField] private bool hit;

    private void Start()
    {
        projectileObject = gameObject;
        projectileTransform = projectileObject.transform;
    }

    protected GameObject ProjectileObject
    {
        get { return projectileObject; }
    }

    protected Transform ProjectileTransform
    {
        get { return projectileTransform; }
    }

    protected WeaponDataProjectile DataProjectile
    {
        get { return dataProjectile; }
    }

    protected Vector3 HitPosition
    {
        get { return hitPosition; }
        set { hitPosition = value; }
    }

    protected bool Hit
    {
        get { return hit; }
        set { hit = value; }
    }
}
