using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataHitScan", menuName = "Weapon/Weapon Data HitScan")]
public class WeaponDataHitScan : WeaponData
{
    [Header("HitScan Weapon Attributes")]
    [Tooltip("The ammount of projectiles simultaneously fired by this weapon.")] public int ammount;
    [Tooltip("The maximum accuracy of this weapon.")] public float accuracyMax;
    [Tooltip("The minimum accuracy of this weapon.")] public float accuracyMin;
}
