using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxMana = 100f;
    public float currentMana;
    public float healAmount = 50f;
    public float healRange = 2f;
    public LayerMask bonfireLayer;
    private bool isNearBonfire;
    private bool isResettingMana = false;
    public Slider healthSlider;
    public Slider manaSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI cloneText;
    public TextMeshProUGUI deathNote;

    public TPSController tpsController;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = 0;
        UpdateHealthSlider();
        UpdateManaSlider();
        UpdateCloneText();
        StartCoroutine(ManaDecayRoutine());
    }

    private void Update()
    {
        CheckBonfireProximity();
        if (isNearBonfire && Input.GetKey(KeyCode.H))
        {
            HealPlayer();
            if (!isResettingMana)
            {
                StartCoroutine(BonfireResetMana());
            }
        }
        UpdateHealthSlider();
        UpdateManaSlider();
        UpdateCloneText();
    }

    private IEnumerator ManaDecayRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            DecreaseMana(10f);
        }
    }

    private void DecreaseMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
        StartCoroutine(SmoothManaUpdate());
    }

    public bool CanUseAbility(float manaCost)
    {
        return currentMana + manaCost <= maxMana;
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        StartCoroutine(SmoothManaUpdate());
    }

    private void HealPlayer()
    {
        currentHealth += healAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private IEnumerator BonfireResetMana()
    {
        isResettingMana = true;
        float resetDuration = 2f;
        float initialMana = currentMana;
        float elapsed = 0f;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / resetDuration;
            currentMana = Mathf.Lerp(initialMana, 0f, t);
            yield return null;
        }

        currentMana = 0f;
        isResettingMana = false;
        StartCoroutine(SmoothManaUpdate());
    }

    private void CheckBonfireProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, healRange, bonfireLayer);
        isNearBonfire = colliders.Length > 0;
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth / maxHealth, Time.deltaTime * 10f);
        }
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString("F0") + "/" + maxHealth;
        }
    }

    private void UpdateManaSlider()
    {
        if (manaSlider != null)
        {
            manaSlider.value = Mathf.Lerp(manaSlider.value, currentMana / maxMana, Time.deltaTime * 10f);
        }
        if (manaText != null)
        {
            manaText.text = currentMana.ToString("F0") + "/" + maxMana;
        }
    }

    private void UpdateCloneText()
    {
        if (tpsController.maxCloneCount<1)
        {
            cloneText.text = "??/??";
        } 
        else if (cloneText != null && tpsController != null)
        {
            cloneText.text = tpsController.currentCloneCount.ToString("F0") + "/" + tpsController.maxCloneCount.ToString("F0");
        }
    }

    private IEnumerator SmoothManaUpdate()
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        float startValue = manaSlider.value;
        float targetValue = currentMana / maxMana;
        while (elapsedTime < duration)
        {
            manaSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        manaSlider.value = targetValue;
    }

    public IEnumerator ResetManaToZero()
    {
        float startMana = currentMana;
        float duration = 0.25f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            currentMana = Mathf.Lerp(startMana, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentMana = 0f;
        StartCoroutine(SmoothManaUpdate());
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        healthText.text = "";
        deathNote.text = "!!!Git Gud!!!";
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}