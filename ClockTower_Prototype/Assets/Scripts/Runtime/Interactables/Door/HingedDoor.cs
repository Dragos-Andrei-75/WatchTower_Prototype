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
        if (doorTransform.eulerAngles == rotationClose.eulerAngles)
        {
            Transform characterTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Vector3 characterPositionHorizontal = new Vector3(characterTransform.position.x, 0, characterTransform.position.z);
            Vector3 doorPositionHorizontal = new Vector3(doorTransform.position.x, 0, doorTransform.position.z);
            Vector3 doorToCharacterHorizontal = characterPositionHorizontal - doorPositionHorizontal;
            float dot = Vector3.Dot(doorTransform.forward, doorToCharacterHorizontal);

            if (dot < 0) rotationOpen = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y + openRotation, doorTransform.eulerAngles.z);
            else rotationOpen = Quaternion.Euler(doorTransform.eulerAngles.x, doorTransform.eulerAngles.y - openRotation, doorTransform.eulerAngles.z);
        }

        rotationBegin = rotationBegin == rotationClose ? rotationOpen : rotationClose;
        rotationEnd = rotationBegin == rotationClose ? rotationOpen : rotationClose;

        while (doorTransform.eulerAngles != rotationEnd.eulerAngles && timePassed < timeToMove)
        {
            doorTransform.rotation = Quaternion.Lerp(rotationBegin, rotationEnd, timePassed / timeToMove);
            timePassed += Time.deltaTime;

            yield return null;
        }

        doorTransform.eulerAngles = rotationEnd.eulerAngles;

        yield break;
    }
}
