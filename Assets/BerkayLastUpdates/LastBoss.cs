using UnityEngine;

public class LastBoss: MonoBehaviour
{
    public Transform player; // Reference to the player's Transform

    void Update()
    {
        // Calculate the direction towards the player
        Vector3 direction = player.position - transform.position;

        // Ignore the vertical axis
        direction.y = 0;

        // Rotate towards the player only if there's a valid direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
