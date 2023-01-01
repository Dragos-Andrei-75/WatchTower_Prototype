using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Elevator Object and Component References")]
    [SerializeField] private Transform elevatorTransform;
    [SerializeField] private Rigidbody elevatorRigidbody;

    [Header("Elevator Attributes")]
    [SerializeField] private ElevatorType elevatorType = ElevatorType.Vertical;
    [SerializeField] private Vector3[] floorPositions;
    [SerializeField] private Vector3 floorStart;
    [SerializeField] private Vector3 floorTarget;
    [SerializeField] private float timeToMove = 2.5f;
    [SerializeField] private float timePassed = 0.0f;
    [SerializeField] private float floorDistance = 5;
    [SerializeField] private int floors = 2;

    [Header("Entities on the Elevator")]
    [SerializeField] private CharacterController characterController;

    private Coroutine coroutineElevatorMove;

    private enum ElevatorType { Vertical, Horizontal };

    private void Start()
    {
        elevatorTransform = gameObject.transform;
        elevatorRigidbody = elevatorTransform.GetComponent<Rigidbody>();

        ElevatorSetup();
    }

    private void OnTriggerEnter(Collider other)
    {
        characterController = other.transform.GetComponent<CharacterController>();
        if (coroutineElevatorMove == null) coroutineElevatorMove = StartCoroutine(ElevatorMove());
    }

    private void OnTriggerStay(Collider other) => characterController.Move(elevatorRigidbody.velocity * Time.deltaTime);

    private void OnTriggerExit(Collider other) => characterController = null;

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
            elevatorRigidbody.MovePosition(Vector3.Lerp(floorStart, floorTarget, timePassed / timeToMove));
            timePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        timePassed = 0;

        coroutineElevatorMove = null;

        yield break;
    }
}
