using UnityEngine;
using System.Collections;

public class Linear : Mobile
{
    [Header("Linear Motion Attributes")]
    [SerializeField] private Directions direction = Directions.Down;
    [SerializeField] private Vector3 positionTarget;
    [SerializeField] private Vector3 positionStart;
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

        MotionType = MotionTypes.Linear;

        position1 = InteractiveTransform.position;

        if (direction == Directions.Up) position2 = InteractiveTransform.position + (InteractiveTransform.up * distance);
        else if (direction == Directions.Down) position2 = InteractiveTransform.position - (InteractiveTransform.up * distance);
        else if (direction == Directions.Left) position2 = InteractiveTransform.position - (InteractiveTransform.right * distance);
        else if (direction == Directions.Right) position2 = InteractiveTransform.position + (InteractiveTransform.right * distance);
    }

    public override void Interact()
    {
        base.Interact();
        InteractiveMove(MotionLinear());
    }

    private IEnumerator MotionLinear()
    {
        positionStart = positionStart == position1 ? position2 : position1;
        positionTarget = positionStart == position1 ? position2 : position1;

        if (InteractiveTransform.position == position1) InteractivePortal.open = true;

        while (InteractiveTransform.position != positionTarget)
        {
            InteractiveRigidbody.MovePosition(Vector3.Lerp(positionStart, positionTarget, TimePassed / TimeToMove));
            TimePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (InteractiveTransform.position == position1) InteractivePortal.open = false;

        yield break;
    }
}
