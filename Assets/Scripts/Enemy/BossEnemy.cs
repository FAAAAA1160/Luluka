using UnityEngine;

namespace LULUKA
{
    public class BossEnemy : EnemyBase
    {
        [Header("BOSS特有设置")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 3f;
        [SerializeField] private float rangedAttackChance = 0.5f;
        
        private bool isFacingRight = true;
        private Vector3 originalSpawnPointLocalPos;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (projectileSpawnPoint != null)
            {
                originalSpawnPointLocalPos = projectileSpawnPoint.localPosition;
            }
            
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EnemyConfig>();
                config.maxHealth = 200f;
                config.moveSpeed = 1.5f;
                config.patrolSpeed = 0.8f;
                config.detectionRange = 8f;
                config.attackRange = 1.5f;
                config.rangedAttackRange = 6f;
                config.patrolRange = 4f;
                config.attackCooldown = 1.5f;
                config.rangedAttackCooldown = 3f;
                config.attackDamage = 20f;
                config.canBeStomped = false;
            }
        }
        
        protected override void Die()
        {
            base.Die();
            GameManager.Instance?.WinGame();
        }
        
        public override void PerformAttack()
        {
            base.PerformAttack();
            
            if (target == null) return;
            
            float distance = Vector2.Distance(transform.position, target.position);
            
            if (distance <= config.attackRange)
            {
                var playerHealth = target.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(config.attackDamage);
                }
            }
        }
        
        public void PerformRangedAttack()
        {
            lastRangedAttackTime = Time.time;
            
            if (projectilePrefab == null) return;
            
            Vector3 spawnPosition = projectileSpawnPoint != null 
                ? projectileSpawnPoint.position 
                : transform.position + new Vector3(isFacingRight ? 1f : -1f, 0.5f, 0f);
            
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            float directionToPlayer = 0f;
            if (target != null)
            {
                directionToPlayer = target.position.x > transform.position.x ? 1f : -1f;
            }
            else
            {
                directionToPlayer = isFacingRight ? 1f : -1f;
            }
            
            var steelRoll = projectile.GetComponent<SteelRollController>();
            if (steelRoll != null)
            {
                steelRoll.Initialize(directionToPlayer);
            }
            else
            {
                var rb2d = projectile.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.linearVelocity = new Vector2(directionToPlayer * projectileSpeed, rb2d.linearVelocity.y);
                }
            }
        }
        
        public bool CanRangedAttack()
        {
            if (Time.time - lastRangedAttackTime < config.rangedAttackCooldown)
            {
                return false;
            }
            
            return Random.value < rangedAttackChance;
        }
        
        public new void Flip(bool facingRight)
        {
            base.Flip(facingRight);
            
            if (isFacingRight != facingRight)
            {
                isFacingRight = facingRight;
                
                if (projectileSpawnPoint != null)
                {
                    Vector3 newLocalPos = originalSpawnPointLocalPos;
                    newLocalPos.x = Mathf.Abs(originalSpawnPointLocalPos.x) * (facingRight ? 1f : -1f);
                    projectileSpawnPoint.localPosition = newLocalPos;
                }
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var playerHealth = collision.gameObject.GetComponent<IDamageable>();
                if (playerHealth != null && currentHealth > 0)
                {
                    playerHealth.TakeDamage(config.attackDamage);
                }
            }
        }
        
        public bool IsFacingRight => isFacingRight;
    }
}