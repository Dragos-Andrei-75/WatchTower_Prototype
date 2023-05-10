using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataHitScan", menuName = "Weapon/Weapon Data HitScan")]
public class WeaponDataHitScan : WeaponData
{
    [Header("HitScan Attributes")]
    [Tooltip("The maximum accuracy of this weapon.")]                        public float[] accuracyMax;
    [Tooltip("The minimum accuracy of this weapon.")]                        public float[] accuracyMin;
    [Tooltip("The travel distance of a projectile fired from this weapon.")] public float[] range;
}
