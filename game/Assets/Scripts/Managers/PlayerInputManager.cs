using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public InputAction MoveInput { get; private set; }
    public InputAction ItemSlot1Input { get; private set; }
    public InputAction ItemSlot2Input { get; private set; }
    public InputAction MenuInput { get; private set; }
    public InputAction JumpInput { get; private set; }
    public InputAction SprintInput { get; private set; }
    public InputAction CrouchInput { get; private set; }
    public InputAction MouseInput { get; private set; }
    public InputAction AttackInput { get; private set; }
    public InputAction PickupInput { get; private set; }
    public InputAction DropInput { get; private set; } 
    public InputAction SlideInput { get; private set; }
    public InputAction SpecialInput { get; private set; }
    public Vector2 MoveValue { get; private set; }
    public Vector2 MouseValue { get; private set; }

    private void Awake()
    {
        ItemSlot1Input = InputSystem.actions.FindAction("ItemSlot1Input");
        ItemSlot2Input = InputSystem.actions.FindAction("ItemSlot2Input");
        MoveInput = InputSystem.actions.FindAction("MovementInput");
        MenuInput = InputSystem.actions.FindAction("MenuInput");
        JumpInput = InputSystem.actions.FindAction("JumpInput");
        SprintInput = InputSystem.actions.FindAction("SprintInput");
        CrouchInput = InputSystem.actions.FindAction("CrouchInput");
        MouseInput = InputSystem.actions.FindAction("MouseInput");
        AttackInput = InputSystem.actions.FindAction("AttackInput");
        PickupInput = InputSystem.actions.FindAction("PickupInput");
        DropInput = InputSystem.actions.FindAction("DropInput"); 
        SlideInput = InputSystem.actions.FindAction("SlideInput");
        SpecialInput = InputSystem.actions.FindAction("SpecialInput");    
    }

    private void Update()
    {
        MoveValue = MoveInput.ReadValue<Vector2>();
        MouseValue = MouseInput.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        ItemSlot1Input.Enable();
        ItemSlot2Input.Enable();
        MoveInput.Enable();
        MenuInput.Enable();
        JumpInput.Enable();
        SprintInput.Enable();
        CrouchInput.Enable();
        MouseInput.Enable();
        AttackInput.Enable();
        PickupInput.Enable();
        DropInput.Enable();
        SlideInput.Enable();
        SpecialInput.Enable();
    }

    private void OnDisable()
    {
        ItemSlot1Input.Disable();
        ItemSlot2Input.Disable();
        MoveInput.Disable();
        MenuInput.Disable();
        JumpInput.Disable();
        SprintInput.Disable();
        CrouchInput.Disable();
        MouseInput.Disable();
        AttackInput.Disable();
        PickupInput.Disable();
        DropInput.Disable(); 
        SlideInput.Disable();
        SpecialInput.Disable();
    }
}
