using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Weapon/Weapon Data", fileName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    [Tooltip("The name of this weapon."), SerializeField]                                                                                         private string weaponName;

    [Header("Weapon Attributes")]
    [Tooltip("The direction of a projectile fired from this weapon."), SerializeField]                                                            private Vector3[] directions;
    [Tooltip("The maximum amount of damage dealt by a projectile fired from this weapon."), SerializeField]                                       private float damageMax;
    [Tooltip("The minimum amount of damage dealt by a projectile fired from this weapon."), SerializeField]                                       private float damageMin;
    [Tooltip("The maximum force with which a projectile fired from this weapon will push an object it comes into contact with."), SerializeField] private float forceMax;
    [Tooltip("The minimum force with which a projectile fired from this weapon will push an object it comes into contact with."), SerializeField] private float forceMin;
    [Tooltip("The time interval between projectiles fired from this weapon."), SerializeField]                                                    private float fireRate;
    [Tooltip("The time after which projectiles can be fired from this weapon."), SerializeField]                                                  private float fireNext;
    [Tooltip("The spread of the projectiles fired from this weapon."), SerializeField]                                                            private float spread;
    [Tooltip("The amount of time during which projectiles fired from this weapon loose accuracy."), SerializeField]                               private float heatMax;
    [Tooltip("The amount of time during which projectiles have been fired from this weapon."), SerializeField]                                    private float heat;
    [Tooltip("The maximum amount of ammunition this weapon can hold."), SerializeField]                                                           private int ammunitionCapacity;
    [Tooltip("The amount of ammunition this weapon has."), SerializeField]                                                                        private int ammunition;
    [Tooltip("The amount of projectiles fired simultaneously by this weapon."), SerializeField]                                                   private int amount;

    public delegate void WeaponShoot();
    public WeaponShoot OnWeaponShoot;

    public delegate IEnumerator WeaponHit(ManagerHealth managerHealth);
    public WeaponHit OnWeaponHit;

    public delegate void AmountSet();
    public event AmountSet OnAmountSet;

    public string WeaponName
    {
        get { return weaponName; }
    }

    public Vector3[] Directions
    {
        get { return directions; }
        set { directions = value; }
    }

    public float DamageMax
    {
        get { return damageMax; }
    }

    public float DamageMin
    {
        get { return damageMin; }
    }

    public float ForceMax
    {
        get { return forceMax; }
    }

    public float ForceMin
    {
        get { return forceMin; }
    }

    public float Spread
    {
        get { return spread; }
        set { spread = value; }
    }

    public float FireRate
    {
        get { return fireRate; }
        set { fireRate = value; }
    }

    public float FireNext
    {
        get { return fireNext; }
        set { fireNext = value; }
    }

    public float HeatMax
    {
        get { return heatMax; }
    }

    public float Heat
    {
        get { return heat; }
        set { heat = value; }
    }

    public int AmmunitionCapacity
    {
        get { return ammunitionCapacity; }
    }

    public int Ammunition
    {
        get { return ammunition; }
        set { ammunition = value; }
    }

    public int Amount
    {
        get
        {
            return amount;
        }
        set
        {
            if (value != amount)
            {
                amount = value;

                directions = new Vector3[amount];

                if (OnAmountSet != null) OnAmountSet();
            }
        }
    }
}
