using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    [Header("Kill Count Settings")]
    [Tooltip("The number of kills required for each object.")]
    public int[] requiredKills;

    [Header("References")]
    [Tooltip("The objects to activate after reaching the required kills.")]
    public GameObject[] targetObjects;

    [Header("Layer Settings")]
    [Tooltip("Layer to exclude from kill count (e.g., Slave layer).")]
    public LayerMask slaveLayer;

    [Header("Debugging")]
    [Tooltip("Tracks the number of enemies killed.")]
    public int enemiesKilled;

    private int currentKillThresholdIndex = 0;

    private void Start()
    {
        // Ensure all target objects are inactive at the start
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    public void EnemyKilled(GameObject enemy)
    {
        // Check if the enemy is on the "Slave" layer
        if ((slaveLayer.value & (1 << enemy.layer)) != 0)
        {
            // Enemy is a "Slave", do not increase the count
            return;
        }

        // Increase kill count
        enemiesKilled++;

        // Check if the current threshold is reached
        if (currentKillThresholdIndex < requiredKills.Length &&
            enemiesKilled >= requiredKills[currentKillThresholdIndex])
        {
            // Activate the corresponding object
            if (targetObjects[currentKillThresholdIndex] != null)
            {
                targetObjects[currentKillThresholdIndex].SetActive(true);
            }

            // Move to the next threshold
            currentKillThresholdIndex++;
        }
    }
}
