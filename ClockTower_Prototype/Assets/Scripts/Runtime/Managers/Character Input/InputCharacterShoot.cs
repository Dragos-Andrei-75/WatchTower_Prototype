using UnityEngine;
using UnityEngine.InputSystem;

public class InputCharacterShoot : InputCharacterSingleton<InputCharacterShoot>
{
    [Header("Input Shoot")]
    [SerializeField] private InputAction inputShootPrimary;
    [SerializeField] private InputAction inputShootSecondary;

    public InputAction InputShootPrimary
    {
        get { return inputShootPrimary; }
    }

    public InputAction InputShootSecondary
    {
        get { return inputShootSecondary; }
    }

    private void Awake()
    {
        inputShootPrimary = Character.Shoot.ShootPrimary;
        inputShootSecondary = Character.Shoot.ShootSecondary;
    }

    protected override void OnEnable()
    {
        inputShootPrimary.Enable();
        inputShootSecondary.Enable();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        inputShootPrimary.Disable();
        inputShootSecondary.Disable();

        base.OnDisable();
    }
}
