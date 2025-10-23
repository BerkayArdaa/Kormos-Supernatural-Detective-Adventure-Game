using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossAttack : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>(); // Objeler listesi
    public List<GameObject> effects = new List<GameObject>(); // Efektler i�in prefab listesi
    public GameObject projectilePrefab; // F�rlat�lacak obje
    public Transform spawnPoint; // F�rlatma noktas�

    private float timer = 0f; // Saya�
    public float interval = 20f; // Rastgele se�im s�resi

    void Start()
    {
        // T�m objeleri pasif hale getir
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        // Saya� zaman ekleme
        timer += Time.deltaTime;

        // Belirlenen s�re ge�tiyse rastgele 60 obje se� ve her birine obje f�rlat
        if (timer >= interval)
        {
            ActivateRandomObjectsAndShoot();
            timer = 0f; // Saya� s�f�rla
        }
    }

    void ActivateRandomObjectsAndShoot()
    {
        // T�m objeleri tekrar pasif hale getir
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // Rastgele se�im i�in kullan�lmayan indeksleri listele
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < objects.Count; i++)
        {
            availableIndices.Add(i);
        }

        // Rastgele 60 obje se� ve her birine obje f�rlat
        for (int i = 0; i < 60; i++)
        {
            if (availableIndices.Count > 0)
            {
                // Rastgele bir indeks se�
                int randomIndex = Random.Range(0, availableIndices.Count);

                // �lgili objeyi aktif hale getir
                GameObject selectedObject = objects[availableIndices[randomIndex]];
                selectedObject.SetActive(true);

                // Se�ili objeye do�ru bir obje f�rlat
                ShootAtObject(selectedObject);

                // Efekt ba�latma coroutine �a��r
                StartCoroutine(TriggerEffectAndDestroy(selectedObject));

                // Se�ilen indeksi listeden kald�r
                availableIndices.RemoveAt(randomIndex);
            }
        }
    }

    void ShootAtObject(GameObject target)
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            // F�rlat�lacak objeyi olu�tur
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

            // Hedefe do�ru y�n belirle
            Vector3 direction = (target.transform.position - spawnPoint.position).normalized;

            // Rigidbody ile hareket ettir
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // H�z belirleyin (10f �rnek bir de�er)
            }

            // F�rlat�lan objeyi yok etme
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
            GameObject chosenEffect = effects[0]; // Buray� de�i�tirerek farkl� efekt se�ebilirsiniz
            GameObject effectInstance = Instantiate(chosenEffect, obj.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
            Destroy(effectInstance);
        }
    }
}
