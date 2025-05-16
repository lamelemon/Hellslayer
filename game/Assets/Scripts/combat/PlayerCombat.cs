using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
//Notes:
// in camera holder the right objcet for combat collider is - ORG-hand.R
// chanced the sphere to box dedection for better shape management
public class PlayerCombat : MonoBehaviour
{
    public float attackCooldown = 0.5f;   // Time between attacks
    public float comboResetTime = 1f;
    private int comboStep = 0;
    public bool isAttacking = false;
    private InputAction AttackAction;
    private PlayerInput playerInput;
    private float attackCooldownTimer = 0f; // Tracks attack spam delay
    private float comboTimer = 0f;          // Tracks time to continue combo
    public Animator animator;
    // Define half extents of the box (half the size in each direction)
    public Transform attackPoint; //= transform.Find("YourCharacter/Armature/Hips/Spine/Chest/RightArm/RightHand");
    public Vector3 boxHalfExtents = new Vector3(1.5f, 1f, 1.5f); // Adjust these values as needed
    public LayerMask enemyLayer;
    [SerializeField] private PlayerInteraction playerItemInteraction; // Reference to PlayerItemInteraction

    void Awake()
    {
        playerInput = new PlayerInput(); // Generated Input Actions class
    }
    void LateUpdate()
    {
        Debug.Log("Hand position: " + attackPoint.position); // This will now show movement
    }
    void Start()
    {
        attackPoint = GameObject.Find("ORG-hand.R")?.transform;

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
    private void InputsValuesReader()
    {
        isAttacking = AttackAction.ReadValue<float>() > 0.1f;
    }

    void Update()
    {
        Debug.Log(isAttacking);
        InputsValuesReader();
        // Decrease attack cooldown timer
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        HandleCombo();
    }



    private void OnAttackInput()
    {
        // Proceed with combo
        if (comboStep == 0 || comboTimer > 0f)
        {
            comboStep++;
            comboTimer = comboResetTime;
            attackCooldownTimer = attackCooldown; // Set delay before next attack

            //Debug.Log("Attack " + comboStep);

            PerformAttack(); // Call the function where the attack actually happens

            if (comboStep > 3)
            {
                ResetCombo();
            }
        }
    }
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
    private void ResetCombo()
    {
        Debug.Log("Combo ended");
        comboStep = 0;
        comboTimer = 0f;
        Debug.Log("Attack " + comboStep);
    }


    private void PerformAttack()
    {
        // This code checks whether the player is currently holding an item, and if so, it attempts to trigger the PerformAttack method on that item
        if (playerItemInteraction.currentlyHeldItem != null)
        {
            TestItem heldItem = playerItemInteraction.currentlyHeldItem.GetComponent<TestItem>();
            if (heldItem != null)
            {
                // Call the PerformAttack method on the held item
                heldItem.PerformAttack();
                return; // Exit to avoid the default attack logic
            }
        }

        // If no item is held, proceed with the default attack logic   "hand attacks"
        animator.SetBool("isAttacking", isAttacking);
        animator.Play("ArmsAttack", 0, 0f);
        
        // Collect all colliders within the box
        Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, boxHalfExtents, attackPoint.rotation, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            hp_system hpSystem = enemy.GetComponent<hp_system>();
            if (hpSystem != null)
            {
                hpSystem.take_damage(5); // Default damage
            }
        }
    }




    // Visualize the attack area in the editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
    }
}