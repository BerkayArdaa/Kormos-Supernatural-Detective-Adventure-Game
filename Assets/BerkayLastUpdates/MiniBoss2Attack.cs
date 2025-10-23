using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAttack : MonoBehaviour
{
    // 5 objeyi tutacak liste
    public List<GameObject> objects = new List<GameObject>();

    // Efektler için prefab listesi
    public List<GameObject> effects = new List<GameObject>();

    // Süre kontrolü için bir sayaç
    private float timer = 0f;

    // Rastgele nesne seçim süresi
    public float interval = 20f;

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

        // 20 saniye geçtiyse rastgele 3 obje seç ve aktif et
        if (timer >= interval)
        {
            ActivateRandomObjects();
            timer = 0f; // Sayaç sýfýrla
        }
    }

    void ActivateRandomObjects()
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

        // Rastgele 3 obje seç
        for (int i = 0; i < 5; i++)
        {
            if (availableIndices.Count > 0)
            {
                // Rastgele bir indeks seç
                int randomIndex = Random.Range(0, availableIndices.Count);

                // Ýlgili objeyi aktif hale getir
                GameObject selectedObject = objects[availableIndices[randomIndex]];
                selectedObject.SetActive(true);

                // Efekt baþlatma coroutine çaðýr
                StartCoroutine(TriggerEffectAndDestroy(selectedObject));

                // Seçilen indeksi listeden kaldýr
                availableIndices.RemoveAt(randomIndex);
            }
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
            // Ýstediðiniz efekti seçin (örneðin, 0. indeks)
            GameObject chosenEffect = effects[0]; // Burayý deðiþtirerek farklý efekt seçebilirsiniz

            // Efekti objenin pozisyonunda oluþtur
            GameObject effectInstance = Instantiate(chosenEffect, obj.transform.position, Quaternion.identity);

            // Efektin tamamlanmasýný bekle
            yield return new WaitForSeconds(1f);

            // Efekti yok et
            Destroy(effectInstance);
        }
    }
}
