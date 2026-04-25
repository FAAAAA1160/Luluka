using UnityEngine;

namespace LULUKA
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "LULUKA/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("基础属性")]
        public float maxHealth = 100f;
        public float moveSpeed = 2f;
        public float patrolSpeed = 1f;
        
        [Header("检测范围")]
        public float detectionRange = 5f;
        public float attackRange = 1f;
        public float patrolRange = 3f;
        
        [Header("攻击设置")]
        public float attackCooldown = 2f;
        public float attackDamage = 10f;
        
        [Header("远程攻击(BOSS)")]
        public float rangedAttackRange = 5f;
        public float rangedAttackCooldown = 3f;
        public GameObject projectilePrefab;
        
        [Header("踩踏设置")]
        public bool canBeStomped = true;
        public float stompDamage = 50f;
        
        [Header("死亡设置")]
        public float deathDestroyTime = 1f;
        
        [Header("掉落")]
        public int scoreValue = 100;
    }
}