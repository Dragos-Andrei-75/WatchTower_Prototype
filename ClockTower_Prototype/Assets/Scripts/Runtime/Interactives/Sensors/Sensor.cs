using UnityEngine;
using System;

public abstract class Sensor : MonoBehaviour
{
    [Header("Sensor Object and Component References")]
    [SerializeField] protected Transform sensorTransform;
    [SerializeField] protected BoxCollider sensorTriggerCollider;

    [Header("Other Object and Component References")]
    [SerializeField] protected Collider[] colliders;

    [Header("Sensor Attributes")]
    [SerializeField] protected float sensorArea = 10.0f;

    public float SensorArea
    {
        get { return sensorArea; }
        set { if (value > 0) sensorArea = value; }
    }

    protected void Awake() => SetupSensor();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Interactable>() != null) other.GetComponent<Interactable>().OnInteractableDestroy += ColliderRemove;
        ColliderAdd(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactable>() != null) other.GetComponent<Interactable>().OnInteractableDestroy -= ColliderRemove;
        ColliderRemove(other);
    }

    public virtual void SetupSensor()
    {
        sensorTransform = gameObject.transform;
        sensorTriggerCollider = ColliderFind(true);
    }

    private void ColliderAdd(Collider collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Surface") && collider.gameObject.layer != LayerMask.NameToLayer("Water"))
        {
            Array.Resize(ref colliders, colliders.Length + 1);
            colliders[colliders.Length - 1] = collider;
        }
    }

    private void ColliderRemove(Collider collider)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == collider)
            {
                for (int j = i; j < colliders.Length - 1; j++)
                {
                    colliders[j] = colliders[j + 1];
                }
            }
        }

        Array.Resize(ref colliders, colliders.Length - 1);
    }

    protected BoxCollider ColliderFind(bool findTrigger)
    {
        BoxCollider[] colliders = sensorTransform.GetComponents<BoxCollider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isTrigger == findTrigger)
            {
                return colliders[i];
            }
        }

        return null;
    }
}
