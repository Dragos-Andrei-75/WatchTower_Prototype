using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Look Attributes")]
    [SerializeField] private float lookSensitivityX = 2.5f;
    [SerializeField] private float lookSensitivityY = 2.5f;
    [SerializeField] private float lookXReference = 0.0f;
    [SerializeField] private float lookXMin = -150.0f;
    [SerializeField] private float lookXMax = 150.0f;
    [SerializeField] private float lookYMin = -90.0f;
    [SerializeField] private float lookYMax = 90.0f;
    [SerializeField] private float delta = 0.025f;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputMouseX;
    [SerializeField] private InputAction inputMouseY;
    [SerializeField] private float mouseX = 0.0f;
    [SerializeField] private float mouseY = 0.0f;

    public InputAction InputMouseX
    {
        get { return inputMouseX; }
    }

    public InputAction InputMouseY
    {
        get { return inputMouseY; }
    }

    public float MouseX
    {
        get { return mouseX; }
    }

    public float LookXReference
    {
        get { return lookXReference; }
        set { lookXReference = value; }
    }

    private void Awake()
    {
        inputPlayer = new InputPlayer();

        inputMouseX = inputPlayer.Character.MouseX;
        inputMouseY = inputPlayer.Character.MouseY;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputMouseX.Enable();
        inputMouseY.Enable();

        Pause.onPauseResume += OnDisable;
        Pause.onPauseResume -= OnEnable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputMouseX.Disable();
        inputMouseY.Disable();

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        characterCameraTransform = gameObject.transform;
        characterTransform = characterCameraTransform.root.GetComponent<Transform>();
        characterMovement = characterCameraTransform.root.GetComponent<CharacterMovement>();
    }

    private void LateUpdate() => Look();

    private void Look()
    {
        mouseX += inputMouseX.ReadValue<float>() * lookSensitivityX * delta;
        mouseY -= inputMouseY.ReadValue<float>() * lookSensitivityY * delta;

        if (characterMovement.CheckSlide == true) mouseX = Mathf.Clamp(mouseX, lookXReference + lookXMin, lookXReference + lookXMax);
        if (characterMovement.CheckSwim == false) mouseY = Mathf.Clamp(mouseY, lookYMin, lookYMax);

        characterCameraTransform.localRotation = Quaternion.Euler(mouseY, characterCameraTransform.localEulerAngles.y, characterCameraTransform.localEulerAngles.z);
        characterTransform.localRotation = Quaternion.Euler(characterTransform.localEulerAngles.x, mouseX, characterTransform.localEulerAngles.z);
    }
}
