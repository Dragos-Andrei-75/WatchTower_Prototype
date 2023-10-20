using UnityEngine;

public abstract class Door : Mobile
{
    [Header("Door Object and Component Attributes")]
    [SerializeField] private OcclusionPortal interactivePortal;

    [Header("Door Attributes")]
    [SerializeField] private DoorTypes doorType;

    public enum DoorTypes : ushort { Linear, Rotary };

    protected OcclusionPortal InteractivePortal
    {
        get { return interactivePortal; }
    }

    public DoorTypes DoorType
    {
        get { return doorType; }
        set { doorType = value; }
    }

    public override void Setup()
    {
        base.Setup();

        interactivePortal = MobileTransform.GetComponent<OcclusionPortal>();
    }

    public void MechanismAdd()
    {
        if (MobileTransform.parent == null || (MobileTransform.parent != null && MobileTransform.parent.GetComponent<Mechanism>() == null))
        {
            GameObject resource;
            GameObject mechanism;

            resource = Resources.Load<GameObject>("Interactives/Controllers/Mechanism");

            mechanism = Instantiate(resource, MobileTransform.position, MobileTransform.rotation);
            mechanism.name = gameObject.name + resource.name;

            MobileTransform.SetParent(mechanism.transform);

            mechanism.GetComponent<Mechanism>().MobileGet();
        }
    }

    public void MechanismRemove()
    {
        if (MobileTransform.parent != null && MobileTransform.parent.GetComponent<Mechanism>() != null)
        {
            GameObject mechanism = MobileTransform.parent.gameObject;

            MobileTransform.SetParent(null);

            DestroyImmediate(mechanism);
        }
    }
}
