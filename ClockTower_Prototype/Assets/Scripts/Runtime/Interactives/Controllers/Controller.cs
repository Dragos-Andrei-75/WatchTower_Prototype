using UnityEngine;

public class Controller : Interactive
{
    [Header("Other Object and Component References")]
    [SerializeField] private Interactive[] interactives;

    protected Interactive[] Interactives
    {
        get { return interactives; }
        set { interactives = value; }
    }

    public override void Setup() => interactives = gameObject.transform.GetComponentsInChildren<Mobile>();

    public override void Interact()
    {
        for (int i = 0; i < interactives.Length; i++) interactives[i].Interact();
    }
}
