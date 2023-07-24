using UnityEngine;

public class ModAdjust : WeaponMod
{
    [Header("Adjust Attributes")]
    [SerializeField] private WeaponAttributes[] weaponAttributes;
    [SerializeField] private float[] valuesMax;
    [SerializeField] private float[] valuesMin;

    private enum WeaponAttributes : ushort {FireRate, Spread, Amount };

    private void OnEnable()
    {
        WeaponData.OnWeaponShoot += Adjust;
        WeaponData.OnWeaponShoot += Adjust;
    }

    private void OnDisable()
    {
        WeaponData.OnWeaponShoot -= Adjust;
        WeaponData.OnWeaponShoot -= Adjust;
    }

    private void Adjust()
    {
        for (int i = 0; i < weaponAttributes.Length; i++)
        {
            if (weaponAttributes[i] == WeaponAttributes.Spread) WeaponData.Spread = Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
            else if (weaponAttributes[i] == WeaponAttributes.FireRate) WeaponData.FireRate = Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
            else if (weaponAttributes[i] == WeaponAttributes.Amount) WeaponData.Amount = (int)Mathf.Lerp(valuesMin[i], valuesMax[i], WeaponData.Heat / WeaponData.HeatMax);
        }
    }
}
