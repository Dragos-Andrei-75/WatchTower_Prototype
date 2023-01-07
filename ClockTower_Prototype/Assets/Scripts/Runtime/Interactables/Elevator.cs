using UnityEngine;
using System;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Elevator Object and Component References")]
    [SerializeField] private Transform elevatorTransform;

    [Header("Elevator Attributes")]
    [SerializeField] private ElevatorType elevatorType = ElevatorType.Vertical;
    [SerializeField] private Vector3[] floorPositions;
    [SerializeField] private Vector3 floorStart;
    [SerializeField] private Vector3 floorTarget;
    [SerializeField] private float timeToMove = 2.5f;
    [SerializeField] private float timePassed = 0.0f;
    [SerializeField] private float floorDistance = 5;
    [SerializeField] private int floors = 2;

    [Header("Objects on the Elevator")]
    [SerializeField] private Transform[] objects;
    [SerializeField] private int objectsNumber = 0;

    private Coroutine coroutineElevatorMove;

    private enum ElevatorType { Vertical, Horizontal };

    private void Start()
    {
        elevatorTransform = gameObject.transform;
        ElevatorSetup();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsNumber++;
        Array.Resize(ref objects, objectsNumber);

        objects[objects.Length - 1] = other.transform;
        objects[objects.Length - 1].SetParent(elevatorTransform);

        if (coroutineElevatorMove == null) coroutineElevatorMove = StartCoroutine(ElevatorMove());
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

        if (elevatorType == ElevatorType.Vertical) levelIncrement = new Vector3(0, floorDistance, 0);
        else levelIncrement = elevatorTransform.TransformDirection(new Vector3(floorDistance, 0, 0));

        floorPositions = new Vector3[floors];

        floorPositions[0] = elevatorTransform.position;

        for (int i = 1; i < floors; i++) floorPositions[i] = floorPositions[i - 1] + levelIncrement;
    }

    private IEnumerator ElevatorMove()
    {
        floorStart = elevatorTransform.position;

        if (elevatorTransform.position == floorPositions[0]) floorTarget = floorPositions[1];
        else floorTarget = floorPositions[0];

        while (elevatorTransform.position != floorTarget)
        {
            elevatorTransform.position = Vector3.Lerp(floorStart, floorTarget, timePassed / timeToMove);
            timePassed += Time.deltaTime;

            yield return null;
        }

        timePassed = 0;

        coroutineElevatorMove = null;

        yield break;
    }
}
