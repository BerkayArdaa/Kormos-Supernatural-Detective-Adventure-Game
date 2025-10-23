using UnityEngine;

public class NPCDestroyer : MonoBehaviour
{
    public GameObject targetObject;
    public float moveDistance = -40f;
    public float moveSpeed = 5f;
    private bool startMoving = false;
    private Vector3 targetPosition;

    void Update()
    {
        if (!startMoving && targetObject == null)
        {
            TriggerMove();
        }

        if (startMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void AssignTarget(GameObject newTarget)
    {
        targetObject = newTarget;
    }

    void TriggerMove()
    {
        targetPosition = transform.position + new Vector3(0, moveDistance, 0);
        startMoving = true;
    }
}
