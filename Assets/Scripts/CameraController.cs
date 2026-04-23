using UnityEngine;

namespace LULUKA
{
    public class CameraController : MonoBehaviour
    {
        [Header("跟随目标")]
        [SerializeField] private Transform target;
        
        [Header("跟随参数")]
        [SerializeField] private float followSpeedX = 2f;
        [SerializeField] private float followSpeedY = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        
        [Header("缓动设置")]
        [SerializeField] private float deadZoneX = 0.5f;
        [SerializeField] private float deadZoneY = 0.5f;
        [SerializeField] private float minFollowSpeed = 0.5f;
        [SerializeField] private float maxFollowSpeed = 10f;
        
        [Header("背景移动")]
        [SerializeField] private bool enableBackgroundMovement = false;
        [SerializeField] private float backgroundMoveSpeed = 0.5f;
        [SerializeField] private Transform backgroundContainer;
        
        private Vector3 lastTargetPosition;
        
        private void Start()
        {
            if (target != null)
            {
                lastTargetPosition = target.position;
                transform.position = target.position + offset;
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            Vector3 targetPosition = target.position + offset;
            Vector3 currentPosition = transform.position;
            
            float distanceX = targetPosition.x - currentPosition.x;
            float distanceY = targetPosition.y - currentPosition.y;
            
            float speedX = CalculateFollowSpeed(Mathf.Abs(distanceX), deadZoneX, followSpeedX);
            float speedY = CalculateFollowSpeed(Mathf.Abs(distanceY), deadZoneY, followSpeedY);
            
            float newX = Mathf.Lerp(currentPosition.x, targetPosition.x, speedX * Time.deltaTime);
            float newY = Mathf.Lerp(currentPosition.y, targetPosition.y, speedY * Time.deltaTime);
            
            transform.position = new Vector3(newX, newY, targetPosition.z);
            
            if (enableBackgroundMovement)
            {
                MoveBackgrounds();
            }
        }
        
        private float CalculateFollowSpeed(float distance, float deadZone, float baseSpeed)
        {
            if (distance <= deadZone)
            {
                return 0f;
            }
            
            float speed = baseSpeed * (distance - deadZone) / (1f + distance);
            return Mathf.Clamp(speed, minFollowSpeed, maxFollowSpeed);
        }
        
        private void MoveBackgrounds()
        {
            if (backgroundContainer == null) return;
            
            Vector3 deltaPosition = target.position - lastTargetPosition;
            
            foreach (Transform background in backgroundContainer)
            {
                if (background != null)
                {
                    Vector3 moveAmount = -deltaPosition * backgroundMoveSpeed;
                    background.position += moveAmount;
                }
            }
            
            lastTargetPosition = target.position;
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                lastTargetPosition = target.position;
            }
        }
        
        public void SetBackgroundMoveSpeed(float speed)
        {
            backgroundMoveSpeed = speed;
        }
        
        public void EnableBackgroundMovement(bool enable)
        {
            enableBackgroundMovement = enable;
        }
        
        public Transform GetBackgroundContainer()
        {
            return backgroundContainer;
        }
    }
}