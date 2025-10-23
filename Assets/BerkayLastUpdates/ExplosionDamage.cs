using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public float damage = 20f; // Damage dealt by the explosion
    public float explosionDuration = 1f; // How long the explosion lasts before disappearing

    private void Start()
    {
        // Destroy the explosion object after the duration
        Destroy(gameObject, explosionDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has a PlayerStats component
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            // Deal damage to the player
            playerStats.TakeDamage(damage);
        }
    }
}
