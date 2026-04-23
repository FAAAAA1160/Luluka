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
        
        private readonly int  speedHash = Animator.StringToHash("Speed");
        private readonly int  isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int  jumpHash = Animator.StringToHash("Jump");
        private readonly int  isRunningHash = Animator.StringToHash("IsRunning");
        private readonly int  velocityYHash = Animator.StringToHash("VelocityY");
        private readonly int  isTransformedHash = Animator.StringToHash("IsTransformed");
        private readonly int  transformHash = Animator.StringToHash("Transform");
        private readonly int  attackHash = Animator.StringToHash("Attack");
        private readonly int  releaseAttackHash = Animator.StringToHash("ReleaseAttack");
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
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
                    isCharging = true;
                    animator.SetTrigger(attackHash);
                }
                else if (Input.GetKeyUp(KeyCode.J) && isCharging)
                {
                    isCharging = false;
                    animator.SetTrigger(releaseAttackHash);
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
        }
        
        private void CheckGroundStatus()
        {
            bool wasGrounded = isGrounded;
            isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            
            if (!wasGrounded && isGrounded)
            {
                airAttackCount = 0;
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
            }
        }
        
        public void OnTransformComplete()
        {
            isTransformed = true;
            isTransforming = false;
            animator.SetBool(isTransformedHash, true);
            animator.SetLayerWeight(transformedLayerIndex, 1f);
        }
        
        public bool IsGrounded => isGrounded;
        public bool IsTransformed => isTransformed;
        
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