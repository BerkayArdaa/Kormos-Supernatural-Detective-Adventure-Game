using UnityEngine;
public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TPSController tpsController = other.GetComponent<TPSController>();
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (tpsController != null && playerStats != null)
            {
                tpsController.CollectCollectible();

                playerStats.currentHealth = playerStats.maxHealth;
                playerStats.currentMana = 0;
                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
                }

                Destroy(gameObject);
            }
        }
    }
}