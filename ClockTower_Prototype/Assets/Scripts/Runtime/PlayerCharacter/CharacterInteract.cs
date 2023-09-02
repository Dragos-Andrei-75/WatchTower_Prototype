using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterInteract : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CharacterLoadOut characterLoadOut;
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Other Object and Component References")]
    [SerializeField] private Transform objectHeldTransform;
    [SerializeField] private Rigidbody objectHeldRigidBody;
    [SerializeField] private Interactable objectHeldInteractable;

    [Header("Character States")]
    [SerializeField] private bool checkHold = false;
    [SerializeField] private bool checkThrow = false;

    [Header("Interaction Attributes")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private Vector3 rayPosition;
    [SerializeField] private int rayPositionX;
    [SerializeField] private int rayPositionY;
    [SerializeField] private int rayLength = 5;

    [Header("Hold Attributes")]
    [SerializeField] private float holdDistance = 2.5f;
    [SerializeField] private float holdSpeed = 100.0f;

    [Header("Throw Attributes")]
    [SerializeField] private float forceThrow = 25.0f;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputUse;

    private Coroutine coroutineHoldObject;

    public delegate void Carry();
    public event Carry OnCarry;

    private Transform ObjectHeldTransform
    {
        get
        {
            return objectHeldTransform;
        }
        set
        {
            objectHeldTransform = value;

            if ((objectHeldTransform != null && characterLoadOut.Holster == false) || (objectHeldTransform == null && characterLoadOut.Holster == true))
            {
                if (OnCarry != null) OnCarry();
            }
        }
    }

    public bool CheckHold
    {
        get { return checkHold; }
    }

    public bool CheckThrow
    {
        get { return checkThrow; }
    }

    private void Awake()
    {
        characterCameraTransform = gameObject.transform.GetChild(0).transform;
        characterCamera = characterCameraTransform.GetComponent<Camera>();
        characterBodyTransform = gameObject.transform.GetChild(1).transform;
        characterMovement = gameObject.transform.GetComponent<CharacterMovement>();
        characterLoadOut = characterCameraTransform.GetChild(1).GetComponent<CharacterLoadOut>();
        characterShoot = characterCameraTransform.GetComponent<CharacterShoot>();

        rayPositionX = Screen.width / 2;
        rayPositionY = Screen.height / 2;

        rayPosition = new Vector3(rayPositionX, rayPositionY);

        layer = LayerMask.GetMask("Player");

        inputPlayer = new InputPlayer();

        inputUse = inputPlayer.Character.Use;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputUse.Enable();

        inputUse.started += Interact;

        GrappleGun.OnGrappleInteractable += HoldObjectStart;

        Pause.OnPauseResume -= OnEnable;
        Pause.OnPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputUse.Disable();

        inputUse.started -= Interact;

        GrappleGun.OnGrappleInteractable -= HoldObjectStart;

        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    private void Interact(InputAction.CallbackContext contextInteract) => Interact();

    private void Interact()
    {
        if (objectHeldTransform == null)
        {
            Ray ray = characterCamera.ScreenPointToRay(rayPosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, ~layer, QueryTriggerInteraction.Ignore) == true)
            {
                if (hit.transform.GetComponent<Interactive>() != null)
                {
                    Interactive interactive = hit.transform.GetComponent<Interactive>();

                    if (interactive.Automatic == false && interactive.SwitchEngaged == false) interactive.Interact();
                }
                else if (hit.transform.GetComponent<Interactable>() != null)
                {
                    HoldObjectStart(hit.transform);
                }
            }
        }
        else
        {
            checkHold = false;
        }
    }

    private void HoldObjectStart(Transform objectToHold)
    {
        ObjectHeldTransform = objectToHold;
        objectHeldRigidBody = objectHeldTransform.GetComponent<Rigidbody>();
        objectHeldInteractable = objectHeldTransform.GetComponent<Interactable>();

        objectHeldRigidBody.useGravity = false;
        objectHeldRigidBody.freezeRotation = true;

        objectHeldInteractable.Held = true;

        coroutineHoldObject = StartCoroutine(HoldObject());

        characterShoot.InputClickLeft.started += ThrowObject;
        characterShoot.InputClickRight.started += ThrowObject;
    }

    private IEnumerator HoldObject()
    {
        Quaternion rotation;
        Vector3 position;
        Vector3 direction;
        float distance;
        float speed;

        checkHold = true;

        while (objectHeldTransform != null && checkHold == true && checkThrow == false)
        {
            rotation = characterBodyTransform.rotation;
            position = characterCameraTransform.position + characterCameraTransform.forward * holdDistance;
            direction = (position - objectHeldTransform.position).normalized;
            distance = Vector3.Distance(objectHeldTransform.position, position);

            speed = Mathf.Lerp(0, holdSpeed, distance);

            if (objectHeldTransform.position != position) objectHeldRigidBody.velocity = direction * speed;

            if (objectHeldInteractable.Contact == false) objectHeldRigidBody.MoveRotation(Quaternion.Lerp(objectHeldTransform.rotation, rotation, Time.fixedDeltaTime * holdSpeed));

            if (Vector3.Distance(characterBodyTransform.position, objectHeldTransform.position) > holdDistance * 3)
            {
                objectHeldRigidBody.velocity = Vector3.zero;
                break;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        HoldObjectStop();

        yield break;
    }

    private void HoldObjectStop()
    {
        Interact();

        characterShoot.InputClickRight.started -= ThrowObject;
        characterShoot.InputClickLeft.started -= ThrowObject;

        StopCoroutine(coroutineHoldObject);
        coroutineHoldObject = null;

        objectHeldInteractable.Held = false;

        objectHeldRigidBody.freezeRotation = false;
        objectHeldRigidBody.useGravity = true;

        objectHeldInteractable = null;
        objectHeldRigidBody = null;
        ObjectHeldTransform = null;
    }

    private void ThrowObject(InputAction.CallbackContext context) => StartCoroutine(ThrowObject());

    private IEnumerator ThrowObject()
    {
        checkThrow = true;

        objectHeldRigidBody.AddForce(characterCameraTransform.forward * forceThrow, ForceMode.Impulse);

        while (characterShoot.InputClickLeft.ReadValue<float>() == 1 || characterShoot.InputClickRight.ReadValue<float>() == 1) yield return null;

        checkThrow = false;

        yield break;
    }
}
