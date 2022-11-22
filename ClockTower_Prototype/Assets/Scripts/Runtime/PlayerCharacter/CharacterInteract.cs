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

    [Header("Character Attributes")]
    [SerializeField] private float forceThrow = 25.0f;

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

        GrappleGun.OnGrappleInteractable += Interact;

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputUse.Disable();

        GrappleGun.OnGrappleInteractable -= Interact;

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
                    objectCarriedTransform = hit.transform;
                    objectCarriedRigidBody = objectCarriedTransform.GetComponent<Rigidbody>();
                    objectCarriedRenderer = objectCarriedTransform.GetComponent<Renderer>();

                    objectCarriedRigidBody.freezeRotation = true;
                    objectCarriedRigidBody.useGravity = false;

                    objectCarriedRenderer.material.color = new Color(objectCarriedRenderer.material.color.r,
                                                                     objectCarriedRenderer.material.color.g,
                                                                     objectCarriedRenderer.material.color.b,
                                                                     0.5f);

                    use = true;

                    StartCoroutine(CarryObject());
                }
                else if (hit.transform.GetComponent<Interactive>() != null)
                {
                    Interactive interactable = hit.transform.GetComponent<Interactive>();

                    if (interactable.GetComponent<Door>() != null)
                    {
                        if (interactable.GetComponent<Door>().Pair == true) interactable.GetComponentInParent<DoorController>().EngageDoors();
                        if (interactable.GetComponent<Door>().Automatic == true) return;
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

    private IEnumerator CarryObject()
    {
        while (use == true)
        {
            float distance = 2.5f;
            float moveSpeed = 75.0f;
            Vector3 carryPosition = cameraTransform.position + cameraTransform.forward * distance;
            Vector3 direction = carryPosition - objectCarriedTransform.position;

            if (characterShoot.LeftButton == false)
            {
                Quaternion rotation = characterBodyTransform.rotation;

                objectCarriedRigidBody.MoveRotation(rotation);

                if (objectCarriedTransform.position != carryPosition) objectCarriedRigidBody.velocity = direction * moveSpeed;

                if (Vector3.Distance(characterBodyTransform.position, objectCarriedTransform.position) > 3 * distance)
                {
                    objectCarriedRigidBody.velocity = Vector3.zero;
                    use = false;
                }
            }
            else
            {
                objectCarriedRigidBody.AddForce(cameraTransform.forward * forceThrow, ForceMode.Impulse);
                checkThrow = true;
                use = false;
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
            checkThrow = characterShoot.LeftButton == true ? true : false;

            yield return null;
        }

        yield break;
    }
}
