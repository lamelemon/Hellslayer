using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public InputAction MoveInput { get; private set; }
    public InputAction MenuInput { get; private set; }
    public InputAction JumpInput { get; private set; }
    public InputAction SprintInput { get; private set; }
    public InputAction CrouchInput { get; private set; }
    public InputAction MouseInput { get; private set; }
    public InputAction AttackInput { get; private set; }
    public Vector2 MoveValue { get; private set; }
    public Vector2 MouseValue { get; private set; }

    private void Awake()
    {
        MoveInput = InputSystem.actions.FindAction("MovementInput");
        MenuInput = InputSystem.actions.FindAction("MenuInput");
        JumpInput = InputSystem.actions.FindAction("JumpInput");
        SprintInput = InputSystem.actions.FindAction("SprintInput");
        CrouchInput = InputSystem.actions.FindAction("CrouchInput");
        MouseInput = InputSystem.actions.FindAction("MouseInput");
        AttackInput = InputSystem.actions.FindAction("AttackInput");
    }

    private void Update()
    {
        MoveValue = MoveInput.ReadValue<Vector2>();
        MouseValue = MouseInput.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        MoveInput.Enable();
        MenuInput.Enable();
        JumpInput.Enable();
        SprintInput.Enable();
        CrouchInput.Enable();
        MouseInput.Enable();
        AttackInput.Enable();
    }

    private void OnDisable()
    {
        MoveInput.Disable();
        MenuInput.Disable();
        JumpInput.Disable();
        SprintInput.Disable();
        CrouchInput.Disable();
        MouseInput.Disable();
        AttackInput.Disable();
    }
}
