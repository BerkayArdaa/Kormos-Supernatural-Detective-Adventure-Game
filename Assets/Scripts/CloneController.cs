using UnityEngine;
using System.Collections;

public class CloneController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private Transform vfxHitGreen;

    private TPSController tpsController;
    private bool cloneShooting;
    private int shotsFired;

    private void Start()
    {
        cloneShooting = false;
        shotsFired = 0;
        tpsController = FindObjectOfType<TPSController>();
        StartCoroutine(IdleBeforeShooting());
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private IEnumerator IdleBeforeShooting()
    {
        animator.SetBool("cloneShooting", false);
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShootingRoutine());
    }

    private IEnumerator ShootingRoutine()
    {
        cloneShooting = true;
        animator.SetBool("cloneShooting", cloneShooting);

        while (shotsFired < 16 && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }

            yield return new WaitForSeconds(0.5f);
            PerformHitscan();
            shotsFired++;

            yield return new WaitForSeconds(0.5f);
        }

        cloneShooting = false;
        StartCoroutine(IdleAfterShooting());
    }

    private void PerformHitscan()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - firePoint.position).normalized;
            Ray ray = new Ray(firePoint.position, directionToTarget);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform hitTransform = hit.transform;
                if (hitTransform.GetComponent<BulletTarget>() != null)
                {
                    Instantiate(vfxHitRed, hit.point, Quaternion.identity);
                }
                else
                {
                    Instantiate(vfxHitGreen, hit.point, Quaternion.identity);
                }

                Target targetComponent = hit.collider.GetComponent<Target>();
                if (targetComponent != null)
                {
                    targetComponent.TakeDamage(20);
                }
            }
        }else
        {
            StartCoroutine(IdleAfterShooting());
        }
    }

    private IEnumerator IdleAfterShooting()
    {
        animator.SetBool("cloneShooting", false);
        yield return new WaitForSeconds(1f);

        if (tpsController != null)
        {
            tpsController.OnCloneDestroyed();
        }

        Destroy(gameObject);
    }
}
