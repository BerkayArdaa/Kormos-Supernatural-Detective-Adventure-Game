using UnityEngine;

public class ActivateOnDestroy : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject watchedObject;

    void Update()
    {
        if (watchedObject == null)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }
            this.enabled = false;
        }
    }
}
