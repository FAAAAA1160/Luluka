using UnityEngine;

namespace LULUKA
{
    public class LaserController : MonoBehaviour
    {
        [Header("渐隐设置")]
        [SerializeField] private float duration = 1f;
        [SerializeField] private float fadeSpeed = 2f;
        
        private SpriteRenderer spriteRenderer;
        private float timer;
        private bool isFading;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            timer = duration;
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
    }
}