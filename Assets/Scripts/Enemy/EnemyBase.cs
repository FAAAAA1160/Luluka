using UnityEngine;

namespace LULUKA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("配置")]
        [SerializeField] protected EnemyConfig config;
        
        protected Rigidbody2D rb;
        protected Animator animator;
        protected Collider2D enemyCollider;
        protected SpriteRenderer spriteRenderer;
        
        protected float currentHealth;
        protected Transform target;
        protected EnemyState currentState;
        
        protected float lastAttackTime;
        protected float lastRangedAttackTime;
        
        public EnemyConfig Config => config;
        public Animator Animator => animator;
        public Transform Target => target;
        public float CurrentHealth => currentHealth;
        
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            currentHealth = config != null ? config.maxHealth : 100f;
        }
        
        protected virtual void Start()
        {
            InitializePlayer();
            ChangeState(new PatrolState(this));
        }
        
        protected virtual void Update()
        {
            currentState?.Update();
        }
        
        protected virtual void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
        
        public void ChangeState(EnemyState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        
        protected void InitializePlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        public bool IsPlayerInRange(float range)
        {
            if (target == null)
            {
                InitializePlayer();
                return false;
            }
            
            return Vector2.Distance(transform.position, target.position) <= range;
        }
        
        public void Move(float direction)
        {
            rb.linearVelocity = new Vector2(direction, rb.linearVelocity.y);
        }
        
        public void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        public void Flip(bool facingRight)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !facingRight;
            }
        }
        
        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            if (animator != null)
            {
                animator.SetTrigger(Animator.StringToHash("Hit"));
            }
            
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
        
        public virtual void TakeStompDamage(float damage)
        {
            if (config != null && config.canBeStomped)
            {
                TakeDamage(damage);
            }
        }
        
        protected virtual void Die()
        {
            ChangeState(new DeathState(this));
        }
        
        public virtual void PerformAttack()
        {
            lastAttackTime = Time.time;
        }
        
        public virtual void DisableCollider()
        {
            if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
        }
        
        public virtual void DisablePhysics()
        {
            if (rb != null)
            {
                rb.simulated = false;
            }
        }
        
        public virtual void OnDeathComplete()
        {
            Destroy(gameObject);
        }
        
        public bool CanBeStomped => config != null && config.canBeStomped;
        
        public virtual bool IsFacingRight => !spriteRenderer.flipX;
    }
}