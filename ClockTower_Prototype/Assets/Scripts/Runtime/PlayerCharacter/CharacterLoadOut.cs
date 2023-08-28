using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLoadOut : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private CharacterInteract characterInteract;

    [Header("LoadOut Attributes")]
    [SerializeField] private Transform[] weapons;
    [SerializeField] private WeaponData[] weaponCurrent;
    [SerializeField] private int weaponSelected = 0;
    [SerializeField] private int loadOutSize = 11;
    [SerializeField] private bool holster = false;

    [Header("Input Player")]
    [SerializeField] private InputPlayer inputPlayer;
    [SerializeField] private InputAction inputHolster;
    [SerializeField] private InputAction inputWheel;
    [SerializeField] private InputAction[] inputButtons;

    public Transform[] Weapons
    {
        get { return weapons; }
        set { weapons = value; }
    }

    public WeaponData[] WeaponCurrent
    {
        get { return weaponCurrent; }
    }

    public bool Holster
    {
        get { return holster; }
    }

    private void Awake()
    {
        characterInteract = gameObject.transform.root.GetComponent<CharacterInteract>();

        weapons = new Transform[loadOutSize];

        inputPlayer = new InputPlayer();

        inputButtons = new InputAction[loadOutSize];

        inputButtons[0] = inputPlayer.LoadOut.Weapon1;
        inputButtons[1] = inputPlayer.LoadOut.Weapon2;
        inputButtons[2] = inputPlayer.LoadOut.Weapon3;
        inputButtons[3] = inputPlayer.LoadOut.Weapon4;
        inputButtons[4] = inputPlayer.LoadOut.Weapon5;
        inputButtons[5] = inputPlayer.LoadOut.Weapon6;
        inputButtons[6] = inputPlayer.LoadOut.Weapon7;
        inputButtons[7] = inputPlayer.LoadOut.Weapon8;
        inputButtons[8] = inputPlayer.LoadOut.Weapon9;
        inputButtons[9] = inputPlayer.LoadOut.Weapon10;
        inputButtons[10] = inputPlayer.LoadOut.Weapon11;

        inputWheel = inputPlayer.LoadOut.Wheel;

        inputHolster = inputPlayer.LoadOut.Holster;

        LoadOutSetup();
    }

    private void OnEnable()
    {
        inputPlayer.Enable();
        inputHolster.Enable();
        inputButtons[0].Enable();
        inputButtons[1].Enable();
        inputButtons[2].Enable();
        inputButtons[3].Enable();
        inputButtons[4].Enable();
        inputButtons[5].Enable();
        inputButtons[6].Enable();
        inputButtons[7].Enable();
        inputButtons[8].Enable();
        inputButtons[9].Enable();
        inputButtons[10].Enable();
        inputWheel.Enable();

        inputHolster.started += WeaponHolster;

        inputButtons[0].started += WeaponSelect;
        inputButtons[1].started += WeaponSelect;
        inputButtons[2].started += WeaponSelect;
        inputButtons[3].started += WeaponSelect;
        inputButtons[4].started += WeaponSelect;
        inputButtons[5].started += WeaponSelect;
        inputButtons[6].started += WeaponSelect;
        inputButtons[7].started += WeaponSelect;
        inputButtons[8].started += WeaponSelect;
        inputButtons[9].started += WeaponSelect;
        inputButtons[10].started += WeaponSelect;

        inputWheel.performed += WeaponSelect;

        characterInteract.OnCarry += WeaponHolster;

        PickUpWeapon.OnWeaponEquip += WeaponSwitch;

        Pause.OnPauseResume -= OnEnable;
        Pause.OnPauseResume += OnDisable;
    }

    private void OnDisable()
    {
        inputPlayer.Disable();
        inputHolster.Disable();
        inputButtons[0].Disable();
        inputButtons[1].Disable();
        inputButtons[2].Disable();
        inputButtons[3].Disable();
        inputButtons[4].Disable();
        inputButtons[5].Disable();
        inputButtons[6].Disable();
        inputButtons[7].Disable();
        inputButtons[8].Disable();
        inputButtons[9].Disable();
        inputButtons[10].Disable();
        inputWheel.Disable();

        inputHolster.started -= WeaponHolster;

        inputButtons[0].started -= WeaponSelect;
        inputButtons[1].started -= WeaponSelect;
        inputButtons[2].started -= WeaponSelect;
        inputButtons[3].started -= WeaponSelect;
        inputButtons[4].started -= WeaponSelect;
        inputButtons[5].started -= WeaponSelect;
        inputButtons[6].started -= WeaponSelect;
        inputButtons[7].started -= WeaponSelect;
        inputButtons[8].started -= WeaponSelect;
        inputButtons[9].started -= WeaponSelect;
        inputButtons[10].started -= WeaponSelect;

        inputWheel.performed -= WeaponSelect;

        characterInteract.OnCarry -= WeaponHolster;

        PickUpWeapon.OnWeaponEquip -= WeaponSwitch;

        Pause.OnPauseResume += OnEnable;
        Pause.OnPauseResume -= OnDisable;
    }

    private void LoadOutSetup()
    {
        for (int i = 0; i < loadOutSize; i++) if (weapons[i] != null && weaponSelected != i) weapons[i].gameObject.SetActive(false);
    }

    private void WeaponSwitch(int index)
    {
        if (weapons[weaponSelected] != null) weapons[weaponSelected].gameObject.SetActive(false);

        weaponSelected = index;

        WeaponSelect();
    }

    private void WeaponHolster(InputAction.CallbackContext contextHolster) => WeaponHolster();

    private void WeaponHolster()
    {
        if (characterInteract.CheckHold == false)
        {
            if (weapons[weaponSelected] != null && weaponSelected != 5)
            {
                if (weapons[weaponSelected].gameObject.activeSelf == true)
                {
                    weapons[weaponSelected].gameObject.SetActive(false);
                    holster = true;
                }
                else
                {
                    weapons[weaponSelected].gameObject.SetActive(true);
                    holster = false;
                }
            }
        }
    }

    private void WeaponSelect(InputAction.CallbackContext contextWeaponSelect) => WeaponSelect();

    private void WeaponSelect()
    {
        if (holster == false)
        {
            int index = 0;

            weapons[weaponSelected].gameObject.SetActive(false);

            if (inputWheel.ReadValue<float>() == 0) WeaponSelectButton();
            else WeaponSelectWheel();

            weapons[weaponSelected].gameObject.SetActive(true);

            if (weapons[weaponSelected].transform.childCount != 0)
            {
                for (int i = 0; i < weapons[weaponSelected].transform.childCount; i++)
                {
                    weapons[weaponSelected].transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            weaponCurrent = new WeaponData[weapons[weaponSelected].GetComponents<Weapon>().Length];

            while (index < weaponCurrent.Length)
            {
                weaponCurrent[index] = weapons[weaponSelected].GetComponents<Weapon>()[index].WeaponData;
                index++;
            }
        }
    }

    private void WeaponSelectButton()
    {
        for (int i = 0; i < loadOutSize; i++)
        {
            if (inputButtons[i].ReadValue<float>() == 1 && weapons[i] != null) weaponSelected = i;
        }
    }

    private void WeaponSelectWheel()
    {
        if (inputWheel.ReadValue<float>() > 0)
        {
            do weaponSelected = weaponSelected == 0 ? weapons.Length - 1 : weaponSelected - 1;
            while (weapons[weaponSelected] == null);
        }
        else if (inputWheel.ReadValue<float>() < 0)
        {
            do weaponSelected = weaponSelected == weapons.Length - 1 ? 0 : weaponSelected + 1;
            while (weapons[weaponSelected] == null);
        }
    }
}
