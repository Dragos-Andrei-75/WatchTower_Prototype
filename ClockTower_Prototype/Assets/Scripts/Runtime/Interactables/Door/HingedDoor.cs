using UnityEngine;
using System.Collections;

public class HingedDoor : Door
{
    [Header("Hinged Door Attributes")]
    [SerializeField] private Quaternion rotationClose;
    [SerializeField] private Quaternion rotationOpen;
    [SerializeField] private Quaternion rotationBegin;
    [SerializeField] private Quaternion rotationEnd;
    [SerializeField] private float openRotation = 90.0f;

    public float OpenRotation
    {
        get { return openRotation; }
        set { openRotation = value; }
    }

    public override void DoorSetUp()
    {
        base.DoorSetUp();

        doorType = DoorTypes.HingedDoor;
        rotationClose = doorTransform.rotation;
    }

    public override void Interact()
    {
        base.Interact();
        coroutineActive = StartCoroutine(HingedDoorOpenClose());
    }

    private IEnumerator HingedDoorOpenClose()
    {
        if (doorTransform.rotation.eulerAngles == rotationClose.eulerAngles)
        {
            Transform characterTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Vector3 doorToCharacter = characterTransform.position - doorTransform.position;
            float dot = Vector3.Dot(doorTransform.forward, doorToCharacter);

            if (dot < 0) rotationOpen = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y + openRotation, doorTransform.eulerAngles.z);
            else rotationOpen = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y - openRotation, doorTransform.eulerAngles.z);
        }

        rotationBegin = rotationBegin == rotationClose ? rotationOpen : rotationClose;
        rotationEnd = rotationBegin == rotationClose ? rotationOpen : rotationClose;

        while (doorTransform.eulerAngles != rotationEnd.eulerAngles)
        {
            doorRigidBody.MoveRotation(Quaternion.Lerp(rotationBegin, rotationEnd, timePassed / timeToMove));
            timePassed += Time.deltaTime;

            yield return null;
        }

        yield break;
    }
}
