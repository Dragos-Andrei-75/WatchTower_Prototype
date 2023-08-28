using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterLook : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private CharacterMovement characterMovement;

    [Header("Look Attributes")]
    [SerializeField] private float lookSensitivityX = 0.05f;
    [SerializeField] private float lookSensitivityY = 0.05f;
    [SerializeField] private float lookXReference = 0.0f;
    [SerializeField] private float lookXMin = -135.0f;
    [SerializeField] private float lookXMax = 135.0f;
    [SerializeField] private float lookYMin = -90.0f;
    [SerializeField] private float lookYMax = 90.0f;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputLook;
    [SerializeField] private float mouseX = 0.0f;
    [SerializeField] private float mouseY = 0.0f;

    public InputAction InputLook
    {
        get { return inputLook; }
    }

    public float MouseX
    {
        get { return mouseX; }
    }

    public float MouseY
    {
        get { return mouseY; }
    }

    public float LookXReference
    {
        get { return lookXReference; }
        set { lookXReference = value; }
    }

    private void Awake()
    {
        characterCameraTransform = gameObject.transform;
        characterTransform = characterCameraTransform.root.GetComponent<Transform>();
        characterMovement = characterCameraTransform.root.GetComponent<CharacterMovement>();

        inputPlayer = new InputPlayer();

        inputLook = inputPlayer.Character.Look;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputLook.Enable();

        inputLook.performed += Look;

        characterMovement.OnActionSlide += LookSlide;

        Pause.OnPauseResume += OnDisable;
        Pause.OnPauseResume -= OnEnable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputLook.Disable();

        inputLook.performed += Look;

        characterMovement.OnActionSlide -= LookSlide;

        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    private void Look(InputAction.CallbackContext contextLook)
    {
        mouseX += inputLook.ReadValue<Vector2>().x * lookSensitivityX;
        mouseY -= inputLook.ReadValue<Vector2>().y * lookSensitivityY;

        mouseY = Mathf.Clamp(mouseY, lookYMin, lookYMax);

        characterCameraTransform.localRotation = Quaternion.Euler(mouseY, characterCameraTransform.localEulerAngles.y, characterCameraTransform.localEulerAngles.z);
        characterTransform.localRotation = Quaternion.Euler(characterTransform.localEulerAngles.x, mouseX, characterTransform.localEulerAngles.z);
    }

    private IEnumerator LookSlide()
    {
        while (characterMovement.CheckSlide == true)
        {
            mouseX = Mathf.Clamp(mouseX, lookXReference + lookXMin, lookXReference + lookXMax);
            yield return null;
        }

        yield break;
    }
}
