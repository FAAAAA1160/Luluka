using UnityEngine;

namespace LULUKA
{
    public class ChaseState : EnemyState
    {
        private Transform target;
        private float rangedAttackCheckTimer;
        private float rangedAttackCheckInterval = 0.5f;
        
        public ChaseState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            target = enemy.Target;
            rangedAttackCheckTimer = 0f;
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 1f);
            }
        }
        
        public override void Update()
        {
            if (target == null || !enemy.IsPlayerInRange(enemy.Config.detectionRange))
            {
                enemy.ChangeState(new PatrolState(enemy));
                return;
            }
            
            float distanceToTarget = Vector2.Distance(enemy.transform.position, target.position);
            
            if (distanceToTarget <= enemy.Config.attackRange)
            {
                enemy.ChangeState(new AttackState(enemy));
                return;
            }
            
            if (enemy is BossEnemy boss)
            {
                rangedAttackCheckTimer += Time.deltaTime;
                
                if (distanceToTarget <= enemy.Config.rangedAttackRange && 
                    distanceToTarget > enemy.Config.attackRange)
                {
                    if (rangedAttackCheckTimer >= rangedAttackCheckInterval)
                    {
                        rangedAttackCheckTimer = 0f;
                        
                        if (boss.CanRangedAttack())
                        {
                            enemy.ChangeState(new RangedAttackState(enemy));
                            return;
                        }
                    }
                }
            }
        }
        
        public override void FixedUpdate()
        {
            if (target == null) return;
            
            float direction = target.position.x > enemy.transform.position.x ? 1f : -1f;
            enemy.Flip(direction > 0f);
            enemy.Move(direction * enemy.Config.moveSpeed);
        }
    }
}