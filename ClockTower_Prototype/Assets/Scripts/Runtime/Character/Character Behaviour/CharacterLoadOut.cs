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

    [Header("Input LoadOut")]
    [SerializeField] private InputCharacterLoadOut inputCharacterLoadOut;

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

        inputCharacterLoadOut = InputCharacterLoadOut.Instance;

        LoadOutSetup();
    }

    private void OnEnable()
    {
        inputCharacterLoadOut.InputHolster.started += WeaponHolster;

        inputCharacterLoadOut.InputButtons[0].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[1].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[2].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[3].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[4].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[5].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[6].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[7].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[8].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[9].started += WeaponSelect;
        inputCharacterLoadOut.InputButtons[10].started += WeaponSelect;

        inputCharacterLoadOut.InputWheel.performed += WeaponSelect;

        characterInteract.OnCarry += WeaponHolster;

        PickUpWeapon.OnWeaponEquip += WeaponSwitch;
    }

    private void OnDisable()
    {
        inputCharacterLoadOut.InputHolster.started -= WeaponHolster;

        inputCharacterLoadOut.InputButtons[0].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[1].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[2].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[3].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[4].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[5].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[6].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[7].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[8].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[9].started -= WeaponSelect;
        inputCharacterLoadOut.InputButtons[10].started -= WeaponSelect;

        inputCharacterLoadOut.InputWheel.performed -= WeaponSelect;

        characterInteract.OnCarry -= WeaponHolster;

        PickUpWeapon.OnWeaponEquip -= WeaponSwitch;
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
        if (weapons[weaponSelected].gameObject.activeSelf == true)
        {
            int index = 0;

            weapons[weaponSelected].gameObject.SetActive(false);

            if (inputCharacterLoadOut.InputWheel.ReadValue<float>() == 0) WeaponSelectButton();
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
            if (inputCharacterLoadOut.InputButtons[i].ReadValue<float>() == 1 && weapons[i] != null) weaponSelected = i;
        }
    }

    private void WeaponSelectWheel()
    {
        if (inputCharacterLoadOut.InputWheel.ReadValue<float>() > 0)
        {
            do weaponSelected = weaponSelected == 0 ? weapons.Length - 1 : weaponSelected - 1;
            while (weapons[weaponSelected] == null);
        }
        else if (inputCharacterLoadOut.InputWheel.ReadValue<float>() < 0)
        {
            do weaponSelected = weaponSelected == weapons.Length - 1 ? 0 : weaponSelected + 1;
            while (weapons[weaponSelected] == null);
        }
    }
}
