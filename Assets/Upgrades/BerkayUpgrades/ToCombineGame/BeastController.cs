using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BeastController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsPlayer;
    public Animator animator;

    public float sightRange;
    public float minimumChaseDistance = 2f;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    public float damage = 5f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public bool isStunned = false;
    private Coroutine stunCoroutine;


    private void Awake()
    {
        player = GameObject.Find("PlayerCharacter").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {       
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        playerInAttackRange = IsWithinMinimumDistance();
        if (isStunned) {
            StunTimer(10f);

        }
        else
        {
            if (playerInSightRange)
            {
                FacePlayer();

                if (playerInAttackRange)
                {

                    AttackPlayer();
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
        }
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !playerInAttackRange);
    }

    private void ChasePlayer()
    {
        animator.SetBool("isAttack", false);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {

        animator.SetBool("isAttack", true);
        agent.SetDestination(transform.position);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

            if (dotProduct > 0.5f)
            {
                DealDamage();
            }
        }
    }

    private void DealDamage()
    {
        PlayerStats playerHealth = player.GetComponent<PlayerStats>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private bool IsWithinMinimumDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= minimumChaseDistance;
    }

    private void Wander()
    {
        animator.SetBool("isAttack", false);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 randomDirection = Random.insideUnitSphere * sightRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, sightRange, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minimumChaseDistance);
    }
    public void StunTimer(float duration)
    {

        if (isStunned == true)
        { 
            stunCoroutine = StartCoroutine(StunCoroutine(duration));
        }
       
    }

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("I WANNA BREAK FREEE");
        isStunned = false;
    }
   
}
