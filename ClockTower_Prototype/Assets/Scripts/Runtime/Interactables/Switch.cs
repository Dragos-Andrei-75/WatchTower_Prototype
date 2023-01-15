using UnityEngine;
using System;

public class Switch : Interactive
{
    [Header("Object and Component References")]
    [SerializeField] private Interactive[] interactablesControlled;

    [Header("Switch Attributes")]
    [SerializeField] private bool singleUse = false;

    private void Start() => SwitchSetup();

    private void SwitchSetup()
    {
        if (gameObject.transform.parent != null)
        {
            int k = 0;

            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                if (gameObject.transform.parent.GetChild(i).GetComponent<Switch>() == null)
                {
                    k++;

                    Array.Resize(ref interactablesControlled, k);

                    interactablesControlled[k - 1] = gameObject.transform.parent.GetChild(i).GetComponent<Interactive>();
                }
            }
        }
    }

    public override void Interact()
    {
        for (int i = 0; i < interactablesControlled.Length; i++) interactablesControlled[i].Interact();

        if (singleUse == true)
        {
            Array.Resize(ref interactablesControlled, 0);
            enabled = false;
        }
    }
}
