using UnityEngine;
using System;
using System.Collections;

public class Elevator : Interactive
{
    [Header("Elevator Object and Component References")]
    [SerializeField] private Transform elevatorTransform;

    [Header("Elevator Attributes")]
    [SerializeField] private ElevatorType elevatorType = ElevatorType.Vertical;
    [SerializeField] private Vector3 position1;
    [SerializeField] private Vector3 position2;
    [SerializeField] private Vector3 positionStart;
    [SerializeField] private Vector3 positionTarget;
    [SerializeField] private float distance = 5;

    [Header("Objects in Relation to the Elevator")]
    [SerializeField] private Transform[] objects;
    [SerializeField] private int objectsNumber = 0;

    private enum ElevatorType { Vertical, Horizontal };

    private void Start()
    {
        elevatorTransform = gameObject.transform;
        ElevatorSetup();
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (reactive == true)
        {
            if (coroutineActive != null)
            {
                Vector3 elevatorDirection = positionTarget - elevatorTransform.position;
                Vector3 elevatorToObject = collision.transform.position - elevatorTransform.position;
                Vector3 objectSize = collision.transform.GetComponent<MeshRenderer>().bounds.size;
                float objectSizeMax = Mathf.Max(Mathf.Max(objectSize.x, objectSize.y), objectSize.z);

                if (Vector3.Dot(elevatorDirection, elevatorToObject) > 0 && Vector3.Distance(elevatorTransform.position, positionTarget) <= objectSizeMax)
                {
                    base.Interact();
                    coroutineActive = StartCoroutine(ElevatorMove());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsNumber++;
        Array.Resize(ref objects, objectsNumber);

        objects[objects.Length - 1] = other.transform;
        objects[objects.Length - 1].SetParent(elevatorTransform);

        if (automatic == true && coroutineActive == null) coroutineActive = StartCoroutine(ElevatorMove());
    }

    private void OnTriggerExit(Collider other)
    {
        int objectRemovedPosition = objectsNumber;

        for (int i = 0; i < objectsNumber; i++)
        {
            if (objects[i] == other.transform)
            {
                objects[i].SetParent(null);
                objects[i] = null;

                objectRemovedPosition = i;
            }
            else if (i > objectRemovedPosition)
            {
                objects[i - 1] = objects[i];
            }
        }

        objectsNumber--;
        Array.Resize(ref objects, objectsNumber);
    }

    private void ElevatorSetup()
    {
        Vector3 levelIncrement;

        if (elevatorType == ElevatorType.Vertical) levelIncrement = new Vector3(0, distance, 0);
        else levelIncrement = elevatorTransform.TransformDirection(new Vector3(distance, 0, 0));

        position1 = elevatorTransform.position;

        position1 = elevatorTransform.position;
        position2 = position1 + levelIncrement;
    }

    public override void Interact()
    {
        if (coroutineActive == null) coroutineActive = StartCoroutine(ElevatorMove());
    }

    private IEnumerator ElevatorMove()
    {
        if (elevatorTransform.position == position1)
        {
            positionStart = position1;
            positionTarget = position2;
        }
        else if (elevatorTransform.position == position2)
        {
            positionStart = position2;
            positionTarget = position1;
        }
        else
        {
            Vector3 positionSwitch;

            positionSwitch = positionStart;
            positionStart = positionTarget;
            positionTarget = positionSwitch;
        }

        while (elevatorTransform.position != positionTarget)
        {
            elevatorTransform.position = Vector3.Lerp(positionStart, positionTarget, timePassed / timeToMove);

            timePassed += Time.deltaTime;
            timePassed = Mathf.Clamp(timePassed, 0, timeToMove);

            yield return null;
        }

        base.Interact();

        yield break;
    }
}
