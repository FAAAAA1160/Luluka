using UnityEngine;

namespace LULUKA
{
    public class LaserController : MonoBehaviour
    {
        [Header("渐隐设置")]
        [SerializeField] private float duration = 1f;
        [SerializeField] private float fadeSpeed = 2f;
        
        [Header("伤害设置")]
        [SerializeField] private float damage = 25f;
        [SerializeField] private bool damageOnce = true;
        
        private SpriteRenderer spriteRenderer;
        private float timer;
        private bool isFading;
        private bool hasDamaged;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            timer = duration;
            hasDamaged = false;
        }
        
        private void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else if (!isFading)
            {
                isFading = true;
            }
            
            if (isFading && spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a -= fadeSpeed * Time.deltaTime;
                spriteRenderer.color = color;
                
                if (color.a <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (damageOnce && hasDamaged) return;
                
                var damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                    hasDamaged = true;
                }
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (damageOnce) return;
            
            if (other.CompareTag("Enemy"))
            {
                var damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage * Time.deltaTime);
                }
            }
        }
        
        public void Initialize(float laserDuration, float laserFadeSpeed)
        {
            duration = laserDuration;
            fadeSpeed = laserFadeSpeed;
            timer = duration;
        }
        
        public void SetRotation(bool facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, facingRight ? -90f : 90f);
        }
        
        public void SetDamage(float laserDamage)
        {
            damage = laserDamage;
        }
    }
}