using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text loadingText;

    [SerializeField]
    private float minimumLoadingTime = 10f;

    [SerializeField]
    private float simulatedLoadingSpeed = 0.001f;

    void Start()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    IEnumerator ShowLoadingScreen()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;
        float elapsedTime = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            while (displayedProgress < targetProgress)
            {
                displayedProgress += simulatedLoadingSpeed * Time.deltaTime;
                displayedProgress = Mathf.Min(displayedProgress, targetProgress);

                progressBar.value = displayedProgress;

                if (loadingText != null)
                {
                    loadingText.text = "Loading: " + (displayedProgress * 100f).ToString("F0") + "%";
                }

                yield return null;
            }

            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                operation.allowSceneActivation = true;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
