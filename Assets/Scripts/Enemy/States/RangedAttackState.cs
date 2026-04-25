using UnityEngine;

namespace LULUKA
{
    public class RangedAttackState : EnemyState
    {
        private float attackTimer;
        private bool hasAttacked;
        private float animationDuration = 0.8f;
        
        public RangedAttackState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            attackTimer = 0f;
            hasAttacked = false;
            enemy.StopMovement();
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 0f);
                animator.SetTrigger(rangedAttackHash);
            }
        }
        
        public override void Update()
        {
            attackTimer += Time.deltaTime;
            
            if (!hasAttacked && attackTimer >= 0.3f)
            {
                hasAttacked = true;
                
                if (enemy is BossEnemy boss)
                {
                    boss.PerformRangedAttack();
                }
            }
            
            if (attackTimer >= animationDuration)
            {
                if (enemy.IsPlayerInRange(enemy.Config.detectionRange))
                {
                    float distance = Vector2.Distance(enemy.transform.position, enemy.Target.position);
                    
                    if (distance <= enemy.Config.attackRange)
                    {
                        enemy.ChangeState(new AttackState(enemy));
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