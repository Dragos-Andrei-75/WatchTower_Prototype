using UnityEngine;
using UnityEngine.InputSystem;

public class InputCharacterLoadOut : InputCharacterSingleton<InputCharacterLoadOut>
{
    [Header("Input LoadOut")]
    [SerializeField] private InputAction inputHolster;
    [SerializeField] private InputAction inputWheel;
    [SerializeField] private InputAction[] inputButtons;

    public InputAction[] InputButtons
    {
        get { return inputButtons; }
    }

    public InputAction InputWheel
    {
        get { return inputWheel; }
    }

    public InputAction InputHolster
    {
        get { return inputHolster; }
    }

    private void Awake()
    {
        inputButtons = new InputAction[11];

        inputButtons[0] = Character.LoadOut.Weapon1;
        inputButtons[1] = Character.LoadOut.Weapon2;
        inputButtons[2] = Character.LoadOut.Weapon3;
        inputButtons[3] = Character.LoadOut.Weapon4;
        inputButtons[4] = Character.LoadOut.Weapon5;
        inputButtons[5] = Character.LoadOut.Weapon6;
        inputButtons[6] = Character.LoadOut.Weapon7;
        inputButtons[7] = Character.LoadOut.Weapon8;
        inputButtons[8] = Character.LoadOut.Weapon9;
        inputButtons[9] = Character.LoadOut.Weapon10;
        inputButtons[10] = Character.LoadOut.Weapon11;

        inputWheel = Character.LoadOut.Wheel;

        inputHolster = Character.LoadOut.Holster;
    }

    protected override void OnEnable()
    {
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

        inputHolster.Enable();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
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

        inputHolster.Disable();

        base.OnDisable();
    }
}
