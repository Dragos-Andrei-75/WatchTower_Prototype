using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterInteract : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private CharacterLoadOut characterLoadOut;

    [Header("Other Object and Component References")]
    [SerializeField] private Transform objectHeldTransform;
    [SerializeField] private Rigidbody objectHeldRigidBody;
    [SerializeField] private Interactable objectHeldInteractable;

    [Header("Character States")]
    [SerializeField] private bool checkHold = false;
    [SerializeField] private bool checkThrow = false;

    [Header("Interaction Attributes")]
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private Vector3 rayPosition;
    [SerializeField] private int rayPositionX;
    [SerializeField] private int rayPositionY;
    [SerializeField] private int rayLength = 5;

    [Header("Hold Attributes")]
    [SerializeField] private float holdDistance = 2.5f;
    [SerializeField] private float holdSpeed = 250.0f;

    [Header("Throw Attributes")]
    [SerializeField] private float forceThrow = 25.0f;

    [Header("Input Interact")]
    [SerializeField] private InputCharacterInteract inputCharacterInteract;
    [SerializeField] private InputCharacterShoot inputCharacterShoot;

    private Coroutine coroutineHoldObject;

    public delegate void Carry();
    public event Carry OnCarry;

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
        characterLoadOut = characterCameraTransform.GetChild(1).GetComponent<CharacterLoadOut>();

        rayPositionX = Screen.width / 2;
        rayPositionY = Screen.height / 2;

        rayPosition = new Vector3(rayPositionX, rayPositionY);

        layerPlayer = LayerMask.GetMask("Player");

        inputCharacterInteract = InputCharacterInteract.Instance;
        inputCharacterShoot = InputCharacterShoot.Instance;
    }

    private void OnEnable()
    {
        inputCharacterInteract.InputUse.started += Interact;

        GrappleGun.OnGrappleInteractable += HoldObjectStart;
    }

    private void OnDisable()
    {
        inputCharacterInteract.InputUse.started -= Interact;

        GrappleGun.OnGrappleInteractable -= HoldObjectStart;
    }

    private void Interact(InputAction.CallbackContext contextInteract) => Interact();

    private void Interact()
    {
        if (objectHeldTransform == null)
        {
            Ray ray = characterCamera.ScreenPointToRay(rayPosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, ~layerPlayer, QueryTriggerInteraction.Ignore) == true)
            {
                if (hit.transform.GetComponent<Interactive>() != null)
                {
                    if (hit.transform.GetComponent<Mobile>() != null)
                    {
                        Mobile mobile = hit.transform.GetComponent<Mobile>();

                        if (mobile.Linked == true)
                        {
                            mobile.GetComponentInParent<Controller>().Interact();
                            return;
                        }
                        else if (mobile.Controlled == true || mobile.Automatic == true)
                        {
                            return;
                        }
                    }

                    hit.transform.GetComponent<Interactive>().Interact();
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
        if (characterLoadOut.Holster == false && OnCarry != null) OnCarry();

        objectHeldTransform = objectToHold;
        objectHeldRigidBody = objectHeldTransform.GetComponent<Rigidbody>();
        objectHeldInteractable = objectHeldTransform.GetComponent<Interactable>();

        objectHeldRigidBody.useGravity = false;

        objectHeldInteractable.Held = true;
        objectHeldInteractable.OnInteractableHeldDestroy += HoldObjectStop;

        coroutineHoldObject = StartCoroutine(HoldObject());

        inputCharacterShoot.InputShootPrimary.started += ThrowObject;
        inputCharacterShoot.InputShootSecondary.started += ThrowObject;
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
            distance = Vector3.Distance(position, objectHeldTransform.position);
            speed = Mathf.Lerp(0, holdSpeed, distance / holdDistance);

            if (objectHeldTransform.position != position) objectHeldRigidBody.velocity = direction * speed;

            if (objectHeldTransform.rotation != rotation && objectHeldInteractable.Contact == false) objectHeldRigidBody.MoveRotation(Quaternion.Lerp(objectHeldTransform.rotation, rotation, Time.fixedDeltaTime * holdSpeed));

            if (Vector3.Distance(characterCameraTransform.position, objectHeldTransform.position) > holdDistance * 3)
            {
                objectHeldRigidBody.velocity = Vector3.zero;
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        HoldObjectStop();

        yield break;
    }

    private void HoldObjectStop()
    {
        Interact();

        inputCharacterShoot.InputShootSecondary.started -= ThrowObject;
        inputCharacterShoot.InputShootPrimary.started -= ThrowObject;

        StopCoroutine(coroutineHoldObject);
        coroutineHoldObject = null;

        objectHeldInteractable.OnInteractableHeldDestroy -= HoldObjectStop;
        objectHeldInteractable.Held = false;

        objectHeldRigidBody.useGravity = true;

        objectHeldInteractable = null;
        objectHeldRigidBody = null;
        objectHeldTransform = null;

        if (characterLoadOut.Holster == true && OnCarry != null) OnCarry();
    }

    private void ThrowObject(InputAction.CallbackContext context) => StartCoroutine(ThrowObject());

    private IEnumerator ThrowObject()
    {
        checkThrow = true;

        objectHeldRigidBody.AddForce(characterCameraTransform.forward * forceThrow, ForceMode.Impulse);

        while (inputCharacterShoot.InputShootPrimary.ReadValue<float>() == 1 || inputCharacterShoot.InputShootSecondary.ReadValue<float>() == 1) yield return null;

        checkThrow = false;

        yield break;
    }
}
