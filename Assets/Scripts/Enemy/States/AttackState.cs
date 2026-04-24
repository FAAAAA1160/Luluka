using UnityEngine;

namespace LULUKA
{
    public class AttackState : EnemyState
    {
        private float attackTimer;
        private bool hasAttacked;
        
        public AttackState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            attackTimer = 0f;
            hasAttacked = false;
            enemy.StopMovement();
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 0f);
                animator.SetTrigger(attackHash);
            }
        }
        
        public override void Update()
        {
            attackTimer += Time.deltaTime;
            
            if (!hasAttacked && attackTimer >= 0.3f)
            {
                hasAttacked = true;
                enemy.PerformAttack();
            }
            
            if (attackTimer >= enemy.Config.attackCooldown)
            {
                if (enemy.IsPlayerInRange(enemy.Config.detectionRange))
                {
                    if (enemy.IsPlayerInRange(enemy.Config.attackRange))
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