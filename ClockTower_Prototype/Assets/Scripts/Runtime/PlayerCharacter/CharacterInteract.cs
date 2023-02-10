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
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Object and Component References")]
    [SerializeField] private Transform objectCarriedTransform;
    [SerializeField] private Rigidbody objectCarriedRigidBody;
    [SerializeField] private Renderer objectCarriedRenderer;
    [SerializeField] private Interactable objectCarriedInteractable;

    [Header("Check State")]
    [SerializeField] private bool checkThrow = false;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputUse;
    [SerializeField] private bool use = false;

    public Transform ObjectCarriedTransform
    {
        get { return objectCarriedTransform; }
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

        GrappleGun.OnGrappleInteractable += HoldObject;

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputUse.Disable();

        GrappleGun.OnGrappleInteractable -= HoldObject;

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        cameraTransform = gameObject.transform.GetChild(0).transform;
        characterBodyTransform = gameObject.transform.GetChild(1).transform;
        characterCamera = gameObject.transform.GetChild(0).GetComponent<Camera>();
        characterMovement = gameObject.transform.GetComponent<CharacterMovement>();
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
            LayerMask mask = LayerMask.GetMask("Default");

            if (Physics.Raycast(ray, out hit, length, ~mask, QueryTriggerInteraction.Ignore) == true)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    StartCoroutine(HoldObject(hit.transform));
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

    private IEnumerator HoldObject(Transform objectToHold)
    {
        float holdDistance = 2.5f;
        float forceThrow = 25;
        float speed = 75;

        use = true;

        objectCarriedTransform = objectToHold;
        objectCarriedRigidBody = objectCarriedTransform.GetComponent<Rigidbody>();
        objectCarriedRenderer = objectCarriedTransform.GetComponent<Renderer>();
        objectCarriedInteractable = objectCarriedTransform.GetComponent<Interactable>();

        objectCarriedRigidBody.freezeRotation = true;
        objectCarriedRigidBody.useGravity = false;

        objectCarriedRenderer.material.color = new Color(objectCarriedRenderer.material.color.r,
                                                         objectCarriedRenderer.material.color.g,
                                                         objectCarriedRenderer.material.color.b,
                                                         0.5f);

        characterMovement.CheckCarry = true;

        while (use == true && checkThrow == false)
        {
            if (characterShoot.LeftButton == false && characterShoot.RightButton == false)
            {
                Vector3 holdPosition = cameraTransform.position + cameraTransform.forward * holdDistance;
                Vector3 direction = holdPosition - objectCarriedTransform.position;
                Quaternion rotation = characterBodyTransform.rotation;

                if (direction.magnitude > 1) direction = direction.normalized;

                if (objectCarriedInteractable.Contact == false) objectCarriedRigidBody.MoveRotation(Quaternion.Lerp(objectCarriedTransform.rotation, rotation, Time.deltaTime * 25));

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

            yield return null;
        }

        characterMovement.CheckCarry = false;

        objectCarriedRenderer.material.color = new Color(objectCarriedRenderer.material.color.r,
                                                         objectCarriedRenderer.material.color.g,
                                                         objectCarriedRenderer.material.color.b,
                                                         1.0f);

        objectCarriedRigidBody.useGravity = true;
        objectCarriedRigidBody.freezeRotation = false;

        objectCarriedInteractable = null;
        objectCarriedRenderer = null;
        objectCarriedRigidBody = null;
        objectCarriedTransform = null;

        while (checkThrow == true)
        {
            if (characterShoot.LeftButton == true || characterShoot.RightButton == true) checkThrow = true;
            else checkThrow = false;

            yield return null;
        }

        yield break;
    }
}
