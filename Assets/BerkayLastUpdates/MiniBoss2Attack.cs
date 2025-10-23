using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAttack : MonoBehaviour
{
    // 5 objeyi tutacak liste
    public List<GameObject> objects = new List<GameObject>();

    // Efektler i�in prefab listesi
    public List<GameObject> effects = new List<GameObject>();

    // S�re kontrol� i�in bir saya�
    private float timer = 0f;

    // Rastgele nesne se�im s�resi
    public float interval = 20f;

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

        // 20 saniye ge�tiyse rastgele 3 obje se� ve aktif et
        if (timer >= interval)
        {
            ActivateRandomObjects();
            timer = 0f; // Saya� s�f�rla
        }
    }

    void ActivateRandomObjects()
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

        // Rastgele 3 obje se�
        for (int i = 0; i < 5; i++)
        {
            if (availableIndices.Count > 0)
            {
                // Rastgele bir indeks se�
                int randomIndex = Random.Range(0, availableIndices.Count);

                // �lgili objeyi aktif hale getir
                GameObject selectedObject = objects[availableIndices[randomIndex]];
                selectedObject.SetActive(true);

                // Efekt ba�latma coroutine �a��r
                StartCoroutine(TriggerEffectAndDestroy(selectedObject));

                // Se�ilen indeksi listeden kald�r
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
            // �stedi�iniz efekti se�in (�rne�in, 0. indeks)
            GameObject chosenEffect = effects[0]; // Buray� de�i�tirerek farkl� efekt se�ebilirsiniz

            // Efekti objenin pozisyonunda olu�tur
            GameObject effectInstance = Instantiate(chosenEffect, obj.transform.position, Quaternion.identity);

            // Efektin tamamlanmas�n� bekle
            yield return new WaitForSeconds(1f);

            // Efekti yok et
            Destroy(effectInstance);
        }
    }
}
