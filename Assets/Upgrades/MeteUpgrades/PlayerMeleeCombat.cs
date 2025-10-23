using System.Collections;
using UnityEngine;

public class PlayerMeleeCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldown = 1.0f;
    public float attackRange = 1.5f;
    public int attackDamage = 25;

    [Header("References")]
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private float nextAttackTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        nextAttackTime = Time.time + attackCooldown;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
           // enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
