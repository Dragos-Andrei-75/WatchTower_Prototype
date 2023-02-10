using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [Header("Interactive Object Attributes")]
    [SerializeField] protected float timeToMove = 1.0f;
    [SerializeField] protected float timePassed = 0.0f;
    [SerializeField] protected bool pair = false;
    [SerializeField] protected bool switchEngaged = false;
    [SerializeField] protected bool automatic = false;

    protected Coroutine coroutineActive;

    public bool SwitchEngaged
    {
        get { return switchEngaged; }
    }

    public bool Automatic
    {
        get { return automatic; }
    }

    public float TimeToMove
    {
        get { return timeToMove; }
        set { timeToMove = value; }
    }

    public virtual void Interact()
    {
        if (coroutineActive == null || timePassed >= timeToMove)
        {
            timePassed = 0;
        }
        else
        {
            StopCoroutine(coroutineActive);
            timePassed = timeToMove - timePassed;
        }

        coroutineActive = null;
    }
}
