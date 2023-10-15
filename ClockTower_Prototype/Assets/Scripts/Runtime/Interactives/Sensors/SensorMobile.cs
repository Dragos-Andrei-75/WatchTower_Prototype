using UnityEngine;

using static Mechanism;

public class SensorMobile : Sensor
{
    [Header("Other Object and Component References")]
    [SerializeField] private BoxCollider[] mobilesColliders;
    [SerializeField] private Mobile[] mobiles;

    [Header("Mobile Sensor Attributes")]
    [SerializeField] private Arrangements arrangement = Arrangements.Horizontal;

    public Arrangements Arrangement
    {
        get { return arrangement; }
        set { arrangement = value; }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (colliders.Length == 1) for (int i = 0; i < mobiles.Length; i++) mobiles[i].Interact();
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (colliders.Length == 0) for (int i = 0; i < mobiles.Length; i++) mobiles[i].Interact();
    }

    public override void SetupSensor()
    {
        base.SetupSensor();

        float sensorSizeX;
        float sensorSizeY;

        mobiles = sensorTransform.GetComponentsInChildren<Mobile>();
        mobilesColliders = new BoxCollider[mobiles.Length];

        for (int i = 0; i < mobiles.Length; i++) mobilesColliders[i] = sensorTransform.GetChild(i).GetComponent<BoxCollider>();

        for (int i = 0; i < mobilesColliders.Length; i++) Physics.IgnoreCollision(mobilesColliders[i], sensorTriggerCollider);

        if (arrangement == Arrangements.Horizontal)
        {
            sensorSizeX = mobilesColliders[0].size.x * mobiles.Length;
            sensorSizeY = mobilesColliders[0].size.y;
        }
        else
        {
            sensorSizeX = mobilesColliders[0].size.x;
            sensorSizeY = mobilesColliders[0].size.y * mobiles.Length;
        }

        sensorTriggerCollider.size = new Vector3(sensorSizeX, sensorSizeY, sensorArea);
    }
}
