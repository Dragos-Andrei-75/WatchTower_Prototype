using UnityEngine;
using UnityEngine.InputSystem;

public class InputCharacterMove : InputCharacterSingleton<InputCharacterMove>
{
    [Header("Input Movement")]
    [SerializeField] private InputAction inputMove;
    [SerializeField] private InputAction inputWalk;
    [SerializeField] private InputAction inputJump;
    [SerializeField] private InputAction inputCrouch;
    [SerializeField] private InputAction inputSwim;
    [SerializeField] private InputAction inputDash;

    public InputAction InputMove
    {
        get { return inputMove; }
    }

    public InputAction InputWalk
    {
        get { return inputWalk; }
    }

    public InputAction InputJump
    {
        get { return inputJump; }
    }

    public InputAction InputCrouch
    {
        get { return inputCrouch; }
    }

    public InputAction InputSwim
    {
        get { return inputSwim; }
    }

    public InputAction InputDash
    {
        get { return inputDash; }
    }

    private void Awake()
    {
        inputMove = Character.Move.Move;
        inputWalk = Character.Move.Walk;
        inputJump = Character.Move.Jump;
        inputCrouch = Character.Move.Crouch;
        inputSwim = Character.Move.Swim;
        inputDash = Character.Move.Dash;
    }

    protected override void OnEnable()
    {
        inputMove.Enable();
        inputWalk.Enable();
        inputJump.Enable();
        inputCrouch.Enable();
        inputSwim.Enable();
        inputDash.Enable();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        inputMove.Disable();
        inputWalk.Disable();
        inputJump.Disable();
        inputCrouch.Disable();
        inputSwim.Disable();
        inputDash.Disable();

        base.OnDisable();
    }
}
