using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data Projectile", fileName = "WeaponDataProjectile")]
public class WeaponDataProjectile : ScriptableObject
{
    [Header("Projectile Attributes")]
    [Tooltip("The projectiles fired from this weapon")]                                                  private GameObject[] projectiles;
    [Tooltip("The prefab of the projectile fired from this weapon."), SerializeField]                    private GameObject projectileObject;
    [Tooltip("The position where a projectile fired from this weapon is instantiated."), SerializeField] private Transform projectilePosition;
    [Tooltip("The speed with which a projectile fired from this weapon moves."), SerializeField]         private float speed;
    [Tooltip("The life span of a projectile fired from this weapon."), SerializeField]                   private float lifeSpan;

    public GameObject[] Projectiles
    {
        get { return projectiles; }
        set { projectiles = value; }
    }

    public GameObject ProjectileObject
    {
        get { return projectileObject; }
    }

    public Transform ProjectilePosition
    {
        get { return projectilePosition; }
        set { projectilePosition = value; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public float LifeSpan
    {
        get { return lifeSpan; }
    }
}
