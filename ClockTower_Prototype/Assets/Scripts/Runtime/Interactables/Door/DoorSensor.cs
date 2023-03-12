using UnityEngine;
using System;

public class DoorSensor : MonoBehaviour
{
    [Header("Sensor Object and Component References")]
    [SerializeField] private BoxCollider colliderSensor;

    [Header("Object and Component References")]
    [SerializeField] private BoxCollider[] colliderDoors;
    [SerializeField] private Door[] doors;

    [Header("Sensor Attributes")]
    [SerializeField] private Collider[] colliders;
    [SerializeField] private float sensorArea = 10.0f;
    [SerializeField] private OpenTypes openType = OpenTypes.Horizontal;

    public enum OpenTypes : ushort { Horizontal, Vertical };

    public Door[] Doors
    {
        get { return doors; }
    }

    public float SensorArea
    {
        get { return sensorArea; }
        set { if (value > 0) sensorArea = value; }
    }

    public OpenTypes OpenType
    {
        get { return openType; }
        set { openType = value; }
    }

    private void Start()
    {
        SensorSetUp();

        for (int i = 0; i < colliderDoors.Length; i++) Physics.IgnoreCollision(colliderDoors[i], colliderSensor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Interactable>() != null) other.GetComponent<Interactable>().OnInteractableDestroy += RemoveCollider;
        AddCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactable>() != null) other.GetComponent<Interactable>().OnInteractableDestroy -= RemoveCollider;
        RemoveCollider(other);
    }

    private void AddCollider(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collider.gameObject.layer == LayerMask.NameToLayer("Water")) return;

        Array.Resize(ref colliders, colliders.Length + 1);
        colliders[colliders.Length - 1] = collider;

        if (colliders.Length == 1) for (int i = 0; i < doors.Length; i++) doors[i].Interact();
    }

    private void RemoveCollider(Collider collider)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == collider)
            {
                for (int j = i; j < colliders.Length - 1; j++)
                {
                    colliders[j] = colliders[j + 1];
                }
            }
        }

        Array.Resize(ref colliders, colliders.Length - 1);

        if (colliders.Length == 0) for (int i = 0; i < doors.Length; i++) doors[i].Interact();
    }

    public void SensorSetUp()
    {
        Vector3 position;
        Vector3 increment;
        float sensorSizeX;
        float sensorSizeY;

        doors = new Door[gameObject.GetComponentsInChildren<Door>().Length];
        colliderDoors = new BoxCollider[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<Door>() != null)
            {
                doors[i] = gameObject.transform.GetChild(i).GetComponent<Door>();
                colliderDoors[i] = gameObject.transform.GetChild(i).GetComponent<BoxCollider>();
            }
        }

        colliderSensor = gameObject.GetComponent<BoxCollider>();

        if (openType == OpenTypes.Horizontal)
        {
            sensorSizeX = colliderDoors[0].size.x * doors.Length;
            sensorSizeY = colliderDoors[0].size.y;

            if (doors[0].DoorType == Door.DoorTypes.HingedDoor) position = new Vector3(-sensorSizeX / 2 + colliderDoors[0].size.x, 0, 0);
            else position = new Vector3(-sensorSizeX / 2 + colliderDoors[0].size.x / 2, 0, 0);
        }
        else
        {
            sensorSizeX = colliderDoors[0].size.x;
            sensorSizeY = colliderDoors[0].size.y * doors.Length;

            if (doors[0].DoorType == Door.DoorTypes.HingedDoor) position = new Vector3(-sensorSizeX / 2 + colliderDoors[0].size.x, -sensorSizeY / 2 + colliderDoors[0].size.y / 2, 0);
            else position = new Vector3(0, -sensorSizeY / 2 + colliderDoors[0].size.y / 2, 0);
        }

        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].transform.localPosition = position;
            increment = openType == OpenTypes.Horizontal ? new Vector3(colliderDoors[i].size.x, 0, 0) : new Vector3(0, colliderDoors[i].size.y, 0);
            position += increment;

            if (doors[i].DoorType == Door.DoorTypes.HingedDoor)
            {
                if (doors[i].transform.localPosition.x >= sensorSizeX / 2)
                {
                    doors[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                else
                {
                    doors[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    doors[i].transform.localPosition -= new Vector3(colliderDoors[i].size.x, 0, 0);
                }
            }

            doors[i].Setup();
        }

        colliderSensor.size = new Vector3(sensorSizeX, sensorSizeY, sensorArea);
    }

    public void AddDoor() => Instantiate(doors[0].gameObject, gameObject.transform);

    public void RemoveDoor() => DestroyImmediate(doors[doors.Length - 1].gameObject);
}
