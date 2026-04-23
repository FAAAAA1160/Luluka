using UnityEngine;

namespace LULUKA
{
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
        
        [Header("动画参数名称")]
        [SerializeField] private string speedParam = "Speed";
        [SerializeField] private string isGroundedParam = "IsGrounded";
        [SerializeField] private string jumpParam = "Jump";
        [SerializeField] private string isRunningParam = "IsRunning";
        [SerializeField] private string velocityYParam = "VelocityY";
        [SerializeField] private string isTransformedParam = "IsTransformed";
        [SerializeField] private string transformParam = "Transform";
        [SerializeField] private string attackParam = "Attack";
        [SerializeField] private string releaseAttackParam = "ReleaseAttack";
        
        [Header("动画层索引")]
        [SerializeField] private int transformedLayerIndex = 1;
        
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        
        private float horizontalInput;
        private bool isGrounded;
        private bool wasGrounded;
        private bool canJump = true;
        private bool isRunning = false;
        
        private float lastTapTimeA = 0f;
        private float lastTapTimeD = 0f;
        private bool isHoldingA = false;
        private bool isHoldingD = false;
        
        private bool isTransformed = false;
        private bool isTransforming = false;
        private bool isCharging = false;
        private int airAttackCount = 0;
        
        private int speedHash;
        private int isGroundedHash;
        private int jumpHash;
        private int isRunningHash;
        private int velocityYHash;
        private int isTransformedHash;
        private int transformHash;
        private int attackHash;
        private int releaseAttackHash;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            speedHash = Animator.StringToHash(speedParam);
            isGroundedHash = Animator.StringToHash(isGroundedParam);
            jumpHash = Animator.StringToHash(jumpParam);
            isRunningHash = Animator.StringToHash(isRunningParam);
            velocityYHash = Animator.StringToHash(velocityYParam);
            isTransformedHash = Animator.StringToHash(isTransformedParam);
            transformHash = Animator.StringToHash(transformParam);
            attackHash = Animator.StringToHash(attackParam);
            releaseAttackHash = Animator.StringToHash(releaseAttackParam);
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
                if (isGrounded)
                {
                    if (Input.GetKeyDown(KeyCode.J) && !isCharging)
                    {
                        StartCharge();
                    }
                    
                    if (Input.GetKeyUp(KeyCode.J) && isCharging)
                    {
                        ReleaseAttack();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.J) && airAttackCount < maxAirAttacks)
                    {
                        AirAttack();
                    }
                }
            }
            
            bool keyDownA = Input.GetKeyDown(KeyCode.A);
            bool keyDownD = Input.GetKeyDown(KeyCode.D);
            bool keyUpA = Input.GetKeyUp(KeyCode.A);
            bool keyUpD = Input.GetKeyUp(KeyCode.D);
            
            if (keyDownA)
            {
                if (Time.time - lastTapTimeA < doubleTapTime && isHoldingA == false)
                {
                    isRunning = true;
                }
                lastTapTimeA = Time.time;
                isHoldingA = true;
            }
            
            if (keyDownD)
            {
                if (Time.time - lastTapTimeD < doubleTapTime && isHoldingD == false)
                {
                    isRunning = true;
                }
                lastTapTimeD = Time.time;
                isHoldingD = true;
            }
            
            if (keyUpA)
            {
                isHoldingA = false;
                if (!isHoldingD)
                {
                    isRunning = false;
                }
            }
            
            if (keyUpD)
            {
                isHoldingD = false;
                if (!isHoldingA)
                {
                    isRunning = false;
                }
            }
            
            horizontalInput = 0f;
            
            if (isHoldingA)
            {
                horizontalInput = -1f;
            }
            else if (isHoldingD)
            {
                horizontalInput = 1f;
            }
            
            if (Input.GetKeyDown(KeyCode.W) && isGrounded && canJump)
            {
                DoJump();
            }
        }
        
        private void Move()
        {
            if (isTransforming || isCharging)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                return;
            }
            
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            float targetVelocityX = horizontalInput * currentSpeed;
            
            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
            
            if (horizontalInput != 0)
            {
                spriteRenderer.flipX = horizontalInput < 0;
            }
        }
        
        private void DoJump()
        {
            if (isTransforming) return;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger(jumpHash);
            canJump = false;
        }
        
        private void CheckGroundStatus()
        {
            wasGrounded = isGrounded;
            
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }
            else
            {
                isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.1f;
            }
            
            if (!wasGrounded && isGrounded)
            {
                canJump = true;
                airAttackCount = 0;
            }
            
            animator.SetBool(isGroundedHash, isGrounded);
        }
        
        private void UpdateAnimation()
        {
            float speedValue = Mathf.Abs(horizontalInput) > 0.1f ? 1f : 0f;
            animator.SetFloat(speedHash, speedValue);
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
        
        private void StartCharge()
        {
            isCharging = true;
            animator.SetTrigger(attackHash);
        }
        
        private void ReleaseAttack()
        {
            isCharging = false;
            animator.SetTrigger(releaseAttackHash);
        }
        
        private void AirAttack()
        {
            airAttackCount++;
            animator.SetTrigger(attackHash);
        }
        
        public void SetTransformed(bool transformed)
        {
            isTransformed = transformed;
            animator.SetBool(isTransformedHash, transformed);
            animator.SetLayerWeight(transformedLayerIndex, transformed ? 1f : 0f);
        }
        
        public void TriggerJumpAction()
        {
            if (isGrounded && canJump)
            {
                DoJump();
            }
        }
        
        public bool IsGrounded => isGrounded;
        public bool IsTransformed => isTransformed;
        public int AirAttackCount => airAttackCount;
        
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