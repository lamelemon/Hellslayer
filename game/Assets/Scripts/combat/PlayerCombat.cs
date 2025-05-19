using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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
    public float comboResetTime = 1f; // need be longer than attack cooldown
    public int FirstAttackDamage = 5; // !!! taks only hole number because hp system is shit :]
    public int SecondAttackDamage = 6;
    public int ThirdAttackDamage = 8;
    public float FirstAttackKnockbackForce = 15.0f;
    public float SecondAttackKnockbackForce = 20.0f;
    public float ThirdAttackKnockbackForce = 35.0f;
    public Animator animator;
    public Transform attackPoint;
    public Vector3 CombatBox = new(0.8f, 0.8f, 0.8f); // Adjust as needed
    public LayerMask enemyLayer;

    [Header("References")]
    [SerializeField] private PlayerInteraction playerItemInteraction; // Reference to PlayerItemInteraction
    public ArmsPointActionFunc AnimationAction; // scripts that get true if animations is in action part

    // === Private State ===
    private int AttackDamage = 0;
    private float KnockbackForce = 0f;
    private int comboStep = 0;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f; // Tracks attack spam delay
    private float comboTimer = 0f;          // Tracks time to continue combo
    private bool canAttack = true;

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

        //Debug.Log("Attack cooldown timer: " + attackCooldownTimer);
        //Debug.Log("Combo step: " + comboStep);
        //Debug.Log("Combo timer: " + comboTimer);
        //Debug.Log("Can attack: " + canAttack);
        //Debug.Log("Is attacking: " + isAttacking);
        animator.SetBool("isAttacking", isAttacking);
        HandleCombo();
        Debug.Log("AnimationAction.FirstAttackPoint" + AnimationAction.FirstAttackPoint);
        Debug.Log("AnimationAction.SecondAttackPoint" + AnimationAction.SecondAttackPoint);
        Debug.Log("AnimationAction.ThirdAttackPoint" + AnimationAction.ThirdAttackPoint);
    }

    // === Input Handling ===
    private void InputsValuesReader()
    {
        //isAttacking = AttackAction.ReadValue<float>() > 0.1f;
        isAttacking = AttackAction.WasPressedThisFrame() && !PauseMenu.isPaused;// && AttackAction.ReadValue<float>() > 0.1f;
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
        if (isAttacking && canAttack)
        {
            canAttack = true;
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

            if (isAttacking) { PerformAttack();} // Call the function where the attack actually happens

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
        if (!canAttack) { return; }
        // This is a held item attack. !!! needs to make that this script pass the combo number to the item attacks and etc.
        // If holding an item, use its attack
        if (playerItemInteraction.currentlyHeldItem != null)
        {
            if (playerItemInteraction.currentlyHeldItem.TryGetComponent<TestItem>(out TestItem heldItem))
            {
                heldItem.PerformAttack();
                return; // Exit to avoid the default attack logic
            }
        }
        else
        {
            if (comboStep == 1)
            {
                animator.SetTrigger("Attack1");
            }
            else if (comboStep == 2)
            {
                animator.SetTrigger("Attack2");
            }
            else if (comboStep == 3)
            {
                animator.SetTrigger("Attack3");
            }

            // Collect all colliders within the box
            Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, CombatBox, attackPoint.rotation, enemyLayer);
            foreach (Collider enemy in hitEnemies) // !!! Make sure to put atleast enemys gameobject where is rigibody and hp system to the EnemyLayer else result is null
            {
                // Apply damage to the enemy

                hp_system hpSystem = enemy.GetComponent<hp_system>();
                if (hpSystem != null)
                {
                    if (comboStep == 1 && AnimationAction.FirstAttackPoint == true)
                    {
                        AttackDamage = FirstAttackDamage;
                    }
                    else if (comboStep == 2 && AnimationAction.SecondAttackPoint == true)
                    {
                        AttackDamage = SecondAttackDamage;
                    }
                    else if (comboStep == 3 && AnimationAction.ThirdAttackPoint == true)
                    {
                        AttackDamage = ThirdAttackDamage;
                    }
                    hpSystem.take_damage(AttackDamage);
                }
                //Debug.Log("We hit " + enemy.name);

                // Apply knockback to the enemy

                Rigidbody targetRigidbody = enemy.GetComponent<Rigidbody>();
                // Try to get Rigidbody on the same GameObject, if not found, try parent
                if (!enemy.TryGetComponent<Rigidbody>(out targetRigidbody))
                {
                    targetRigidbody = enemy.GetComponentInParent<Rigidbody>();
                }

                if (targetRigidbody != null)
                {
                    //Debug.Log("Applying knockback to: " + enemy.name);
                    if (comboStep == 1 && AnimationAction.FirstAttackPoint == true)
                    {
                        KnockbackForce = FirstAttackKnockbackForce;
                        //Debug.Log("1");
                    }
                    else if (comboStep == 2 && AnimationAction.SecondAttackPoint == true)
                    {
                        KnockbackForce = SecondAttackKnockbackForce;
                        //Debug.Log("2");
                    }
                    else if (comboStep == 3 && AnimationAction.ThirdAttackPoint == true)
                    {
                        KnockbackForce = ThirdAttackKnockbackForce;
                        //Debug.Log("3");
                    }
                    Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    targetRigidbody.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
                }
            }
        }
        AnimationAction.FirstAttackPoint = false;
        AnimationAction.SecondAttackPoint = false;
        AnimationAction.ThirdAttackPoint = false;
        canAttack = false;
        StartCoroutine(AttackCooldownRoutine());
    }
    IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // === Gizmos ===
    // Visualize the attack area in the editor *the red box
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, CombatBox * 2);
    }
}