using UnityEngine;

namespace LULUKA
{
    public class ChargeEffectController : MonoBehaviour
    {
        [Header("魔法阵设置")]
        [SerializeField] private SpriteRenderer magicCircle;
        [SerializeField] private float circleFadeSpeed = 2f;
        [SerializeField] private float circleRotateSpeed = 30f;
        
        [Header("星星设置")]
        [SerializeField] private SpriteRenderer[] stars = new SpriteRenderer[7];
        [SerializeField] private float starFadeSpeed = 3f;
        [SerializeField] private float chargeTimePerStar = 0.3f;
        
        [Header("激光设置")]
        [SerializeField] private GameObject laserPrefab;
        [SerializeField] private float laserDuration = 1f;
        [SerializeField] private float laserFadeSpeed = 2f;
        
        private float currentChargeTime;
        private int currentStarIndex;
        private bool isCharging;
        private bool isFullyCharged;
        private bool characterFacingRight = true;
        
        private void Awake()
        {
            InitializeEffect();
        }
        
        private void InitializeEffect()
        {
            if (magicCircle != null)
            {
                Color color = magicCircle.color;
                color.a = 0f;
                magicCircle.color = color;
            }
            
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    Color color = stars[i].color;
                    color.a = 0f;
                    stars[i].color = color;
                }
            }
            
            gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (!isCharging) return;
            
            UpdateMagicCircleEffect();
            UpdateStarsEffect();
        }
        
        private void UpdateMagicCircleEffect()
        {
            if (magicCircle == null) return;
            
            if (magicCircle.color.a < 1f)
            {
                Color color = magicCircle.color;
                color.a = Mathf.Min(color.a + circleFadeSpeed * Time.deltaTime, 1f);
                magicCircle.color = color;
            }
            
            magicCircle.transform.Rotate(0f, 0f, circleRotateSpeed * Time.deltaTime);
        }
        
        private void UpdateStarsEffect()
        {
            if (currentStarIndex >= stars.Length) return;
            
            currentChargeTime += Time.deltaTime;
            
            if (currentChargeTime >= chargeTimePerStar)
            {
                currentChargeTime = 0f;
                currentStarIndex++;
                
                if (currentStarIndex >= stars.Length)
                {
                    isFullyCharged = true;
                }
            }
            
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null) continue;
                
                float targetAlpha = i < currentStarIndex ? 1f : (i == currentStarIndex ? currentChargeTime / chargeTimePerStar : 0f);
                Color color = stars[i].color;
                color.a = Mathf.Lerp(color.a, targetAlpha, starFadeSpeed * Time.deltaTime);
                stars[i].color = color;
            }
        }
        
        public void StartCharge()
        {
            gameObject.SetActive(true);
            isCharging = true;
            isFullyCharged = false;
            currentChargeTime = 0f;
            currentStarIndex = 0;
            
            if (magicCircle != null)
            {
                Color color = magicCircle.color;
                color.a = 0f;
                magicCircle.color = color;
            }
            
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    Color color = stars[i].color;
                    color.a = 0f;
                    stars[i].color = color;
                }
            }
        }
        
        public void EndCharge()
        {
            isCharging = false;
            SpawnLasers();
            gameObject.SetActive(false);
        }
        
        private void SpawnLasers()
        {
            if (laserPrefab == null) return;
            
            int visibleStarCount = currentStarIndex;
            
            for (int i = 0; i < visibleStarCount; i++)
            {
                if (stars[i] == null) continue;
                
                GameObject laser = Instantiate(laserPrefab, stars[i].transform.position, Quaternion.identity);
                
                LaserController laserController = laser.GetComponent<LaserController>();
                if (laserController == null)
                {
                    laserController = laser.AddComponent<LaserController>();
                }
                
                laserController.Initialize(laserDuration, laserFadeSpeed);
                laserController.SetRotation(characterFacingRight);
            }
        }
        
        public void SetCharacterFacing(bool facingRight)
        {
            characterFacingRight = facingRight;
        }
        
        public bool IsFullyCharged => isFullyCharged;
        public float ChargeProgress => (float)currentStarIndex / stars.Length;
        public int VisibleStarCount => currentStarIndex;
    }
}