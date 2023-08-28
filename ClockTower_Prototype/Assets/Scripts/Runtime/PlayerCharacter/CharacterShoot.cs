using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class CharacterShoot : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private Transform loadOutTransform;
    [SerializeField] private CharacterLook characterLook;
    [SerializeField] private CharacterInteract characterInteract;
    [SerializeField] private CharacterLoadOut characterLoadOut;

    [Header("Sway Attributes")]
    [SerializeField] private Quaternion weaponSwayTarget;
    [SerializeField] private Quaternion weaponSwayX;
    [SerializeField] private Quaternion weaponSwayY;
    [SerializeField] private float swayX = 0.0f;
    [SerializeField] private float swayY = 0.0f;
    [SerializeField] private float swayXMultiplier = 5.0f;
    [SerializeField] private float swayYMultiplier = 2.5f;
    [SerializeField] private float swaySmooth = 0.0f;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputClickLeft;
    [SerializeField] private InputAction inputClickRight;
    [SerializeField] private bool[] click;

    public delegate void ActionShoot();
    public ActionShoot[] OnShoot;

    public InputAction InputClickLeft
    {
        get { return inputClickLeft; }
    }

    public InputAction InputClickRight
    {
        get { return inputClickRight; }
    }

    private void Awake()
    {
        loadOutTransform = gameObject.transform.GetChild(1).GetComponent<Transform>();

        characterLook = gameObject.transform.GetComponent<CharacterLook>();
        characterInteract = gameObject.transform.root.GetComponent<CharacterInteract>();
        characterLoadOut = gameObject.transform.GetChild(1).GetComponent<CharacterLoadOut>();

        click = new bool[2];

        OnShoot = new ActionShoot[2];

        inputPlayer = new InputPlayer();

        inputClickLeft = inputPlayer.Character.ClickLeft;
        inputClickRight = inputPlayer.Character.ClickRight;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputClickLeft.Enable();
        inputClickRight.Enable();

        inputClickLeft.started += ShootWeapon;
        inputClickRight.started += ShootWeapon;

        Pause.OnPauseResume -= OnEnable;
        Pause.OnPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputClickLeft.Disable();
        inputClickRight.Disable();

        inputClickLeft.started -= ShootWeapon;
        inputClickRight.started -= ShootWeapon;

        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    private void Update() => WeaponSway();

    private void WeaponSway()
    {
        swayX = Mathf.Clamp(characterLook.InputLook.ReadValue<Vector2>().x, -1, 1);
        swayY = Mathf.Clamp(characterLook.InputLook.ReadValue<Vector2>().y, -1, 1);

        swaySmooth = swayX != 0 || swayY != 0 ? 1.25f : 5.0f;

        weaponSwayX = Quaternion.AngleAxis(swayX * swayXMultiplier, Vector3.up);
        weaponSwayY = Quaternion.AngleAxis(-swayY * swayYMultiplier, Vector3.right);

        weaponSwayTarget = weaponSwayX * weaponSwayY;

        loadOutTransform.localRotation = Quaternion.Slerp(loadOutTransform.localRotation, weaponSwayTarget, swaySmooth * Time.deltaTime);
    }

    private void ShootWeapon(InputAction.CallbackContext contextShoot)
    {
        if (characterInteract.CheckHold == false && characterInteract.CheckThrow == false)
        {
            StartCoroutine(ShootInput());

            if (OnShoot[0] != null && click[0] == true) StartCoroutine(Shoot(0));
            else if (OnShoot[1] != null && click[1] == true) StartCoroutine(Shoot(1));
        }
    }

    private IEnumerator ShootInput()
    {
        while (inputClickLeft.ReadValue<float>() != 0 || inputClickRight.ReadValue<float>() != 0)
        {
            click[0] = Convert.ToBoolean(inputClickLeft.ReadValue<float>());
            click[1] = Convert.ToBoolean(inputClickRight.ReadValue<float>());

            yield return null;
        }

        click[0] = false;
        click[1] = false;

        yield break;
    }

    private IEnumerator Shoot(int fire)
    {
        int fireCancel = fire == 0 ? 1 : 0;

        while (OnShoot[fire] != null && click[fire] == true && click[fireCancel] == false)
        {
            if (characterLoadOut.WeaponCurrent[fire].Ammunition > 0 && Time.time > characterLoadOut.WeaponCurrent[fire].FireNext) OnShoot[fire]();

            characterLoadOut.WeaponCurrent[fire].Heat += Time.deltaTime;
            characterLoadOut.WeaponCurrent[fire].Heat = Mathf.Clamp(characterLoadOut.WeaponCurrent[fire].Heat, 0, characterLoadOut.WeaponCurrent[fire].HeatMax);

            yield return null;
        }

        StartCoroutine(Release(fire));

        yield break;
    }

    private IEnumerator Release(int fire)
    {
        while (characterLoadOut.WeaponCurrent[fire].Heat > 0)
        {
            if (OnShoot[fire] != null && click[fire] == false) characterLoadOut.WeaponCurrent[fire].Heat -= Time.deltaTime;
            else yield break;

            yield return null;
        }

        characterLoadOut.WeaponCurrent[fire].Heat = 0;

        yield break;
    }
}
