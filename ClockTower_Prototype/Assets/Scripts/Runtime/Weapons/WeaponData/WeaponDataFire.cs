using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Fire", fileName = "WeaponFire")]
public class WeaponDataFire : ScriptableObject
{
    [Header("Weapon Fire Attributes")]
    [Tooltip("The maximum ammount of damage dealt by a projectile fired from this weapon.")]                                      public float[] damageMax;
    [Tooltip("The minimum ammount of damage dealt by a projectile fired from this weapon.")]                                      public float[] damageMin;
    [Tooltip("The maximum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMax;
    [Tooltip("The minimum force with which a projectile fired from this weapon will push an object it comes into contact with.")] public float[] forceMin;
    [Tooltip("The ammount of projectiles fired simultaneously by this weapon.")]                                                  public int[] ammount;
}
