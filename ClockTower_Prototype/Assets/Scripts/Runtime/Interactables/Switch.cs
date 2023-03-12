using UnityEngine;
using System;

public class Switch : Interactive
{
    [Header("Object and Component References")]
    [SerializeField] private Interactive[] interactivesControlled;

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

                    Array.Resize(ref interactivesControlled, k);

                    interactivesControlled[k - 1] = gameObject.transform.parent.GetChild(i).GetComponent<Interactive>();
                }
            }
        }
    }

    public override void Interact()
    {
        for (int i = 0; i < interactivesControlled.Length; i++) interactivesControlled[i].Interact();

        if (singleUse == true) enabled = false;
    }
}
