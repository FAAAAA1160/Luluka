using UnityEngine;

namespace LULUKA
{
    public class BossEnemy : EnemyBase
    {
        [Header("BOSS特有设置")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 5f;
        [SerializeField] private float rangedAttackChance = 0.3f;
        
        protected override void Awake()
        {
            base.Awake();
            
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
                config.attackCooldown = 2f;
                config.rangedAttackCooldown = 3f;
                config.attackDamage = 20f;
                config.canBeStomped = false;
            }
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
            
            if (projectilePrefab == null || target == null) return;
            
            Vector3 spawnPosition = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            Vector2 direction = (target.position - spawnPosition).normalized;
            
            var rb2d = projectile.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = direction * projectileSpeed;
            }
            
            var projectileScript = projectile.GetComponent<EnemyProjectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(config.attackDamage);
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
    }
}