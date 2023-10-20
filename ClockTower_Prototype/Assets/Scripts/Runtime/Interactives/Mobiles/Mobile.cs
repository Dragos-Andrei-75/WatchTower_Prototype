using UnityEngine;
using System.Collections;

public abstract class Mobile : Interactive
{
    [Header("Mobile Object and Component References")]
    [SerializeField] private Transform mobileTransform;
    [SerializeField] private Rigidbody mobileRigidbody;

    [Header("Mobile Attributes")]
    [SerializeField] private float timeToMove = 1.0f;
    [SerializeField] private float timePassed = 0.0f;
    [SerializeField] private bool interruptible = false;
    [SerializeField] private bool linked = false;
    [SerializeField] private bool controlled = false;
    [SerializeField] private bool automatic = false;

    protected Coroutine coroutineActive;

    protected Transform MobileTransform
    {
        get { return mobileTransform; }
    }

    protected Rigidbody MobileRigidbody
    {
        get { return mobileRigidbody; }
    }

    public Coroutine CoroutineActive
    {
        get { return coroutineActive; }
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
        mobileTransform = gameObject.transform;
        mobileRigidbody = mobileTransform.GetComponent<Rigidbody>();

        if (mobileTransform.parent != null)
        {
            if (mobileTransform.parent.GetComponentInChildren<Switch>() != null) controlled = true;
            else if (MobileTransform.parent.GetComponent<Controller>() != null) linked = true;
        }

        if (mobileTransform.GetComponentInParent<Sensor>() != null || mobileTransform.GetComponent<Sensor>() != null)
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

    protected void MoveMobile(IEnumerator coroutine)
    {
        if (coroutineActive == null) coroutineActive = StartCoroutine(coroutine);
    }
}
