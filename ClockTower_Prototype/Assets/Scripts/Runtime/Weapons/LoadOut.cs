using UnityEngine;
using UnityEngine.InputSystem;

public class LoadOut : MonoBehaviour
{
    [Header("LoadOut Attributes")]
    [SerializeField] private Transform[] weapons;
    [SerializeField] private Weapon weaponCurrent;
    [SerializeField] private int weaponSelected = 0;
    [SerializeField] private int loadOutSize = 11;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputHolster;
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
    [SerializeField] private InputAction inputWheel;
    [SerializeField] private float wheelScroll = 0;
    [SerializeField] private int weaponIndex = -1;

    public Transform[] Weapons
    {
        get { return weapons; }
        set { weapons = value; }
    }

    public Weapon WeaponCurrent
    {
        get { return weaponCurrent; }
        set { weaponCurrent = value; }
    }

    private void Awake()
    {
        weapons = new Transform[loadOutSize];

        inputPlayer = new InputPlayer();

        inputHolster = inputPlayer.LoadOut.Holster;
        inputHolster.started += _ => Holster();

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

        inputWheel = inputPlayer.LoadOut.Wheel;
        inputWheel.performed += _ => WeaponSelect();
        inputWheel.performed += _ => wheelScroll = inputWheel.ReadValue<float>();
        inputWheel.canceled += _ => wheelScroll = 0;
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputWheel.Enable();

        PickUpWeapon.OnWeaponEquip += LoadOutSetUp;

        Pause.onPauseResume -= OnEnable;
        Pause.onPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputWheel.Disable();

        PickUpWeapon.OnWeaponEquip -= LoadOutSetUp;

        Pause.onPauseResume += OnEnable;
        Pause.onPauseResume -= OnDisable;
    }

    private void LoadOutSetUp(int weaponIndexNew)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            int index = gameObject.transform.GetChild(i).GetComponent<Weapon>().WeaponData.index;

            weapons[index] = gameObject.transform.GetChild(i);
            weapons[index].gameObject.SetActive(false);
        }

        if (weapons[weaponIndexNew] != null)
        {
            weaponSelected = weaponIndexNew;
            WeaponSelect();
        }
    }

    private void Holster()
    {
        if (weapons[weaponSelected] != null)
        {
            if (weapons[weaponSelected].gameObject.activeSelf == true) weapons[weaponSelected].gameObject.SetActive(false);
            else weapons[weaponSelected].gameObject.SetActive(true);
        }
    }

    private void WeaponSelect()
    {
        weapons[weaponSelected].gameObject.SetActive(false);

        if (weaponIndex != -1) WeaponSelectButton();
        else if (wheelScroll != 0) WeaponSelectWheel();

        weapons[weaponSelected].gameObject.SetActive(true);

        if (weapons[weaponSelected].transform.childCount != 0)
        {
            for (int i = 0; i < weapons[weaponSelected].transform.childCount; i++)
            {
                weapons[weaponSelected].transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        weaponCurrent = weapons[weaponSelected].GetComponent<Weapon>();
    }

    private void WeaponSelectButton()
    {
        if (weapons[weaponIndex] != null) weaponSelected = weaponIndex;
        weaponIndex = -1;
    }

    private void WeaponSelectWheel()
    {
        if (wheelScroll > 0)
        {
            do
            {
                weaponSelected = weaponSelected == 0 ? weapons.Length - 1 : weaponSelected - 1;
            } while (weapons[weaponSelected] == null);
        }
        else if (wheelScroll < 0)
        {
            do
            {
                weaponSelected = weaponSelected == weapons.Length - 1 ? 0 : weaponSelected + 1;
            } while (weapons[weaponSelected] == null);
        }
    }
}
