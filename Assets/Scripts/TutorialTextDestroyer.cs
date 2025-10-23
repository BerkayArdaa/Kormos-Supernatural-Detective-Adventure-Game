using UnityEngine;

public class TutorialTextDestroyer : MonoBehaviour
{
    [SerializeField] private string targetTag = "TutorialText";
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            GameObject[] tutorialTexts = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject text in tutorialTexts)
            {
                Destroy(text);
            }
        }
    }
}