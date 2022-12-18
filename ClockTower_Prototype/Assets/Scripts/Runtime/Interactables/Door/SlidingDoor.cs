using UnityEngine;
using System.Collections;

public class SlidingDoor : Door
{
    [Header("Sliding Door Attributes")]
    [SerializeField] private OpenDirections openDirection = OpenDirections.Down;
    [SerializeField] private Vector3 positionClose;
    [SerializeField] private Vector3 positionOpen;
    [SerializeField] private Vector3 positionBegin;
    [SerializeField] private Vector3 positionEnd;
    [SerializeField] private float openDistance = 9.75f;

    public enum OpenDirections : ushort { Up, Down, Left, Right };

    public float OpenDistance
    {
        get { return openDistance; }
        set { openDistance = value; }
    }

    public OpenDirections OpenDirection
    {
        get { return openDirection; }
        set { openDirection = value; }
    }

    public override void DoorSetUp()
    {
        base.DoorSetUp();

        doorType = DoorTypes.SlidingDoor;
        positionClose = doorTransform.position;

        if (openDirection == OpenDirections.Up) positionOpen = doorTransform.position + (doorTransform.up * openDistance);
        else if (openDirection == OpenDirections.Down) positionOpen = doorTransform.position - (doorTransform.up * openDistance);
        else if (openDirection == OpenDirections.Left) positionOpen = doorTransform.position - (doorTransform.right * openDistance);
        else if (openDirection == OpenDirections.Right) positionOpen = doorTransform.position + (doorTransform.right * openDistance);
    }

    public override void Interact()
    {
        base.Interact();
        coroutineActive = StartCoroutine(SlidingDoorOpenClose());
    }

    private IEnumerator SlidingDoorOpenClose()
    {
        positionBegin = positionBegin == positionClose ? positionOpen : positionClose;
        positionEnd = positionBegin == positionClose ? positionOpen : positionClose;

        while (doorTransform.position != positionEnd && timePassed < timeToMove)
        {
            doorRigidBody.MovePosition(Vector3.Lerp(positionBegin, positionEnd, timePassed / timeToMove));
            timePassed += Time.deltaTime;

            yield return null;
        }

        doorTransform.position = positionEnd;

        yield break;
    }
}
