using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // �arp��ma alg�land���nda �a�r�lan fonksiyon
    private void OnCollisionEnter(Collision collision)
    {
        // �arp��t���n�z obje "Player" tag'ine sahipse sahne de�i�tir
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeToNextScene();
        }
    }

    // Alternatif olarak tetikleyici (Trigger) ile kullanabilirsiniz
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeToNextScene();
        }
    }

    // Bir sonraki sahneye ge�i� yapar
    private void ChangeToNextScene()
    {
        // �u anki aktif sahneyi al
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Bir sonraki sahnenin indeksini belirle
        int nextSceneIndex = currentSceneIndex + 1;

        // E�er toplam sahne say�s�n� ge�miyorsa ge�i� yap
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Bir sonraki sahneye ge�iliyor: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Son sahneye ula��ld�, daha ileri ge�i� yap�lamaz.");
        }
    }
}
