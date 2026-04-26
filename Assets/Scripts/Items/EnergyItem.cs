using UnityEngine;

namespace LULUKA
{
    public class EnergyItem : MonoBehaviour
    {
        [SerializeField] private float energyAmount = 30f;
        [SerializeField] private string playerTag = "Player";
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddEnergy(energyAmount);
                Destroy(gameObject);
            }
        }
    }
}