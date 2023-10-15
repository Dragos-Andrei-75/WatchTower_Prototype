using UnityEngine;
using System.Collections;

public abstract class Mobile : Interactive
{
    [Header("Mobile Object and Component References")]
    [SerializeField] private Transform interactiveTransform;
    [SerializeField] private Rigidbody interactiveRigidbody;
    [SerializeField] private OcclusionPortal interactivePortal;

    [Header("Mobile Attributes")]
    [SerializeField] private MotionTypes motionType;
    [SerializeField] private float timeToMove = 1.0f;
    [SerializeField] private float timePassed = 0.0f;
    [SerializeField] private bool interruptible = false;
    [SerializeField] private bool linked = false;
    [SerializeField] private bool controlled = false;
    [SerializeField] private bool automatic = false;

    protected Coroutine coroutineActive;

    public enum MotionTypes : ushort { Linear, Rotary };

    protected Transform InteractiveTransform
    {
        get { return interactiveTransform; }
    }

    protected Rigidbody InteractiveRigidbody
    {
        get { return interactiveRigidbody; }
    }

    protected OcclusionPortal InteractivePortal
    {
        get { return interactivePortal; }
    }

    public Coroutine CoroutineActive
    {
        get { return coroutineActive; }
    }

    public MotionTypes MotionType
    {
        get { return motionType; }
        set { motionType = value; }
    }

    public float TimeToMove
    {
        get { return timeToMove; }
        set { timeToMove = value; }
    }

    protected float TimePassed
    {
        get { return timePassed; }
        set { timePassed = value; }
    }

    public bool Linked
    {
        get { return linked; }
        set { linked = value; }
    }

    public bool Controlled
    {
        get { return controlled; }
        set { controlled = value; }
    }

    public bool Automatic
    {
        get { return automatic; }
        set { automatic = value; }
    }

    public override void Setup()
    {
        interactiveTransform = gameObject.transform;
        interactiveRigidbody = interactiveTransform.GetComponent<Rigidbody>();
        interactivePortal = interactiveTransform.GetComponent<OcclusionPortal>();

        if (interactiveTransform.parent != null)
        {
            if (interactiveTransform.parent.GetComponent<Controller>() != null) linked = true;
            else if (interactiveTransform.parent.GetComponentInChildren<Switch>() != null) controlled = true;
        }

        if (interactiveTransform.GetComponentInParent<Sensor>() != null || interactiveTransform.GetComponent<Sensor>() != null)
        {
            automatic = true;
            interruptible = true;
        }
    }

    public override void Interact()
    {
        if (coroutineActive != null)
        {
            if (timePassed >= timeToMove)
            {
                timePassed = 0;
            }
            else if (interruptible == true)
            {
                StopCoroutine(coroutineActive);
                timePassed = timeToMove - timePassed;
            }
            else
            {
                return;
            }

            coroutineActive = null;
        }
    }

    protected void InteractiveMove(IEnumerator coroutine)
    {
        if (coroutineActive == null) coroutineActive = StartCoroutine(coroutine);
    }

    public void MechanismAdd()
    {
        if (interactiveTransform.parent == null || (interactiveTransform.parent != null && interactiveTransform.parent.GetComponent<Mechanism>() == null))
        {
            GameObject resource;
            GameObject mechanism;

            resource = Resources.Load<GameObject>("Interactives/Controllers/Mechanism");

            mechanism = Instantiate(resource, interactiveTransform.position, interactiveTransform.rotation);
            mechanism.GetComponent<Mechanism>().MobileGet();
            mechanism.name = gameObject.name + resource.name;

            interactiveTransform.SetParent(mechanism.transform);
        }
    }

    public void MechanismRemove()
    {
        if (interactiveTransform.parent != null && interactiveTransform.parent.GetComponent<Mechanism>() != null)
        {
            GameObject mechanism = InteractiveTransform.parent.gameObject;

            interactiveTransform.SetParent(null);

            DestroyImmediate(mechanism);
        }
    }
}
