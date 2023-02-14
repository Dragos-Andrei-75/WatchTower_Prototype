using UnityEngine;
using System;

public class Interactable : MonoBehaviour
{
    [Header("Hit Objects Reference")]
    [SerializeField] private Transform[] objectsHit;
    [SerializeField] private Transform objectHitSwitch;
    [SerializeField] private int objectsHitSize = 0;

    [Header("Interactable Object Atrributes")]
    [SerializeField] private float health = 10.0f;
    [SerializeField] private float collisionDamageMax = 5.0f;
    [SerializeField] private float impulseTresholdLower = 10.0f;
    [SerializeField] private float impulseTresholdUpper = 25.0f;
    [SerializeField] private bool contact = false;
    [SerializeField] private bool carried = false;

    public delegate void DestroyInteractable(Collider collider);
    public event DestroyInteractable OnDestroyInteractable;

    public bool Contact
    {
        get { return contact; }
    }

    public bool Carried
    {
        get { return carried; }
        set { carried = value; }
    }

    private void Start() => objectsHit = new Transform[objectsHitSize];

    private void OnCollisionEnter(Collision collision)
    {
        contact = true;

        AddObjectHit(collision.transform);

        if (collision.gameObject.tag.CompareTo("Player") != 0 && carried == false) CollisionDamage(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        RemoveObjectHit(collision.transform);

        if (objectsHit.Length == 0) contact = false;
    }

    private void AddObjectHit(Transform objectHitTransform)
    {
        objectsHitSize++;
        Array.Resize(ref objectsHit, objectsHitSize);

        objectsHit[objectsHitSize - 1] = objectHitTransform;
    }

    private void RemoveObjectHit(Transform objectHitTransform)
    {
        int k = 0;

        for (int i = 0; i < objectsHit.Length; i++)
        {
            if (objectsHit[i] == objectHitTransform || objectsHit[i] == null)
            {
                k++;

                if (objectsHit[i] != objectsHit[objectsHit.Length - k])
                {
                    objectHitSwitch = objectsHit[i];
                    objectsHit[i] = objectsHit[objectsHit.Length - k];
                    objectsHit[objectsHit.Length - k] = objectHitSwitch;
                }
            }
        }

        objectsHitSize = objectsHit.Length - k;
        Array.Resize(ref objectsHit, objectsHitSize);
    }

    private void CollisionDamage(Collision collisionInfo)
    {
        if (collisionInfo.impulse.magnitude > impulseTresholdLower)
        {
            float impulse = collisionInfo.impulse.magnitude;
            float collisionDamage = Mathf.Lerp(0, collisionDamageMax, impulse / impulseTresholdUpper);

            TakeDamage(collisionDamage);
        }
    }

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
