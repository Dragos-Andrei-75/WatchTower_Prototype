using UnityEngine;
using UnityEngine.InputSystem;

public class InputCharacterInteract : InputCharacterSingleton<InputCharacterInteract>
{
    [Header("Input Interact")]
    [SerializeField] private InputAction inputUse;

    public InputAction InputUse
    {
        get { return inputUse; }
    }

    private void Awake() => inputUse = Character.Interact.Use;

    protected override void OnEnable()
    {
        inputUse.Enable();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        inputUse.Disable();

        base.OnDisable();
    }
}
