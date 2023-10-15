using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterLook : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform characterCameraTransform;
    [SerializeField] private CharacterMove characterMove;

    [Header("Look Attributes")]
    [SerializeField] private float lookSensitivityX = 0.05f;
    [SerializeField] private float lookSensitivityY = 0.05f;
    [SerializeField] private float lookXMax = 0.0f;
    [SerializeField] private float lookXMin = 0.0f;
    [SerializeField] private float lookYMax = 90.0f;
    [SerializeField] private float lookYMin = -90.0f;

    [Header("Input Look")]
    [SerializeField] private InputCharacterLook inputCharacterLook;
    [SerializeField] private float mouseX = 0.0f;
    [SerializeField] private float mouseY = 0.0f;

    private void Awake()
    {
        characterCameraTransform = gameObject.transform;
        characterTransform = characterCameraTransform.root.GetComponent<Transform>();
        characterMove = characterCameraTransform.root.GetComponent<CharacterMove>();

        inputCharacterLook = InputCharacterLook.Instance;
    }

    private void OnEnable()
    {
        inputCharacterLook.InputLook.performed += Look;

        characterMove.OnActionTurn += LookClamp;
    }

    private void OnDisable()
    {
        inputCharacterLook.InputLook.performed -= Look;

        characterMove.OnActionTurn -= LookClamp;
    }

    private void Look(InputAction.CallbackContext contextLook)
    {
        mouseX += contextLook.ReadValue<Vector2>().x * lookSensitivityX;
        mouseY -= contextLook.ReadValue<Vector2>().y * lookSensitivityY;

        mouseY = Mathf.Clamp(mouseY, lookYMin, lookYMax);

        characterCameraTransform.localRotation = Quaternion.Euler(mouseY, characterCameraTransform.localEulerAngles.y, characterCameraTransform.localEulerAngles.z);

        if (characterMove.CheckTurnClamp == false) characterTransform.rotation = Quaternion.Euler(characterTransform.localEulerAngles.x, mouseX, characterTransform.localEulerAngles.z);
        else characterCameraTransform.localRotation = Quaternion.Euler(mouseY, mouseX, characterCameraTransform.localEulerAngles.z);
    }

    private void LookClamp()
    {
        lookXMax = mouseX + 90;
        lookXMin = mouseX - 90;

        StartCoroutine(LookClampHorizontal());
    }

    private IEnumerator LookClampHorizontal()
    {
        while (characterMove.CheckTurnClamp == true)
        {
            mouseX = Mathf.Clamp(mouseX, lookXMin, lookXMax);
            yield return null;
        }

        lookXMax = 0;
        lookXMin = 0;

        yield break;
    }
}
