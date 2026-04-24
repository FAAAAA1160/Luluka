using UnityEngine;

namespace LULUKA
{
    public abstract class EnemyState
    {
        protected EnemyBase enemy;
        protected Animator animator;
        
        protected static readonly int speedHash = Animator.StringToHash("Speed");
        protected static readonly int attackHash = Animator.StringToHash("Attack");
        protected static readonly int rangedAttackHash = Animator.StringToHash("RangedAttack");
        protected static readonly int deathHash = Animator.StringToHash("Death");
        protected static readonly int hitHash = Animator.StringToHash("Hit");
        
        public EnemyState(EnemyBase enemy)
        {
            this.enemy = enemy;
            this.animator = enemy.Animator;
        }
        
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}