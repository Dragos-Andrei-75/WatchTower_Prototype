using UnityEngine;

public abstract class Door : Interactive
{
    [Header("Door Object and Component Referenes")]
    [SerializeField] protected Transform doorTransform;
    [SerializeField] protected Rigidbody doorRigidBody;

    [Header("Door Attributes")]
    [SerializeField] protected DoorTypes doorType;
    [SerializeField] protected float timeToMove = 1.0f;
    [SerializeField] protected float timePassed = 0.0f;
    [SerializeField] protected bool pair = false;
    [SerializeField] protected bool automatic = false;

    protected Coroutine coroutineActive;

    public enum DoorTypes : ushort { HingedDoor, SlidingDoor };

    public DoorTypes DoorType
    {
        get { return doorType; }
    }

    public float TimeToMove
    {
        get { return timeToMove; }
        set { timeToMove = value; }
    }

    public bool Pair
    {
        get { return pair; }
    }

    public bool Automatic
    {
        get { return automatic; }
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

    public override void Interact()
    {
        if (coroutineActive == null)
        {
            timePassed = 0;
        }
        else
        {
            StopCoroutine(coroutineActive);
            timePassed = timeToMove - timePassed;
        }
    }
}
