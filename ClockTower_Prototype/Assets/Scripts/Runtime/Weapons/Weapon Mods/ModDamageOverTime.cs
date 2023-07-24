using UnityEngine;
using System.Collections;

public class ModDamageOverTime : WeaponMod
{
    [Header("Damage Over Time Attributes")]
    [SerializeField] private float damage = 0.1f;
    [SerializeField] private float time = 1.0f;
    [SerializeField] private float tick = 0.1f;

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
