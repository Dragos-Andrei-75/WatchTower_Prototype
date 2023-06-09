using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data Projectile", fileName = "WeaponDataProjectile")]
public class WeaponDataProjectile : ScriptableObject
{
    [Header("Projectile Attributes")]
    [Tooltip("The projectile fired from this weapon.")]                                  public GameObject[] projectileObject;
    [Tooltip("The position where a projectile fired from this weapon is instantiated.")] public Transform projectilePosition;
    [Tooltip("The spread of the projectiles fired from this weapon.")]                   public float[] spreadMax;
    [Tooltip("The spread of the projectiles fired from this weapon.")]                   public float[] spreadMin;
    [Tooltip("The speed with which a projectile fired from this weapon moves.")]         public float[] speed;
    [Tooltip("The life span of a projectile fired from this weapon.")]                   public float[] lifeSpan;
}
