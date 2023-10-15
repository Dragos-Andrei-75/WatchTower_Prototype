using UnityEngine;

public class Switch : Controller
{
    [Header("Switch Attributes")]
    [SerializeField] private bool singleUse = false;

    public override void Setup() => Interactives = gameObject.transform.parent.GetComponentsInChildren<Mobile>();

    public override void Interact()
    {
        base.Interact();

        if (singleUse == true) enabled = false;
    }
}
