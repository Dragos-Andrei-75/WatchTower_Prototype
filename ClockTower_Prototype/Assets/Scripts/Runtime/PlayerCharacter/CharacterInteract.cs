using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterInteract : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CharacterLoadOut characterLoadOut;
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Object and Component References")]
    [SerializeField] private Transform objectCarriedTransform;
    [SerializeField] private Rigidbody objectCarriedRigidBody;
    [SerializeField] private Interactable objectCarriedInteractable;

    [Header("Check State")]
    [SerializeField] private bool checkThrow = false;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputUse;
    [SerializeField] private bool use = false;

    private Coroutine coroutineHoldObject;

    public delegate void Carry();
    public event Carry OnCarry;

    public Transform ObjectCarriedTransform
    {
        get
        {
            return objectCarriedTransform;
        }
        set
        {
            objectCarriedTransform = value;

            if ((objectCarriedTransform != null && characterLoadOut.Holster == false) || (objectCarriedTransform == null && characterLoadOut.Holster == true))
            {
                if (OnCarry != null) OnCarry();
            }
        }
    }

    public bool CheckThrow
    {
        get { return checkThrow; }
    }

    private void Awake()
    {
        inputPlayer = new InputPlayer();

        inputUse = inputPlayer.Character.Use;
        inputUse.started += _ => Interact();
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputUse.Enable();

        GrappleGun.OnGrappleInteractable += HoldObjectStart;

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputUse.Disable();

        GrappleGun.OnGrappleInteractable -= HoldObjectStart;

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        cameraTransform = gameObject.transform.GetChild(0).transform;
        characterBodyTransform = gameObject.transform.GetChild(1).transform;
        characterCamera = gameObject.transform.GetChild(0).GetComponent<Camera>();
        characterMovement = gameObject.transform.GetComponent<CharacterMovement>();
        characterLoadOut = cameraTransform.GetChild(1).GetComponent<CharacterLoadOut>();
        characterShoot = cameraTransform.GetComponent<CharacterShoot>();
    }

    private void Interact()
    {
        if (objectCarriedTransform == null)
        {
            int x = Screen.width / 2;
            int y = Screen.height / 2;

            Vector3 rayPosition = new Vector3(x, y);

            Ray ray = characterCamera.ScreenPointToRay(rayPosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hit;
            int length = 5;
            LayerMask layer = LayerMask.GetMask("Default");

            if (Physics.Raycast(ray, out hit, length, ~layer, QueryTriggerInteraction.Ignore) == true)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    HoldObjectStart(hit.transform);
                }
                else if (hit.transform.GetComponent<Interactive>() != null)
                {
                    Interactive interactive = hit.transform.GetComponent<Interactive>();

                    if (interactive.Automatic == true || interactive.SwitchEngaged == true) return;

                    if (interactive.GetComponent<Door>() != null && interactive.GetComponent<Door>().Pair == true) interactive.GetComponentInParent<DoorController>().EngageDoors();
                    else interactive.Interact();
                }
            }
        }
        else
        {
            use = false;
        }
    }

    private void HoldObjectStart(Transform objectToHold)
    {
        ObjectCarriedTransform = objectToHold;
        objectCarriedRigidBody = objectCarriedTransform.GetComponent<Rigidbody>();
        objectCarriedInteractable = objectCarriedTransform.GetComponent<Interactable>();

        objectCarriedRigidBody.freezeRotation = true;
        objectCarriedRigidBody.useGravity = false;

        objectCarriedInteractable.Carried = true;
        objectCarriedInteractable.OnInteractableCarriedDestroy += HoldObjectStop;

        characterMovement.CheckCarry = true;

        coroutineHoldObject = StartCoroutine(HoldObject());
    }

    private IEnumerator HoldObject()
    {
        float holdDistance = 2.5f;
        float forceThrow = 25;
        float speed = 75;

        use = true;

        while (use == true && checkThrow == false)
        {
            if (characterShoot.LeftButton == false && characterShoot.RightButton == false)
            {
                Vector3 holdPosition = cameraTransform.position + cameraTransform.forward * holdDistance;
                Vector3 direction = holdPosition - objectCarriedTransform.position;
                Quaternion rotation = characterBodyTransform.rotation;

                if (direction.magnitude > 1) direction = direction.normalized;

                if (objectCarriedInteractable.Contact == false) objectCarriedRigidBody.MoveRotation(Quaternion.Lerp(objectCarriedTransform.rotation, rotation, Time.fixedDeltaTime * 25));

                if (objectCarriedTransform.position != holdPosition) objectCarriedRigidBody.velocity = direction * speed;

                if (Vector3.Distance(characterBodyTransform.position, objectCarriedTransform.position) > holdDistance * 3)
                {
                    objectCarriedRigidBody.velocity = Vector3.zero;
                    use = false;
                }
            }
            else
            {
                objectCarriedRigidBody.AddForce(cameraTransform.forward * forceThrow, ForceMode.Impulse);
                checkThrow = true;
            }

            yield return new WaitForFixedUpdate();
        }

        while (checkThrow == true)
        {
            if (characterShoot.LeftButton == true || characterShoot.RightButton == true) checkThrow = true;
            else checkThrow = false;

            yield return null;
        }

        HoldObjectStop();

        yield break;
    }

    private void HoldObjectStop()
    {
        StopCoroutine(coroutineHoldObject);
        coroutineHoldObject = null;

        characterMovement.CheckCarry = false;

        objectCarriedInteractable.OnInteractableCarriedDestroy -= HoldObjectStop;
        objectCarriedInteractable.Carried = false;

        objectCarriedRigidBody.useGravity = true;
        objectCarriedRigidBody.freezeRotation = false;

        objectCarriedInteractable = null;
        objectCarriedRigidBody = null;
        ObjectCarriedTransform = null;
    }
}
