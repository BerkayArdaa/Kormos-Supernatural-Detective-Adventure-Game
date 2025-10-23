using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera followCam;
    public CinemachineVirtualCamera aimCam;
    public CinemachineVirtualCamera closedSpaceCam;

    public Transform playerFeet;
    public float rayDistance = 2f;
    public float coneAngle = 15f;
    public int coneRayCount = 8;
    public string closedSpaceTag = "ClosedSpace";

    private void Start()
    {
        followCam.Priority = 10;
        aimCam.Priority = 10;
        closedSpaceCam.Priority = 5;
    }

    private void Update()
    {
        if (CastConeRays())
        {
            SwitchToClosedSpaceCam();
        }
        else
        {
            SwitchToFollowCam();
        }
    }

    private bool CastConeRays()
    {
        Vector3 downward = Vector3.down;

        if (CastSingleRay(downward)) return true;

        for (int i = 0; i < coneRayCount; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = Mathf.Clamp(randomDirection.y, -1f, -0.1f);
            randomDirection = Vector3.Slerp(downward, randomDirection, coneAngle / 90f);

            if (CastSingleRay(randomDirection)) return true;
        }

        return false;
    }

    private bool CastSingleRay(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerFeet.position, direction, out hit, rayDistance))
        {
            if (hit.collider.CompareTag(closedSpaceTag))
            {
                return true;
            }
        }
        return false;
    }

    private void SwitchToClosedSpaceCam()
    {
        followCam.Priority = 5;
        closedSpaceCam.Priority = 10;
    }

    private void SwitchToFollowCam()
    {
        closedSpaceCam.Priority = 5;
        followCam.Priority = 10;
    }
}
