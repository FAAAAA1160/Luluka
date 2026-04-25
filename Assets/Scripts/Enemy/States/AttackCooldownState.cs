using UnityEngine;

namespace LULUKA
{
    public class AttackCooldownState : EnemyState
    {
        private float cooldownTimer;
        
        public AttackCooldownState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            cooldownTimer = 0f;
            enemy.StopMovement();
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 0f);
            }
        }
        
        public override void Update()
        {
            cooldownTimer += Time.deltaTime;
            
            if (cooldownTimer >= enemy.Config.attackCooldown)
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