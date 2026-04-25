using UnityEngine;

namespace LULUKA
{
    public class DeathState : EnemyState
    {
        private float deathTimer;
        
        public DeathState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            deathTimer = 0f;
            enemy.StopMovement();
            enemy.DisableCollider();
            enemy.DisablePhysics();
            
            if (animator != null)
            {
                animator.SetTrigger(deathHash);
            }
        }
        
        public override void Update()
        {
            deathTimer += Time.deltaTime;
            
            if (deathTimer >= enemy.Config.deathDestroyTime)
            {
                enemy.OnDeathComplete();
            }
        }
    }
}