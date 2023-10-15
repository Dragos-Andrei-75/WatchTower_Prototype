using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    protected void Awake() => Setup();

    public abstract void Setup();

    public abstract void Interact();
}
