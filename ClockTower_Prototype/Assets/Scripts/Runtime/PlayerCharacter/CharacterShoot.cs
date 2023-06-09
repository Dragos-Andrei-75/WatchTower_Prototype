using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterShoot : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform loadOutTransform;
    [SerializeField] private CharacterLook characterLook;
    [SerializeField] private CharacterInteract characterInteract;
    [SerializeField] private CharacterLoadOut characterLoadOut;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputLeftButton;
    [SerializeField] private InputAction inputRightButton;
    [SerializeField] private bool leftButton = false;
    [SerializeField] private bool rightButton = false;

    public delegate void ShootRelease();
    public event ShootRelease OnShootPrimary;
    public event ShootRelease OnShootSecondary;
    public event ShootRelease OnReleasePrimary;
    public event ShootRelease OnReleaseSecondary;

    public bool LeftButton
    {
        get { return leftButton; }
    }

    public bool RightButton
    {
        get { return rightButton; }
    }

    private void Awake()
    {
        inputPlayer = new InputPlayer();

        inputLeftButton = inputPlayer.Character.LeftButton;
        inputLeftButton.started += _ => leftButton = true;
        inputLeftButton.started += _ => Shoot();
        inputLeftButton.canceled += _ => leftButton = false;

        inputRightButton = inputPlayer.Character.RightButton;
        inputRightButton.started += _ => rightButton = true;
        inputRightButton.started += _ => Shoot();
        inputRightButton.canceled += _ => rightButton = false;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputLeftButton.Enable();
        inputRightButton.Enable();

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputLeftButton.Disable();
        inputRightButton.Disable();

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        loadOutTransform = gameObject.transform.GetChild(1).GetComponent<Transform>();

        characterLook = gameObject.transform.GetComponent<CharacterLook>();
        characterInteract = gameObject.transform.root.GetComponent<CharacterInteract>();
        characterLoadOut = gameObject.transform.GetChild(1).GetComponent<CharacterLoadOut>();
    }

    private void Update() => Sway();

    private void Sway()
    {
        float swayX = Mathf.Clamp(characterLook.InputMouseX.ReadValue<float>(), -1, 1);
        float swayY = Mathf.Clamp(characterLook.InputMouseY.ReadValue<float>(), -1, 1);
        float swayMultiplierX = 5.0f;
        float swayMultiplierY = 2.5f;
        float swaySmooth = swayX != 0 || swayY != 0 ? 1.25f : 5.0f;

        Quaternion weaponSwayX = Quaternion.AngleAxis(swayX * swayMultiplierX, Vector3.up);
        Quaternion weaponSwayY = Quaternion.AngleAxis(-swayY * swayMultiplierY, Vector3.right);
        Quaternion weaponSwayTarget = weaponSwayX * weaponSwayY;

        loadOutTransform.localRotation = Quaternion.Slerp(loadOutTransform.localRotation, weaponSwayTarget, swaySmooth * Time.deltaTime);
    }

    private void Shoot()
    {
        if (characterInteract.ObjectCarriedTransform == null && characterInteract.CheckThrow == false)
        {
            if (leftButton == true) StartCoroutine(ShootPrimary());
            else if (rightButton == true) StartCoroutine(ShootSecondary());
        }
    }

    private IEnumerator ShootPrimary()
    {
        if (rightButton == true) rightButton = false;

        while (OnShootPrimary != null && leftButton == true)
        {
            if (characterLoadOut.WeaponCurrent.ammunition[0] > 0 && Time.time > characterLoadOut.WeaponCurrent.fireNext[0]) OnShootPrimary();

            characterLoadOut.WeaponCurrent.heat[0] += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(ReleasePrimary());

        yield break;
    }

    private IEnumerator ShootSecondary()
    {
        if (leftButton == true) leftButton = false;

        while (OnShootSecondary != null && rightButton == true)
        {
            if (characterLoadOut.WeaponCurrent.ammunition[1] > 0 && Time.time > characterLoadOut.WeaponCurrent.fireNext[1]) OnShootSecondary();

            characterLoadOut.WeaponCurrent.heat[1] += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(ReleaseSecondary());

        yield break;
    }

    private IEnumerator ReleasePrimary()
    {
        while (OnReleasePrimary != null && leftButton == false)
        {
            while (characterLoadOut.WeaponCurrent.heat[0] > 0)
            {
                characterLoadOut.WeaponCurrent.heat[0] -= Time.deltaTime;
                yield return null;
            }

            characterLoadOut.WeaponCurrent.heat[0] = 0;

            yield return null;
        }

        yield break;
    }

    private IEnumerator ReleaseSecondary()
    {
        while (OnReleaseSecondary != null && rightButton == false)
        {
            while (characterLoadOut.WeaponCurrent.heat[1] > 0)
            {
                characterLoadOut.WeaponCurrent.heat[1] -= Time.deltaTime;
                yield return null;
            }

            characterLoadOut.WeaponCurrent.heat[1] = 0;

            yield return null;
        }

        yield break;
    }
}
