using UnityEngine;

namespace LULUKA
{
    public class AttackState : EnemyState
    {
        private float attackTimer;
        private bool hasAttacked;
        private bool hasDashed;
        private float animationDuration = 0.5f;
        private float dashSpeed = 8f;
        private float dashStartTime = 0.15f;
        private int dashDirection;
        
        public AttackState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            attackTimer = 0f;
            hasAttacked = false;
            hasDashed = false;
            enemy.StopMovement();
            
            if (enemy.Target != null)
            {
                dashDirection = enemy.Target.position.x > enemy.transform.position.x ? 1 : -1;
            }
            else
            {
                dashDirection = enemy.IsFacingRight ? 1 : -1;
            }
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 0f);
                animator.SetTrigger(attackHash);
            }
        }
        
        public override void Update()
        {
            attackTimer += Time.deltaTime;
            
            if (!hasDashed && attackTimer >= dashStartTime)
            {
                hasDashed = true;
                
                if (enemy is BossEnemy)
                {
                    var rb = enemy.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
                    }
                }
            }
            
            if (!hasAttacked && attackTimer >= 0.3f)
            {
                hasAttacked = true;
                enemy.PerformAttack();
            }
            
            if (attackTimer >= animationDuration)
            {
                if (enemy.IsPlayerInRange(enemy.Config.detectionRange))
                {
                    if (enemy.IsPlayerInRange(enemy.Config.attackRange))
                    {
                        enemy.ChangeState(new AttackCooldownState(enemy));
                    }
                    else
                    {
                        enemy.ChangeState(new ChaseState(enemy));
                    }
                }
                else
                {
                    enemy.ChangeState(new PatrolState(enemy));
                }
            }
        }
    }
}