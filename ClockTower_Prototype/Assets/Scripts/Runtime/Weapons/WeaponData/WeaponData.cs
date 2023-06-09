using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    [Tooltip("The name of the weapon.")]                                                                                          public string weaponName;
    [Tooltip("The index this weapon occupies in the loadout.")]                                                                   public int index;

    [Header("Weapon Attributes")]                                                                    
    [Tooltip("The position of this weapon.")]                                                                                     public Vector3 position;
    [Tooltip("The direction of a projectile fired from this weapon.")]                                                            public Vector3 direction;
    [Tooltip("The maximum amount of damage dealt by a projectile fired from this weapon.")]                                       public float[] damageMax;
    [Tooltip("The minimum amount of damage dealt by a projectile fired from this weapon.")]                                       public float[] damageMin;
    [Tooltip("The maximum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMax;
    [Tooltip("The minimum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMin;
    [Tooltip("The longest time interval between projectiles fired from this weapon.")]                                            public float[] fireRateMax;
    [Tooltip("The shortest time interval between projectiles fired from this weapon.")]                                           public float[] fireRateMin;
    [Tooltip("The time interval between projectiles fired from this weapon.")]                                                    public float[] fireRate;
    [Tooltip("The time after which projectiles can be fired from this weapon.")]                                                  public float[] fireNext;
    [Tooltip("The amount of time during which projectiles fired from this weapon loose accuracy.")]                               public float[] heatMax;
    [Tooltip("The amount of time during which projectiles have been fired from this weapon.")]                                    public float[] heat;
    [Tooltip("The maximum amount of ammunition this weapon can hold.")]                                                           public int[] ammunitionCapacity;
    [Tooltip("The amount of ammunition this weapon has.")]                                                                        public int[] ammunition;
    [Tooltip("The ammount of projectiles fired simultaneously by this weapon.")]                                                  public int[] amount;
}
