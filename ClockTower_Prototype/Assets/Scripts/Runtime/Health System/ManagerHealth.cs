using UnityEngine;

public class ManagerHealth : MonoBehaviour
{
    [Header("Health Attributes")]
    [SerializeField] private float health = 10.0f;

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0) Brake();
    }

    public void Brake() => Destroy(gameObject);
}
