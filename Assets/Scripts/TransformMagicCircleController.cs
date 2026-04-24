using UnityEngine;

namespace LULUKA
{
    public class TransformMagicCircleController : MonoBehaviour
    {
        [Header("渐显渐隐设置")]
        [SerializeField] private float fadeSpeed = 2f;
        
        [Header("旋转设置")]
        [SerializeField] private float rotateSpeed = 60f;
        
        private SpriteRenderer spriteRenderer;
        private bool isTransforming;
        private bool isFadingOut;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            InitializeAlpha(0f);
            gameObject.SetActive(false);
        }
        
        private void InitializeAlpha(float alpha)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
        
        private void Update()
        {
            if (!isTransforming) return;
            
            if (isFadingOut)
            {
                UpdateFadeOut();
            }
            else
            {
                UpdateFadeInAndRotate();
            }
        }
        
        private void UpdateFadeOut()
        {
            float currentAlpha = spriteRenderer.color.a;
            currentAlpha -= fadeSpeed * Time.deltaTime;
            
            if (currentAlpha <= 0f)
            {
                InitializeAlpha(0f);
                isTransforming = false;
                isFadingOut = false;
                gameObject.SetActive(false);
            }
            else
            {
                InitializeAlpha(currentAlpha);
            }
        }
        
        private void UpdateFadeInAndRotate()
        {
            float currentAlpha = spriteRenderer.color.a;
            if (currentAlpha < 1f)
            {
                currentAlpha = Mathf.Min(currentAlpha + fadeSpeed * Time.deltaTime, 1f);
                InitializeAlpha(currentAlpha);
            }
            
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
        
        public void StartTransform()
        {
            gameObject.SetActive(true);
            isTransforming = true;
            isFadingOut = false;
            InitializeAlpha(0f);
        }
        
        public void EndTransform()
        {
            isFadingOut = true;
        }
        
        public bool IsTransforming => isTransforming;
    }
}