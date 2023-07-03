using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data HitScan", fileName = "WeaponDataHitScan")]
public class WeaponDataHitScan : ScriptableObject
{
    [Header("HitScan Attributes")]
    [Tooltip("The travel distance of a projectile fired from this weapon."), SerializeField] private float range;

    public float Range
    {
        get { return range; }
    }
}
