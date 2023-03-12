using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Object and Component References")]
    [SerializeField] private Door[] doors;

    private void Start() => DoorControllerSetUp();

    private void DoorControllerSetUp()
    {
        doors = new Door[gameObject.transform.childCount];

        for (int i = 0; i < doors.Length; i++) if (gameObject.transform.GetChild(i).GetComponent<Door>() != null) doors[i] = gameObject.transform.GetChild(i).GetComponent<Door>();
    }

    public void Interact()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].Engaged == false) doors[i].Interact();
        }
    }
}
