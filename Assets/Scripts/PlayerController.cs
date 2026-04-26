using UnityEngine;

namespace LULUKA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("移动参数")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float jumpForce = 8f;
        
        [Header("可变跳跃设置")]
        [SerializeField] private float jumpHoldForce = 2f;
        [SerializeField] private float maxJumpHoldTime = 0.5f;
        [SerializeField] private float jumpHoldGravityScale = 0.5f;
        
        [Header("生命值")]
        [SerializeField] private float maxHealth = 100f;
        
        [Header("能量设置")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float energyRegenRate = 5f;
        [SerializeField] private float energyDrainRate = 10f;
        [SerializeField] private float energyFromEnemy = 20f;
        [SerializeField] private float energyFromItem = 30f;
        
        [Header("受击设置")]
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private float knockbackDuration = 0.2f;
        [SerializeField] private float flashDuration = 1f;
        [SerializeField] private float flashInterval = 0.1f;
        
        [Header("双击奔跑设置")]
        [SerializeField] private float doubleTapTime = 0.3f;
        
        [Header("地面检测")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("空中攻击设置")]
        [SerializeField] private int maxAirAttacks = 1;
        
        [Header("动画层索引")]
        [SerializeField] private int transformedLayerIndex = 1;
        
        [Header("技能特效")]
        [SerializeField] private ChargeEffectController chargeEffect;
        [SerializeField] private TransformMagicCircleController transformMagicCircle;
        
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        
        private float horizontalInput;
        private bool isGrounded;
        private bool wasGrounded;
        private bool isRunning;
        
        private float lastTapTimeA;
        private float lastTapTimeD;
        private bool isHoldingA;
        private bool isHoldingD;
        
        private bool isTransformed;
        private bool isTransforming;
        private bool isCharging;
        private int airAttackCount;
        
        private bool isJumping;
        private float jumpHoldTimer;
        private float originalGravityScale;
        
        private float currentHealth;
        private float currentEnergy;
        private bool isInvincible;
        private float invincibleTimer;
        
        private bool wasHoldingChargeKey;
        
        private bool isKnockback;
        private float knockbackTimer;
        
        private bool isFlashing;
        private float flashTimer;
        private float flashCounter;
        
        private readonly int speedHash = Animator.StringToHash("Speed");
        private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int jumpHash = Animator.StringToHash("Jump");
        private readonly int isRunningHash = Animator.StringToHash("IsRunning");
        private readonly int velocityYHash = Animator.StringToHash("VelocityY");
        private readonly int isTransformedHash = Animator.StringToHash("IsTransformed");
        private readonly int transformHash = Animator.StringToHash("Transform");
        private readonly int isChargingHash = Animator.StringToHash("IsCharging");
        private readonly int releaseAttackHash = Animator.StringToHash("ReleaseAttack");
        private readonly int hitHash = Animator.StringToHash("Hit");
        private readonly int dieHash = Animator.StringToHash("Die");
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float CurrentEnergy => currentEnergy;
        public float MaxEnergy => maxEnergy;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            originalGravityScale = rb.gravityScale;
            currentHealth = maxHealth;
            currentEnergy = 0f;
            
            if (chargeEffect == null)
            {
                chargeEffect = GetComponentInChildren<ChargeEffectController>();
            }
            
            if (transformMagicCircle == null)
            {
                transformMagicCircle = GetComponentInChildren<TransformMagicCircleController>();
            }
        }
        
        private void Start()
        {
            animator.SetLayerWeight(transformedLayerIndex, 0f);
        }
        
        private void Update()
        {
            HandleInput();
            CheckGroundStatus();
            UpdateAnimation();
            UpdateJumpHold();
            UpdateInvincibility();
            UpdateFlash();
            UpdateKnockback();
            UpdateEnergy();
        }
        
        private void FixedUpdate()
        {
            Move();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.K) && !isTransforming)
            {
                ToggleTransform();
            }
            
            if (isTransformed && !isTransforming)
            {
                HandleAttackInput();
            }
            
            HandleMovementInput();
            
            if (Input.GetKeyDown(KeyCode.W) && isGrounded && !isCharging)
            {
                Jump();
            }
        }
        
        private void HandleAttackInput()
        {
            bool isHoldingJ = Input.GetKey(KeyCode.J);
            bool pressedJ = Input.GetKeyDown(KeyCode.J);
            bool releasedJ = Input.GetKeyUp(KeyCode.J);
            
            if (isGrounded)
            {
                if (pressedJ && !isCharging)
                {
                    StartCharge();
                }
                else if (releasedJ && isCharging)
                {
                    ReleaseAttack();
                }
            }
            else
            {
                if (pressedJ && airAttackCount < maxAirAttacks)
                {
                    airAttackCount++;
                    animator.SetTrigger("AirAttack");
                }
            }
            
            wasHoldingChargeKey = isHoldingJ;
        }
        
        private void HandleMovementInput()
        {
            bool keyDownA = Input.GetKeyDown(KeyCode.A);
            bool keyDownD = Input.GetKeyDown(KeyCode.D);
            bool keyUpA = Input.GetKeyUp(KeyCode.A);
            bool keyUpD = Input.GetKeyUp(KeyCode.D);
            
            if (keyDownA)
            {
                if (Time.time - lastTapTimeA < doubleTapTime && !isHoldingA)
                {
                    isRunning = true;
                }
                lastTapTimeA = Time.time;
                isHoldingA = true;
            }
            
            if (keyDownD)
            {
                if (Time.time - lastTapTimeD < doubleTapTime && !isHoldingD)
                {
                    isRunning = true;
                }
                lastTapTimeD = Time.time;
                isHoldingD = true;
            }
            
            if (keyUpA)
            {
                isHoldingA = false;
                if (!isHoldingD) isRunning = false;
            }
            
            if (keyUpD)
            {
                isHoldingD = false;
                if (!isHoldingA) isRunning = false;
            }
            
            horizontalInput = isHoldingA ? -1f : (isHoldingD ? 1f : 0f);
        }
        
        private void Move()
        {
            if (isTransforming || isCharging || isKnockback)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }
            
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
            
            if (horizontalInput != 0)
            {
                spriteRenderer.flipX = horizontalInput < 0;
            }
        }
        
        private void Jump()
        {
            if (isTransforming || isCharging || isKnockback) return;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger(jumpHash);
            
            isJumping = true;
            jumpHoldTimer = 0f;
            rb.gravityScale = jumpHoldGravityScale;
        }
        
        private void UpdateJumpHold()
        {
            if (isJumping)
            {
                if (Input.GetKey(KeyCode.W) && jumpHoldTimer < maxJumpHoldTime)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + jumpHoldForce * Time.deltaTime);
                    jumpHoldTimer += Time.deltaTime;
                }
                else
                {
                    EndJumpHold();
                }
            }
            
            if (isJumping && isGrounded && jumpHoldTimer > 0.1f)
            {
                EndJumpHold();
            }
        }
        
        private void EndJumpHold()
        {
            isJumping = false;
            rb.gravityScale = originalGravityScale;
        }
        
        private void CheckGroundStatus()
        {
            wasGrounded = isGrounded;
            isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            
            if (!wasGrounded && isGrounded)
            {
                airAttackCount = 0;
                EndJumpHold();
                
                if (isTransformed && wasHoldingChargeKey && !isCharging)
                {
                    StartCharge();
                }
            }
            
            animator.SetBool(isGroundedHash, isGrounded);
        }
        
        private void UpdateAnimation()
        {
            animator.SetFloat(speedHash, Mathf.Abs(horizontalInput) > 0.1f ? 1f : 0f);
            animator.SetBool(isRunningHash, isRunning);
            animator.SetFloat(velocityYHash, rb.linearVelocity.y);
        }
        
        private void UpdateInvincibility()
        {
            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer <= 0f)
                {
                    isInvincible = false;
                    isFlashing = false;
                    spriteRenderer.enabled = true;
                }
            }
        }
        
        private void UpdateFlash()
        {
            if (isFlashing)
            {
                flashTimer += Time.deltaTime;
                
                if (flashTimer >= flashInterval)
                {
                    flashTimer = 0f;
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                    flashCounter += flashInterval;
                }
            }
        }
        
        private void UpdateKnockback()
        {
            if (isKnockback)
            {
                knockbackTimer -= Time.deltaTime;
                if (knockbackTimer <= 0f)
                {
                    isKnockback = false;
                }
            }
        }
        
        private void UpdateEnergy()
        {
            if (isTransformed)
            {
                currentEnergy -= energyDrainRate * Time.deltaTime;
                
                if (currentEnergy <= 0f)
                {
                    currentEnergy = 0f;
                    ForceCancelTransform();
                }
            }
            else
            {
                currentEnergy += energyRegenRate * Time.deltaTime;
                if (currentEnergy > maxEnergy)
                {
                    currentEnergy = maxEnergy;
                }
            }
        }
        
        private void ToggleTransform()
        {
            if (isTransformed)
            {
                isTransformed = false;
                animator.SetBool(isTransformedHash, false);
                animator.SetLayerWeight(transformedLayerIndex, 0f);
            }
            else
            {
                if (currentEnergy < maxEnergy) return;
                
                isTransforming = true;
                animator.SetTrigger(transformHash);
                
                if (transformMagicCircle != null)
                {
                    transformMagicCircle.StartTransform();
                }
            }
        }
        
        private void ForceCancelTransform()
        {
            isTransformed = false;
            animator.SetBool(isTransformedHash, false);
            animator.SetLayerWeight(transformedLayerIndex, 0f);
        }
        
        public void OnTransformComplete()
        {
            isTransformed = true;
            isTransforming = false;
            animator.SetBool(isTransformedHash, true);
            animator.SetLayerWeight(transformedLayerIndex, 1f);
            
            if (transformMagicCircle != null)
            {
                transformMagicCircle.EndTransform();
            }
        }
        
        private void StartCharge()
        {
            isCharging = true;
            animator.SetBool(isChargingHash, true);
            
            if (chargeEffect != null)
            {
                chargeEffect.SetCharacterFacing(!spriteRenderer.flipX);
                chargeEffect.StartCharge();
            }
        }
        
        private void ReleaseAttack()
        {
            if (!isCharging) return;
            
            isCharging = false;
            animator.SetBool(isChargingHash, false);
            animator.SetTrigger(releaseAttackHash);
            
            if (chargeEffect != null)
            {
                chargeEffect.EndCharge();
            }
        }
        
        private void ForceReleaseCharge()
        {
            if (!isCharging) return;
            
            isCharging = false;
            animator.SetBool(isChargingHash, false);
            
            if (chargeEffect != null)
            {
                chargeEffect.EndCharge();
            }
        }
        
        public void TakeDamage(float damage)
        {
            if (isInvincible) return;
            
            if (isCharging)
            {
                ForceReleaseCharge();
            }
            
            currentHealth -= damage;
            animator.SetTrigger(hitHash);
            
            isInvincible = true;
            invincibleTimer = 1f;
            
            isFlashing = true;
            flashTimer = 0f;
            flashCounter = 0f;
            
            isKnockback = true;
            knockbackTimer = knockbackDuration;
            
            float knockbackDirection = spriteRenderer.flipX ? 1f : -1f;
            rb.linearVelocity = new Vector2(knockbackDirection * knockbackForce, rb.linearVelocity.y + 2f);
            
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
        
        public void AddEnergy(float amount)
        {
            currentEnergy += amount;
            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
        }
        
        public void AddEnergyFromEnemy()
        {
            AddEnergy(energyFromEnemy);
        }
        
        public void AddEnergyFromItem()
        {
            AddEnergy(energyFromItem);
        }
        
        private void Die()
        {
            animator.SetTrigger(dieHash);
            GameManager.Instance?.LoseGame();
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            enabled = false;
        }
        
        public bool IsGrounded => isGrounded;
        public bool IsTransformed => isTransformed;
        public bool IsFacingRight => !spriteRenderer.flipX;
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}