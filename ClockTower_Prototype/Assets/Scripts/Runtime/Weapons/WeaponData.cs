using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    [Tooltip("The name of the weapon.")]                                                                                     public string weaponName;

    [Header("Weapon Attributes")]
    [Tooltip("The projectile fired from this weapon.")]                                                                      public GameObject projectile;
    [Tooltip("The ammount of damage dealt by a projectile fired from this weapon.")]                                         public float damage;
    [Tooltip("The force with which a projectile fired from this weapon will push an object when it makes contact with it.")] public float forceImpact;
    [Tooltip("The travel distance of a projectile fired from this weapon.")]                                                 public float range;
    [Tooltip("The time interval between shots fired from this weapon.")]                                                     public float fireRate;
    [Tooltip("The time after which shots can be fired from this weapon.")]                                                   public float fireNext;
    [Tooltip("The maximum ammount of ammunition this weapon can have.")]                                                     public int ammunitionCapacity;
    [Tooltip("The ammount of ammunition this weapon has.")]                                                                  public int ammunition;
}
