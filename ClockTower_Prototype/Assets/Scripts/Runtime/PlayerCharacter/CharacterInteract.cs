using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterInteract : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform characterBodyTransform;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private CharacterShoot characterShoot;

    [Header("Object and Component References")]
    [SerializeField] private Transform objectCarriedTransform;
    [SerializeField] private Rigidbody objectCarriedRigidBody;
    [SerializeField] private Renderer objectCarriedRenderer;

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

            if (Physics.Raycast(ray, out hit, length, ~mask) == true)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                {
                    StartCoroutine(HoldObject(hit.transform));
                }
                else if (hit.transform.GetComponent<Interactive>() != null)
                {
                    Interactive interactable = hit.transform.GetComponent<Interactive>();

                    if (interactable.GetComponent<Door>() != null)
                    {
                        if (interactable.GetComponent<Door>().Automatic == true) return;

                        if (interactable.GetComponent<Door>().Pair == true) interactable.GetComponentInParent<DoorController>().EngageDoors();
                        else interactable.Interact();
                    }
                    else
                    {
                        interactable.Interact();
                    }
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

        objectCarriedTransform = objectToHold.transform;
        objectCarriedRigidBody = objectCarriedTransform.GetComponent<Rigidbody>();
        objectCarriedRenderer = objectCarriedTransform.GetComponent<Renderer>();

        objectCarriedRigidBody.freezeRotation = true;
        objectCarriedRigidBody.useGravity = false;

        objectCarriedRenderer.material.color = new Color(objectCarriedRenderer.material.color.r,
                                                         objectCarriedRenderer.material.color.g,
                                                         objectCarriedRenderer.material.color.b,
                                                         0.5f);

        while (use == true && checkThrow == false)
        {
            if (characterShoot.LeftButton == false && characterShoot.RightButton == false)
            {
                Vector3 holdPosition = cameraTransform.position + cameraTransform.forward * holdDistance;
                Vector3 direction = holdPosition - objectCarriedTransform.position;
                Quaternion rotation = characterBodyTransform.rotation;

                objectCarriedRigidBody.MoveRotation(rotation);

                if (objectCarriedTransform.position != holdPosition) objectCarriedRigidBody.velocity = direction * speed;

                if (Vector3.Distance(characterBodyTransform.position, objectCarriedTransform.position) > 3 * holdDistance) objectCarriedRigidBody.velocity = Vector3.zero;
            }
            else
            {
                objectCarriedRigidBody.AddForce(cameraTransform.forward * forceThrow, ForceMode.Impulse);
                checkThrow = true;
            }

            yield return null;
        }

        objectCarriedRenderer.material.color = new Color(objectCarriedRenderer.material.color.r,
                                                         objectCarriedRenderer.material.color.g,
                                                         objectCarriedRenderer.material.color.b,
                                                         1.0f);

        objectCarriedRigidBody.useGravity = true;
        objectCarriedRigidBody.freezeRotation = false;

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
