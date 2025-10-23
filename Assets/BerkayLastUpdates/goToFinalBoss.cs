using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Çarpýþma algýlandýðýnda çaðrýlan fonksiyon
    private void OnCollisionEnter(Collision collision)
    {
        // Çarpýþtýðýnýz obje "Player" tag'ine sahipse sahne deðiþtir
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

    // Bir sonraki sahneye geçiþ yapar
    private void ChangeToNextScene()
    {
        // Þu anki aktif sahneyi al
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Bir sonraki sahnenin indeksini belirle
        int nextSceneIndex = currentSceneIndex + 1;

        // Eðer toplam sahne sayýsýný geçmiyorsa geçiþ yap
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Bir sonraki sahneye geçiliyor: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Son sahneye ulaþýldý, daha ileri geçiþ yapýlamaz.");
        }
    }
}
