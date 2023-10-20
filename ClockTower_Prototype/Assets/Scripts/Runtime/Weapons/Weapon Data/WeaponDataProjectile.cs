using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data Projectile", fileName = "WeaponDataProjectile")]
public class WeaponDataProjectile : ScriptableObject
{
    [Header("Projectile Data")]
    [Tooltip("The prefab of the projectile fired from this weapon."), SerializeField]                    private GameObject projectileObject;
    [Tooltip("The transform of the prjectile fired from this weapon."), SerializeField]                  private Transform projectileTransform;
    [Tooltip("The life span of a projectile fired from this weapon."), SerializeField]                   private float lifeSpan;
    [Tooltip("The speed with which a projectile fired from this weapon moves."), SerializeField]         private float speed;

    public GameObject ProjectileObject
    {
        get { return projectileObject; }
    }

    public Transform ProjectilePosition
    {
        get { return projectileTransform; }
        set { projectileTransform = value; }
    }

    public float LifeSpan
    {
        get { return lifeSpan; }
    }

    public float Speed
    {
        get { return speed; }
    }
}
