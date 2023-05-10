using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    [Tooltip("The name of the weapon.")]                                                                                          public string weaponName;

    [Header("Weapon Attributes")]
    [Tooltip("The position of this weapon.")]                                                                                     public Vector3 position;
    [Tooltip("The index this weapon occupies in the loadout.")]                                                                   public int index;
    [Tooltip("The maximum ammount of damage dealt by a projectile fired from this weapon.")]                                      public float[] damageMax;
    [Tooltip("The minimum ammount of damage dealt by a projectile fired from this weapon.")]                                      public float[] damageMin;
    [Tooltip("The maximum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMax;
    [Tooltip("The minimum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMin;
    [Tooltip("The time interval between shots fired from this weapon.")]                                                          public float[] fireRate;
    [Tooltip("The time after which shots can be fired from this weapon.")]                                                        public float[] fireNext;
    [Tooltip("The maximum ammount of ammunition this weapon can hold.")]                                                          public int[] ammunitionCapacity;
    [Tooltip("The ammount of ammunition this weapon has.")]                                                                       public int[] ammunition;
    [Tooltip("The ammount of projectiles fired simultaneously by this weapon.")]                                                  public int[] ammount;
}
