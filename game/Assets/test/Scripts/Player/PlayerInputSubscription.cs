using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSubscription : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool MenuInput { get; private set; } = false;
    public bool JumpInput { get; private set; } = false;
    public bool SprintInput { get; private set; } = false;
    public bool CrouchInput { get; private set; } = false;

    PlayerInput _Input = null;
    private void Update()
    {
        MenuInput = _Input.Player.MenuInput.WasPressedThisFrame();
        JumpInput = _Input.Player.JumpInput.IsPressed();
        SprintInput = _Input.Player.SprintInput.IsPressed();
        CrouchInput = _Input.Player.CrouchInput.IsPressed();
    }

    private void OnEnable()
    {
        _Input = new PlayerInput();

        _Input.Player.Enable(); // keep at top of function

        _Input.Player.MovementInput.performed += SetMovement;
        _Input.Player.MovementInput.canceled += SetMovement;

        _Input.Player.JumpInput.performed += JumpAction;
        _Input.Player.JumpInput.canceled += JumpAction;

        _Input.Player.SprintInput.performed += SprintAction;
        _Input.Player.SprintInput.canceled += SprintAction;
        
        _Input.Player.CrouchInput.performed += CrouchAction;
        _Input.Player.CrouchInput.canceled += CrouchAction;

    }

    private void OnDisable()
    {
        
        _Input.Player.MovementInput.performed -= SetMovement;
        _Input.Player.MovementInput.canceled -= SetMovement;

        _Input.Player.JumpInput.performed -= JumpAction;
        _Input.Player.JumpInput.canceled -= JumpAction;
        
        _Input.Player.SprintInput.performed -= SprintAction;
        _Input.Player.SprintInput.canceled -= SprintAction;
        
        _Input.Player.CrouchInput.performed -= CrouchAction;
        _Input.Player.CrouchInput.canceled -= CrouchAction;
        
        _Input.Player.Disable(); // keep at bottom of function
    }

    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    void JumpAction(InputAction.CallbackContext ctx)
    {
        JumpInput = ctx.started;
    }

    void SprintAction(InputAction.CallbackContext ctx)
    {
        SprintInput = ctx.started;
    }

    void CrouchAction(InputAction.CallbackContext ctx)
    {
        CrouchInput = ctx.started;
    }
}
