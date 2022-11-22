using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactive Object Atrributes")]
    [SerializeField] private float health = 10.0f;

    public delegate void DestroyInteractable(Collider collider);
    public event DestroyInteractable OnDestroyInteractable;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0.0f) Brake();
    }

    public void Brake()
    {
        Destroy(gameObject);

        if (OnDestroyInteractable != null) OnDestroyInteractable(gameObject.GetComponent<Collider>());
    }
}
