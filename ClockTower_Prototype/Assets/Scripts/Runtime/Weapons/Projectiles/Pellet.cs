using UnityEngine;
using System.Collections;

public class Pellet : Projectile
{
    private void OnEnable() => OnContact += PelletBehaviour;

    private void OnDisable() => OnContact -= PelletBehaviour;

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        HitRigidbody = collision.rigidbody;
    }

    private IEnumerator PelletBehaviour()
    {
        float hitForce = Mathf.Lerp(ProjectileData.forceMin, ProjectileData.forceMax, LifeSpan / ProjectileData.lifeSpan);

        if (HitRigidbody != null && HitRigidbody.gameObject.layer == LayerMask.NameToLayer("Interactable")) HitRigidbody.AddForce(Direction * hitForce, ForceMode.Impulse);

        yield break;
    }
}
