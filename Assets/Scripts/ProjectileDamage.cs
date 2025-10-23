using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 20f;
    public float destroyDelay = 2f;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStats player = collision.collider.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        if (!collision.collider.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
