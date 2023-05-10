using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataProjectile", menuName = "Weapon/Weapon Data Projectile")]
public class WeaponDataProjectile : WeaponData
{
    [Header("Projectile Attributes")]
    [Tooltip("The projectile fired from this weapon.")]                                                    public GameObject[] projectileObject;
    [Tooltip("The position where a projectile fired from this weapon is instantiated.")]                   public Transform projectilePosition;
    [Tooltip("The speed with which a projectile fired from this weapon moves.")]                           public float[] speed;
    [Tooltip("The spread of the projectiles fired from this weapon.")]                                     public float[] spread;
    [Tooltip("The life span of a projectile fired from this weapon.")]                                     public float[] lifeSpan;
}
