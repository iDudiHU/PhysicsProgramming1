using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private void Start()
    {
        loadScreen.SetActive(false);
    }

    public void OnStartGameClick()
    {
        mainMenu.SetActive(false);
        loadScreen.SetActive(true);
        StartCoroutine(LoadLevelAsync("Level1"));
    }

    private IEnumerator LoadLevelAsync(string sceneName)
    {
        float minLoadTime = 2f;
        float elapsedTime = 0f;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            elapsedTime += Time.deltaTime;
            float asyncProgress = asyncOperation.progress / 0.9f;
            float timedProgress = elapsedTime / minLoadTime;
            float progress = Mathf.Clamp01(Mathf.Min(asyncProgress, timedProgress));

            progressBar.value = progress;
            progressText.text = (progress * 100).ToString("0") + "%";

            if (asyncOperation.progress >= 0.9f && elapsedTime >= minLoadTime)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
