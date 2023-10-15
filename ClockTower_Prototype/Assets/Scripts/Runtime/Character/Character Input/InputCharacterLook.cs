using UnityEngine;
using UnityEngine.InputSystem;

public class InputCharacterLook : InputCharacterSingleton<InputCharacterLook>
{
    [Header("Input Look")]
    [SerializeField] private InputAction inputLook;

    public InputAction InputLook
    {
        get { return inputLook; }
    }

    private void Awake() => inputLook = Character.Look.Look;

    protected override void OnEnable()
    {
        inputLook.Enable();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        inputLook.Disable();

        base.OnDisable();
    }
}
