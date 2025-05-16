using UnityEngine;
using UnityEngine.InputSystem;
//Notes:
// in camera holder the right objcet for combat collider is - ORG-hand.R
// chanced the sphere to box dedection for better shape management
// PlayerCombat.cs
// Handles player attack input, combos, and attack logic
// !! there is issue when animation moves the arms it dosent get tranforms from hand. So make the CombatBox extra big

public class PlayerCombat : MonoBehaviour
{
    // === Inspector Fields ===
    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;   // Time between attacks
    public float comboResetTime = 1f;
    public Animator animator;
    public Transform attackPoint;
    public Vector3 CombatBox = new Vector3(1.5f, 1f, 1.5f); // Adjust as needed
    public LayerMask enemyLayer;

    [Header("References")]
    [SerializeField] private PlayerInteraction playerItemInteraction; // Reference to PlayerItemInteraction

    // === Private State ===
    private int comboStep = 0;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f; // Tracks attack spam delay
    private float comboTimer = 0f;          // Tracks time to continue combo

    // === Input System ===
    private InputAction AttackAction;
    private PlayerInput playerInput;

    // === Unity Events ===
    void Awake()
    {
        playerInput = new PlayerInput(); // Generated Input Actions class
    }

    private void OnEnable()
    {
        AttackAction = playerInput.Player.AttackInput;
        AttackAction.Enable();
    }

    private void OnDisable()
    {
        AttackAction.Disable();
    }

    void Update()
    {
        InputsValuesReader();

        // Decrease attack cooldown timer
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        HandleCombo();
    }

    // === Input Handling ===
    private void InputsValuesReader()
    {
        isAttacking = AttackAction.ReadValue<float>() > 0.1f;
    }

    // === Combo Logic ===
    private void HandleCombo()
    {
        // If in a combo, tick down the combo timer
        if (comboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

        // If player presses attack input and cooldown allows it
        if (isAttacking && attackCooldownTimer <= 0f)
        {
            OnAttackInput();
        }
    }

    private void OnAttackInput()
    {
        // Proceed with combo
        if (comboStep == 0 || comboTimer > 0f)
        {
            comboStep++;
            comboTimer = comboResetTime;
            attackCooldownTimer = attackCooldown; // Set delay before next attack

            PerformAttack(); // Call the function where the attack actually happens

            if (comboStep > 3)
            {
                ResetCombo();
            }
        }
    }

    private void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
    }

    // === Attack Logic ===
    private void PerformAttack()
    {
        // If holding an item, use its attack
        if (playerItemInteraction.currentlyHeldItem != null)
        {
            TestItem heldItem = playerItemInteraction.currentlyHeldItem.GetComponent<TestItem>();
            if (heldItem != null)
            {
                heldItem.PerformAttack();
                return; // Exit to avoid the default attack logic
            }
        }
        else
        {
            // Default hand attack
            animator.SetBool("isAttacking", isAttacking);
            animator.Play("ArmsAttack1", 0, 0f);

            // Collect all colliders within the box
            Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, CombatBox, attackPoint.rotation, enemyLayer);
            foreach (Collider enemy in hitEnemies)
            {
                hp_system hpSystem = enemy.GetComponent<hp_system>();
                if (hpSystem != null)
                {
                    hpSystem.take_damage(10); // Default damage
                }
                Debug.Log("We hit " + enemy.name);
            }
        }
    }

    // === Gizmos ===
    // Visualize the attack area in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, CombatBox * 2);
    }
}