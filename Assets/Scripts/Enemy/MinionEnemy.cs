using UnityEngine;

namespace LULUKA
{
    public class MinionEnemy : EnemyBase
    {
        [Header("小怪特有设置")]
        [SerializeField] private float stompBounceForce = 5f;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EnemyConfig>();
                config.maxHealth = 50f;
                config.moveSpeed = 2f;
                config.patrolSpeed = 1f;
                config.detectionRange = 5f;
                config.attackRange = 1f;
                config.patrolRange = 3f;
                config.attackCooldown = 2f;
                config.attackDamage = 10f;
                config.canBeStomped = true;
                config.stompDamage = 50f;
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
        
        public void OnStomped(GameObject player)
        {
            if (!CanBeStomped) return;
            
            TakeStompDamage(config.stompDamage);
            
            var playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ContactPoint2D contact = collision.contacts[0];
                
                if (contact.normal.y < -0.5f)
                {
                    OnStomped(collision.gameObject);
                }
                else
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
}