using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [Header("ending ui")]
    [SerializeField] private GameObject endingText;
    [SerializeField] private Button quitButton;
    [SerializeField] private Image fadeImage;

    [Header("timing")]
    [SerializeField] private float delayBeforeEndingScreen = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        // hide the ending content while the screen is black
        endingText.SetActive(false);
        quitButton.gameObject.SetActive(false);

        // begin completely black
        SetFadeAlpha(1f);

        // fade into the ending screen
        StartCoroutine(ShowEndingScreen());
    }

    private IEnumerator ShowEndingScreen()
    {
        yield return StartCoroutine(FadeIn());

        endingText.SetActive(true);
        quitButton.gameObject.SetActive(true);

        quitButton.onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        // stop play mode when testing inside the unity editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // close the built game
        Application.Quit();
#endif
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = 1f - progress;

            SetFadeAlpha(alpha);

            yield return null;
        }

        SetFadeAlpha(0f);
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            SetFadeAlpha(progress);

            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}