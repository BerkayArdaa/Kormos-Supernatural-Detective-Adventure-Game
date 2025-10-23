using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private GameObject teleportDestination;
    [SerializeField] private string playerTag = "Player";

    private Vector3 startPosition;
    private bool movingForward = true;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float currentZ = transform.position.z;

        if (movingForward && currentZ >= startPosition.z + moveDistance)
        {
            movingForward = false;
        }
        else if (!movingForward && currentZ <= startPosition.z - moveDistance)
        {
            movingForward = true;
        }

        float direction = movingForward ? 1 : -1;
        transform.Translate(Vector3.forward * direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (teleportDestination != null)
            {
                CharacterController controller = other.GetComponent<CharacterController>();
                Rigidbody rb = other.GetComponent<Rigidbody>();

                if (controller != null)
                {
                    controller.enabled = false;
                }

                other.transform.position = teleportDestination.transform.position;

                if (controller != null)
                {
                    controller.enabled = true;
                }

                if (rb != null)
                {
                    rb.position = teleportDestination.transform.position;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("Teleport Destination not set in the inspector!");
            }
        }
    }
}