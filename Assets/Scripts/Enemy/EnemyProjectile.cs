using UnityEngine;

namespace LULUKA
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private bool destroyOnHit = true;
        
        private float damage;
        
        public void Initialize(float projectileDamage)
        {
            damage = projectileDamage;
            Destroy(gameObject, lifetime);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<IDamageable>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                
                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}