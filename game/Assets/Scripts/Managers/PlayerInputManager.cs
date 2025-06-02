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
    public InputAction AttackInput { get; private set; }
    public InputAction PickupInput { get; private set; }
    public InputAction DropInput { get; private set; }
    public InputAction SlideInput { get; private set; }
    public InputAction SpecialInput { get; private set; }
    public InputAction ReloadInput { get; private set; }
    public Vector2 MoveValue { get; private set; }
    public Vector2 MouseValue { get; private set; }
    public Mouse mouse { get; private set; }

    void Awake()
    {
        ItemSlot1Input = FindAction("ItemSlot1Input");
        ItemSlot2Input = FindAction("ItemSlot2Input");
        MoveInput = FindAction("MovementInput");
        MenuInput = FindAction("MenuInput");
        JumpInput = FindAction("JumpInput");
        SprintInput = FindAction("SprintInput");
        CrouchInput = FindAction("CrouchInput");
        AttackInput = FindAction("AttackInput");
        PickupInput = FindAction("PickupInput");
        DropInput = FindAction("DropInput");
        SlideInput = FindAction("SlideInput");
        SpecialInput = FindAction("SpecialInput");
        ReloadInput = FindAction("ReloadInput");
        mouse = Mouse.current;
    }

    private void Update()
    {
        MoveValue = MoveInput.ReadValue<Vector2>();
        MouseValue = mouse.delta.ReadValue();
    }

    public void OnEnable()
    {
        ItemSlot1Input.Enable();
        ItemSlot2Input.Enable();
        MoveInput.Enable();
        MenuInput.Enable();
        JumpInput.Enable();
        SprintInput.Enable();
        CrouchInput.Enable();
        AttackInput.Enable();
        PickupInput.Enable();
        DropInput.Enable();
        SlideInput.Enable();
        SpecialInput.Enable();
        ReloadInput.Enable();
    }

    public void OnDisable()
    {
        ItemSlot1Input.Disable();
        ItemSlot2Input.Disable();
        MoveInput.Disable();
        MenuInput.Disable();
        JumpInput.Disable();
        SprintInput.Disable();
        CrouchInput.Disable();
        AttackInput.Disable();
        PickupInput.Disable();
        DropInput.Disable();
        SlideInput.Disable();
        SpecialInput.Disable();
    }

    public void DisableExptMenu()
    {
        ItemSlot1Input.Disable();
        ItemSlot2Input.Disable();
        MoveInput.Disable();
        JumpInput.Disable();
        SprintInput.Disable();
        CrouchInput.Disable();
        AttackInput.Disable();
        PickupInput.Disable();
        DropInput.Disable();
        SlideInput.Disable();
        SpecialInput.Disable();
        ReloadInput.Disable();
    }
    
    private InputAction FindAction(string actionName)
    {
        var action = InputSystem.actions.FindAction(actionName);
        if (action == null)
            Debug.LogWarning($"Input action '{actionName}' not found.");
        return action;
    }
}
