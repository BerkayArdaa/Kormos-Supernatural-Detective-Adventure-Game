using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossAttack : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>(); // Objeler listesi
    public List<GameObject> effects = new List<GameObject>(); // Efektler için prefab listesi
    public GameObject projectilePrefab; // Fýrlatýlacak obje
    public Transform spawnPoint; // Fýrlatma noktasý

    private float timer = 0f; // Sayaç
    public float interval = 20f; // Rastgele seçim süresi

    void Start()
    {
        // Tüm objeleri pasif hale getir
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // Sayaç zaman ekleme
        timer += Time.deltaTime;

        // Belirlenen süre geçtiyse rastgele 60 obje seç ve her birine obje fýrlat
        if (timer >= interval)
        {
            ActivateRandomObjectsAndShoot();
            timer = 0f; // Sayaç sýfýrla
        }
    }

    void ActivateRandomObjectsAndShoot()
    {
        // Tüm objeleri tekrar pasif hale getir
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // Rastgele seçim için kullanýlmayan indeksleri listele
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < objects.Count; i++)
        {
            availableIndices.Add(i);
        }

        // Rastgele 60 obje seç ve her birine obje fýrlat
        for (int i = 0; i < 60; i++)
        {
            if (availableIndices.Count > 0)
            {
                // Rastgele bir indeks seç
                int randomIndex = Random.Range(0, availableIndices.Count);

                // Ýlgili objeyi aktif hale getir
                GameObject selectedObject = objects[availableIndices[randomIndex]];
                selectedObject.SetActive(true);

                // Seçili objeye doðru bir obje fýrlat
                ShootAtObject(selectedObject);

                // Efekt baþlatma coroutine çaðýr
                StartCoroutine(TriggerEffectAndDestroy(selectedObject));

                // Seçilen indeksi listeden kaldýr
                availableIndices.RemoveAt(randomIndex);
            }
        }
    }

    void ShootAtObject(GameObject target)
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            // Fýrlatýlacak objeyi oluþtur
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

            // Hedefe doðru yön belirle
            Vector3 direction = (target.transform.position - spawnPoint.position).normalized;

            // Rigidbody ile hareket ettir
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // Hýz belirleyin (10f örnek bir deðer)
            }

            // Fýrlatýlan objeyi yok etme
            Destroy(projectile, 5f); // 5 saniye sonra yok olur
        }
    }

    IEnumerator TriggerEffectAndDestroy(GameObject obj)
    {
        // 2 saniye bekle
        yield return new WaitForSeconds(2f);

        // Objeyi pasif hale getir
        obj.SetActive(false);

        // Efekt yarat
        if (effects.Count > 0)
        {
            GameObject chosenEffect = effects[0]; // Burayý deðiþtirerek farklý efekt seçebilirsiniz
            GameObject effectInstance = Instantiate(chosenEffect, obj.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            Destroy(effectInstance);
        }
    }
}
