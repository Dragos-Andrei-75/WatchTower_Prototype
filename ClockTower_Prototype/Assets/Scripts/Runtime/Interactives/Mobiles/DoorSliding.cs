using UnityEngine;
using System.Collections;

public class DoorSliding : Door
{
    [Header("Linear Motion Attributes")]
    [SerializeField] private Directions direction = Directions.Down;
    [SerializeField] private Vector3 positionCurrent;
    [SerializeField] private Vector3 positionTarget;
    [SerializeField] private Vector3 position1;
    [SerializeField] private Vector3 position2;
    [SerializeField] private float distance = 10.0f;

    public enum Directions : ushort { Up, Down, Left, Right };

    public Directions Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    protected Vector3 PositionTarget
    {
        get { return positionTarget; }
    }

    public float Distance
    {
        get { return distance; }
        set { distance = value; }
    }

    public override void Setup()
    {
        base.Setup();

        DoorType = DoorTypes.Linear;

        position1 = MobileTransform.position;

        if (direction == Directions.Up) position2 = MobileTransform.position + (MobileTransform.up * distance);
        else if (direction == Directions.Down) position2 = MobileTransform.position - (MobileTransform.up * distance);
        else if (direction == Directions.Left) position2 = MobileTransform.position - (MobileTransform.right * distance);
        else if (direction == Directions.Right) position2 = MobileTransform.position + (MobileTransform.right * distance);
    }

    public override void Interact()
    {
        base.Interact();
        MoveMobile(DoorSlidingMove());
    }

    private IEnumerator DoorSlidingMove()
    {
        positionCurrent = positionCurrent == position1 ? position2 : position1;
        positionTarget = positionCurrent == position1 ? position2 : position1;

        if (MobileTransform.position == position1) InteractivePortal.open = true;

        while (MobileTransform.position != positionTarget)
        {
            MobileRigidbody.MovePosition(Vector3.Lerp(positionCurrent, positionTarget, TimePassed / TimeToMove));
            TimePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (MobileTransform.position == position1) InteractivePortal.open = false;

        yield break;
    }
}
