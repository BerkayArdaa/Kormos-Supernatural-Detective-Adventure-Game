using UnityEngine;

public class FloatingLookAtPlayer : MonoBehaviour
{
    public Transform player;  // Reference to the player's Transform
    public float floatAmplitude = 0.5f;  // How far the object moves up and down
    public float floatSpeed = 2f;  // Speed of the floating movement

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Floating up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Look at the player only on the Y-axis
        Vector3 direction = player.position - transform.position;
        direction.y = 0;  // Ignore the vertical axis
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
