using UnityEngine;

public abstract class Door : Interactive
{
    [Header("Door Object and Component Referenes")]
    [SerializeField] protected Transform doorTransform;
    [SerializeField] protected Rigidbody doorRigidBody;

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
        doorRigidBody = gameObject.GetComponent<Rigidbody>();

        if (doorTransform.parent != null)
        {
            if (doorTransform.GetComponentInParent<DoorController>() != null) pair = true;
            else if (doorTransform.GetComponentInParent<DoorSensor>() != null) automatic = true;
        }
    }

    public override void Interact() => base.Interact();

    protected void DoorKinematic()
    {
        if (coroutineActive != null && doorRigidBody.isKinematic == false) doorRigidBody.isKinematic = true;
    }

    protected void DoorNonKinematic()
    {
        if (coroutineActive != null && doorRigidBody.isKinematic == true) doorRigidBody.isKinematic = false;
    }
}
