using UnityEngine;
using System.Collections;

public class ModDamageOverTime : WeaponMod
{
    [Header("Damage Over Time Attributes")]
    [SerializeField] private float damage;
    [SerializeField] private float time;
    [SerializeField] private float tick;

    private void OnEnable() => WeaponData.OnWeaponHit += DamageOverTime;

    private void OnDisable() => WeaponData.OnWeaponHit -= DamageOverTime;

    private IEnumerator DamageOverTime(ManagerHealth managerHealth)
    {
        float timePassed = 0;

        while (timePassed < time && managerHealth != null)
        {
            managerHealth.Health -= damage;
            timePassed += tick;

            yield return new WaitForSeconds(tick);
        }

        yield break;
    }
}
