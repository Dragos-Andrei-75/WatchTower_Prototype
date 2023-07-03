using UnityEngine;

public class WeaponModAdjust : WeaponMod
{
    [Header("Adjust Attributes")]
    [SerializeField] private WeaponAttributes[] weaponAttributes;
    [SerializeField] private float[] valuesMax;
    [SerializeField] private float[] valuesMin;
    [SerializeField, Range(1, 3)] private int weaponAttributesSize;

    private enum WeaponAttributes : ushort { Spread, FireRate, Amount };

    protected override void Awake()
    {
        base.Awake();

        weaponAttributes = new WeaponAttributes[weaponAttributesSize];

        for (int i = 0; i < weaponAttributesSize; i++) weaponAttributes[i] = (WeaponAttributes)i;
    }

    protected override void Mod()
    {
        for (int i = 0; i < weaponAttributes.Length; i++)
        {
            if (weaponAttributes[i] == WeaponAttributes.Spread) WeaponData.Spread = Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
            else if (weaponAttributes[i] == WeaponAttributes.FireRate) WeaponData.FireRate = Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
            else if (weaponAttributes[i] == WeaponAttributes.Amount) WeaponData.Amount = (int)Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
        }
    }
}
