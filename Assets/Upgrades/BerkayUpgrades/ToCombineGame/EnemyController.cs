using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] wanderPoints;
    private Transform currentWanderPoint;
    private int lastWanderIndex = -1;
    public float waitAtPointTime = 2f;
    private bool isWaiting = false;
    private bool isAttacking = false;
    public Transform PlayerTarget;


    public NavMeshAgent agent;
    public Transform player;
  
    public LayerMask whatIsPlayer;
    public Animator animator;

    public float sightRange;
    private bool playerInSightRange;
    public float throwRange;
    private bool playerInThrowRange;

    public GameObject projectilePrefab;
    public Transform throwPoint;
    public float throwCooldown = 2f;
    public float throwForce = 20f;
    private bool readyToThrow = true;

    private void Awake()
    {
        player = GameObject.Find("PlayerCharacter").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInThrowRange = Physics.CheckSphere(transform.position, throwRange, whatIsPlayer);

        if (playerInSightRange)
        {
            FacePlayer();

            if (playerInThrowRange)
            {
                StopChasingPlayer();

                if (readyToThrow)
                {
                    animator.Play("mixamo_com", 0, 0);
                    readyToThrow = false;
                    Invoke(nameof(ThrowAtPlayer), 0.5f);
                }
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Wander();
        }
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isWaiting);
    }


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void StopChasingPlayer()
    {
        agent.SetDestination(transform.position);
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void ThrowAtPlayer()
    {
        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        Vector3 direction = (PlayerTarget.position - throwPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().AddForce(direction * throwForce, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCooldown);
    }


    private void ResetThrow()
    {
        readyToThrow = true;
    }

    private void Wander()
    {
        if (isWaiting) return;

        if (currentWanderPoint != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtPoint());
            return;
        }

        if (currentWanderPoint == null || agent.remainingDistance <= agent.stoppingDistance)
        {
            SelectNewWanderPoint();
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitAtPointTime);
        isWaiting = false;
        SelectNewWanderPoint();
    }

    private void SelectNewWanderPoint()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, wanderPoints.Length);
        } while (randomIndex == lastWanderIndex);

        currentWanderPoint = wanderPoints[randomIndex];
        lastWanderIndex = randomIndex;

        agent.SetDestination(currentWanderPoint.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, throwRange);
    }
}