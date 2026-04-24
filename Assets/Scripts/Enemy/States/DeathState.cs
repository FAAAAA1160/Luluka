using UnityEngine;

namespace LULUKA
{
    public class DeathState : EnemyState
    {
        private float deathTimer;
        private float deathDuration = 1f;
        
        public DeathState(EnemyBase enemy) : base(enemy) { }
        
        public override void Enter()
        {
            enemy.StopMovement();
            enemy.DisableCollider();
            
            if (animator != null)
            {
                animator.SetTrigger(deathHash);
            }
            
            deathTimer = 0f;
            
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name.Contains("死亡") || clip.name.Contains("Death"))
                {
                    deathDuration = clip.length;
                    break;
                }
            }
        }
        
        public override void Update()
        {
            deathTimer += Time.deltaTime;
            
            if (deathTimer >= deathDuration)
            {
                enemy.OnDeathComplete();
            }
        }
    }
}