using UnityEngine;

namespace LULUKA
{
    public class PatrolState : EnemyState
    {
        private Vector2 startPosition;
        private float patrolDirection = 1f;
        private float waitTimer;
        private bool isWaiting;
        
        public PatrolState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            startPosition = enemy.transform.position;
            isWaiting = false;
            waitTimer = 0f;
            
            if (animator != null)
            {
                animator.SetFloat(speedHash, 0.5f);
            }
        }
        
        public override void Update()
        {
            if (enemy.IsPlayerInRange(enemy.Config.detectionRange))
            {
                enemy.ChangeState(new ChaseState(enemy));
                return;
            }
            
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    patrolDirection *= -1f;
                    enemy.Flip(patrolDirection > 0f);
                }
                return;
            }
            
            float distanceFromStart = Mathf.Abs(enemy.transform.position.x - startPosition.x);
            
            if (distanceFromStart >= enemy.Config.patrolRange)
            {
                isWaiting = true;
                waitTimer = Random.Range(1f, 3f);
                
                if (animator != null)
                {
                    animator.SetFloat(speedHash, 0f);
                }
            }
        }
        
        public override void FixedUpdate()
        {
            if (isWaiting) return;
            
            enemy.Move(patrolDirection * enemy.Config.patrolSpeed);
        }
    }
}