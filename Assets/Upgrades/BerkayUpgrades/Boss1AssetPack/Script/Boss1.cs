using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss1 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsPlayer;
    public Animator animator;

    public float sightRange;
    public float minimumChaseDistance = 3f;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    public float damage = 15f;
    public float attackCooldown = 3f;
    private float lastAttackTime;

    public float spawnCooldown = 30f;
    private float lastSpawnTime;
    public GameObject minionPrefab;
    public Transform minionParent;

    private void Awake()
    {
        player = GameObject.Find("PlayerCharacter").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = 2f; // Boss'un yürüme hýzýný düþür
    }

    private void Update()
    {
        // Oyuncunun görüþ alanýnda olup olmadýðýný kontrol et
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        // Oyuncunun minimum mesafede olup olmadýðýný kontrol et
        playerInAttackRange = IsWithinMinimumDistance();

        if (playerInSightRange)
        {
            FacePlayer();

            if (playerInAttackRange)
            {
                AttackPlayer(); // Saldýrý baþlat
            }
            else
            {
                ChasePlayer(); // Minimum mesafede deðilse oyuncuyu takip et
            }
        }
        else
        {
            Wander(); // Oyuncu görüþ alanýnda deðilse dolaþ
        }

        // Hareket animasyonu
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !playerInAttackRange);

        // Canavar spawn mekanizmasý
        if (Time.time - lastSpawnTime >= spawnCooldown)
        {
            StartCoroutine(SpawnMinions());
            lastSpawnTime = Time.time;
        }
    }

    private void ChasePlayer()
    {
        animator.SetBool("isAttack", false); // Saldýrýyý durdur
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        animator.SetBool("isAttack", true); // Saldýrýyý baþlat
        agent.SetDestination(transform.position); // Olduðu yerde kal

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Check if the player is in front of the boss
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

            if (dotProduct > 0.5f) // Player is in front of the boss
            {
                DealDamage();
            }
        }
    }

    private void DealDamage()
    {
        // Assuming the player has a script with a method to take damage
        PlayerStats playerHealth = player.GetComponent<PlayerStats>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void FacePlayer()
    {
        // Oyuncuya doðru yönelme
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
        animator.SetBool("isAttack", false); // Saldýrýyý durdur
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Yeni bir nokta seç ve dolaþ
            Vector3 randomDirection = Random.insideUnitSphere * sightRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, sightRange, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }
    }

    private IEnumerator SpawnMinions()
    {
        animator.SetTrigger("SpawnEnemy"); // Trigger spawn animation
        yield return new WaitForSeconds(1.5f); // Wait for animation duration

        // Define spawn distances
        float spawnDistance = 2f;

        // Calculate right and left spawn positions
        Vector3 rightSpawnPosition = transform.position + transform.right * spawnDistance;
        Vector3 leftSpawnPosition = transform.position - transform.right * spawnDistance;

        // Ensure the positions are on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rightSpawnPosition, out hit, spawnDistance, NavMesh.AllAreas))
        {
            GameObject minion = Instantiate(minionPrefab, hit.position, Quaternion.identity);
            if (minionParent != null)
            {
                minion.transform.SetParent(minionParent); // Set the parent of the minion
            }
        }
        if (NavMesh.SamplePosition(leftSpawnPosition, out hit, spawnDistance, NavMesh.AllAreas))
        {
            GameObject minion = Instantiate(minionPrefab, hit.position, Quaternion.identity);
            if (minionParent != null)
            {
                minion.transform.SetParent(minionParent); // Set the parent of the minion
            }
        }

        animator.ResetTrigger("SpawnEnemy"); // Reset spawn trigger
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minimumChaseDistance);
    }
}
