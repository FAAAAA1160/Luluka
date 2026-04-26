using UnityEngine;

namespace LULUKA
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathZone : MonoBehaviour
    {
        private void Awake()
        {
            var collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(float.MaxValue);
                }
            }
        }
    }
}
