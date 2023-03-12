using UnityEngine;

public abstract class Door : Interactive
{
    [Header("Door Object and Component Referenes")]
    [SerializeField] protected Transform doorTransform;

    [Header("Door Attributes")]
    [SerializeField] protected DoorController doorController;
    [SerializeField] protected DoorTypes doorType;
    [SerializeField] protected bool engaged = false;

    public enum DoorTypes { HingedDoor, SlidingDoor };

    public bool Engaged
    {
        get { return engaged; }
    }

    public DoorTypes DoorType
    {
        get { return doorType; }
    }

    private void Start() => Setup();

    public override void Setup()
    {
        base.Setup();

        doorTransform = gameObject.transform;

        if (doorTransform.parent != null)
        {
            if (doorTransform.GetComponentInParent<DoorController>() != null) doorController = doorTransform.GetComponentInParent<DoorController>();
            else doorController = null;

            if (doorTransform.GetComponentInParent<DoorSensor>() != null) automatic = true;
            else automatic = false;
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (doorController != null)
        {
            engaged = true;
            doorController.Interact();
            engaged = false;
        }
    }
}
