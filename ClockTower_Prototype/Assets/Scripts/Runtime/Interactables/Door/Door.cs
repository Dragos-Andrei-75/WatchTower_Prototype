using UnityEngine;

public abstract class Door : Interactive
{
    [Header("Door Object and Component Referenes")]
    [SerializeField] protected Transform doorTransform;

    [Header("Door Attributes")]
    [SerializeField] protected DoorTypes doorType;

    public enum DoorTypes : ushort { HingedDoor, SlidingDoor };

    public DoorTypes DoorType
    {
        get { return doorType; }
    }

    public bool Pair
    {
        get { return pair; }
    }

    private void Start() => DoorSetUp();

    public virtual void DoorSetUp()
    {
        doorTransform = gameObject.transform;

        if (doorTransform.parent != null)
        {
            if (doorTransform.GetComponentInParent<DoorController>() != null) pair = true;
            else if (doorTransform.GetComponentInParent<DoorSensor>() != null) automatic = true;
        }
    }

    public override void Interact() => base.Interact();
}
