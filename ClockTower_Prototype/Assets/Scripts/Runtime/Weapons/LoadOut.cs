using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class LoadOut : MonoBehaviour
{
    [Header("LoadOut Attributes")]
    [SerializeField] private Transform[] weapons;
    [SerializeField] private Weapon weaponCurrent;
    [SerializeField] private int weaponSelected = 0;
    [SerializeField] private int loadOutSize = 0;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputWheel;
    [SerializeField] private InputAction inputWeapon1;
    [SerializeField] private InputAction inputWeapon2;
    [SerializeField] private InputAction inputWeapon3;
    [SerializeField] private InputAction inputWeapon4;
    [SerializeField] private InputAction inputWeapon5;
    [SerializeField] private InputAction inputWeapon6;
    [SerializeField] private InputAction inputWeapon7;
    [SerializeField] private InputAction inputWeapon8;
    [SerializeField] private InputAction inputWeapon9;
    [SerializeField] private InputAction inputWeapon10;
    [SerializeField] private InputAction inputWeapon11;
    [SerializeField] private float wheelScroll = 0;
    [SerializeField] private int weaponIndex = -1;

    public Weapon WeaponCurrent
    {
        get { return weaponCurrent; }
        set { weaponCurrent = value; }
    }

    private void Awake()
    {
        inputPlayer = new InputPlayer();

        inputWheel = inputPlayer.LoadOut.Wheel;
        inputWheel.performed += _ => WeaponSelect();
        inputWheel.performed += _ => wheelScroll = inputWheel.ReadValue<float>();
        inputWheel.canceled += _ => wheelScroll = 0;

        inputWeapon1 = inputPlayer.LoadOut.Weapon1;
        inputWeapon1.started += _ => weaponIndex = 0;
        inputWeapon1.started += _ => WeaponSelect();

        inputWeapon2 = inputPlayer.LoadOut.Weapon2;
        inputWeapon2.started += _ => weaponIndex = 1;
        inputWeapon2.started += _ => WeaponSelect();

        inputWeapon3 = inputPlayer.LoadOut.Weapon3;
        inputWeapon3.started += _ => weaponIndex = 2;
        inputWeapon3.started += _ => WeaponSelect();

        inputWeapon4 = inputPlayer.LoadOut.Weapon4;
        inputWeapon4.started += _ => weaponIndex = 3;
        inputWeapon4.started += _ => WeaponSelect();

        inputWeapon5 = inputPlayer.LoadOut.Weapon5;
        inputWeapon5.started += _ => weaponIndex = 4;
        inputWeapon5.started += _ => WeaponSelect();

        inputWeapon6 = inputPlayer.LoadOut.Weapon6;
        inputWeapon6.started += _ => weaponIndex = 5;
        inputWeapon6.started += _ => WeaponSelect();

        inputWeapon7 = inputPlayer.LoadOut.Weapon7;
        inputWeapon7.started += _ => weaponIndex = 6;
        inputWeapon7.started += _ => WeaponSelect();

        inputWeapon8 = inputPlayer.LoadOut.Weapon8;
        inputWeapon8.started += _ => weaponIndex = 7;
        inputWeapon8.started += _ => WeaponSelect();

        inputWeapon9 = inputPlayer.LoadOut.Weapon9;
        inputWeapon9.started += _ => weaponIndex = 8;
        inputWeapon9.started += _ => WeaponSelect();

        inputWeapon10 = inputPlayer.LoadOut.Weapon10;
        inputWeapon10.started += _ => weaponIndex = 9;
        inputWeapon10.started += _ => WeaponSelect();

        inputWeapon11 = inputPlayer.LoadOut.Weapon11;
        inputWeapon11.started += _ => weaponIndex = 10;
        inputWeapon11.started += _ => WeaponSelect();
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputWheel.Enable();

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputWheel.Disable();

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void Start()
    {
        SetUpLoadOut();
        WeaponSelect();
    }

    private void SetUpLoadOut()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).parent == gameObject.transform)
            {
                loadOutSize++;

                Array.Resize(ref weapons, loadOutSize);

                weapons[loadOutSize - 1] = gameObject.transform.GetChild(i);
                weapons[loadOutSize - 1].gameObject.SetActive(false);
            }
        }
    }

    private void WeaponSelect()
    {
        weapons[weaponSelected].gameObject.SetActive(false);

        if (wheelScroll != 0) StartCoroutine(WeaponSelectWheel());
        else if (weaponIndex != -1) StartCoroutine(WeaponSelectButton());

        weapons[weaponSelected].gameObject.SetActive(true);

        if (weapons[weaponSelected].transform.childCount != 0) weapons[weaponSelected].transform.GetChild(0).gameObject.SetActive(true);

        weaponCurrent = weapons[weaponSelected].GetComponent<Weapon>();
    }

    private IEnumerator WeaponSelectWheel()
    {
        if (wheelScroll > 0) weaponSelected = weaponSelected == 0 ? weapons.Length - 1 : weaponSelected - 1;
        else if (wheelScroll < 0) weaponSelected = weaponSelected == weapons.Length - 1 ? 0 : weaponSelected + 1;

        yield break;
    }

    private IEnumerator WeaponSelectButton()
    {
        if (weaponIndex < weapons.Length && weapons[weaponIndex] != null) weaponSelected = weaponIndex;

        weaponIndex = -1;

        yield break;
    }
}
