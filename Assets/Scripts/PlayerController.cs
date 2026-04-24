using UnityEngine;

namespace LULUKA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移动参数")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float jumpForce = 8f;
        
        [Header("可变跳跃设置")]
        [SerializeField] private float jumpHoldForce = 2f;
        [SerializeField] private float maxJumpHoldTime = 0.5f;
        [SerializeField] private float jumpHoldGravityScale = 0.5f;
        
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
        
        private readonly int speedHash = Animator.StringToHash("Speed");
        private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int jumpHash = Animator.StringToHash("Jump");
        private readonly int isRunningHash = Animator.StringToHash("IsRunning");
        private readonly int velocityYHash = Animator.StringToHash("VelocityY");
        private readonly int isTransformedHash = Animator.StringToHash("IsTransformed");
        private readonly int transformHash = Animator.StringToHash("Transform");
        private readonly int attackHash = Animator.StringToHash("Attack");
        private readonly int releaseAttackHash = Animator.StringToHash("ReleaseAttack");
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            originalGravityScale = rb.gravityScale;
            
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
            
            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                Jump();
            }
        }
        
        private void HandleAttackInput()
        {
            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.J) && !isCharging)
                {
                    StartCharge();
                }
                else if (Input.GetKeyUp(KeyCode.J) && isCharging)
                {
                    ReleaseAttack();
                }
            }
            else if (Input.GetKeyDown(KeyCode.J) && airAttackCount < maxAirAttacks)
            {
                airAttackCount++;
                animator.SetTrigger(attackHash);
            }
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
            if (isTransforming || isCharging)
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
            if (isTransforming) return;
            
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
            bool wasGrounded = isGrounded;
            isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            
            if (!wasGrounded && isGrounded)
            {
                airAttackCount = 0;
                EndJumpHold();
            }
            
            animator.SetBool(isGroundedHash, isGrounded);
        }
        
        private void UpdateAnimation()
        {
            animator.SetFloat(speedHash, Mathf.Abs(horizontalInput) > 0.1f ? 1f : 0f);
            animator.SetBool(isRunningHash, isRunning);
            animator.SetFloat(velocityYHash, rb.linearVelocity.y);
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
                isTransforming = true;
                animator.SetTrigger(transformHash);
                
                if (transformMagicCircle != null)
                {
                    transformMagicCircle.StartTransform();
                }
            }
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
            animator.SetTrigger(attackHash);
            
            if (chargeEffect != null)
            {
                chargeEffect.SetCharacterFacing(!spriteRenderer.flipX);
                chargeEffect.StartCharge();
            }
        }
        
        private void ReleaseAttack()
        {
            isCharging = false;
            animator.SetTrigger(releaseAttackHash);
            
            if (chargeEffect != null)
            {
                chargeEffect.EndCharge();
            }
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