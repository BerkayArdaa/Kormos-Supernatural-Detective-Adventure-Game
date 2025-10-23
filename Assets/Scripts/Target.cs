using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (transform.parent != null)
        {
            GameObject.FindObjectOfType<EnemyKillCounter>().EnemyKilled(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
        else
        {
            GameObject.FindObjectOfType<EnemyKillCounter>().EnemyKilled(gameObject);
            Destroy(gameObject);
        }
    }
}
