using UnityEngine;

namespace LULUKA
{
    public class SteelRollController : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotationSpeed = 200f;
        
        [Header("伤害设置")]
        [SerializeField] private float damage = 15f;
        
        [Header("生命周期")]
        [SerializeField] private float lifetime = 5f;
        
        private float moveDirection = 1f;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
        
        private void Update()
        {
            transform.Rotate(0f, 0f, -rotationSpeed * moveDirection * Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
            }
        }
        
        public void Initialize(float direction)
        {
            moveDirection = direction;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}