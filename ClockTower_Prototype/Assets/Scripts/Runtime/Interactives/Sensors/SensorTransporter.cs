using UnityEngine;

public class SensorTransporter : Sensor
{
    [Header("Other Object and Component References")]
    [SerializeField] private Linear linear;

    [Header("Elevator Sensor Attributes")]
    [SerializeField] private float reduction = 1.0f;

    public float Reduction
    {
        get { return reduction; }
        set { if (value > 0) reduction = value; }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (linear.Automatic == true || linear.CoroutineActive != null) linear.Interact();
    }

    protected override void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
        base.OnTriggerExit(other);
    }

    public override void SetupSensor()
    {
        base.SetupSensor();

        BoxCollider elevatorCollider;
        float sensorSizeX;
        float sensorSizeZ;

        linear = sensorTransform.GetComponent<Linear>();

        elevatorCollider = ColliderFind(false);

        sensorSizeX = elevatorCollider.size.x - reduction;
        sensorSizeZ = elevatorCollider.size.z - reduction;

        sensorTriggerCollider.size = new Vector3(sensorSizeX, sensorArea, sensorSizeZ);
        sensorTriggerCollider.center = new Vector3(0, (sensorTriggerCollider.size.y / 2) + (elevatorCollider.size.y / 2), 0);
    }
}
