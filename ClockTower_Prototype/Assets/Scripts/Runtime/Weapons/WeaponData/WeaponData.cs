using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    [Tooltip("The name of the weapon.")]                                                                                          public string weaponName;

    [Header("Weapon Attributes")]
    [Tooltip("The position of this weapon.")]                                                                                     public Vector3 position;
    [Tooltip("The index this weapon occupies in the loadout.")]                                                                   public int index;
    [Tooltip("The longest time interval between shots fired from this weapon.")]                                                  public float[] fireRateLimitUpper;
    [Tooltip("The shortest time interval between shots fired from this weapon.")]                                                 public float[] fireRateLimitLower;
    [Tooltip("The time interval between shots fired from this weapon.")]                                                          public float[] fireRate;
    [Tooltip("The time after which shots can be fired from this weapon.")]                                                        public float[] fireNext;
    [Tooltip("The ammount of time during which projectiles fired from this weapon loose accuracy.")]                              public float[] heatMax;
    [Tooltip("The ammount of time during which projectiles have been fired from this weapon.")]                                   public float[] heat;
    [Tooltip("The maximum ammount of ammunition this weapon can hold.")]                                                          public int[] ammunitionCapacity;
    [Tooltip("The ammount of ammunition this weapon has.")]                                                                       public int[] ammunition;
}
