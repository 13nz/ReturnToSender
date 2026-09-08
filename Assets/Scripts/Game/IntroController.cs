using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    [Header("intro ui")]
    [SerializeField] private Button playButton;
    [SerializeField] private Image fadeImage;

    [Header("fade settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("scene settings")]
    [SerializeField] private string firstSceneName = "PostOffice";

    private bool isFading;

    private void Start()
    {
        // connect the play button to the start game method
        playButton.onClick.AddListener(StartGame);

        // begin with the screen completely black
        SetFadeAlpha(1f);

        // fade into the intro screen
        StartCoroutine(FadeIn());
    }

    private void StartGame()
    {
        // prevent the button from being pressed multiple times
        if (isFading)
        {
            return;
        }

        isFading = true;
        playButton.interactable = false;

        // fade the screen to black before loading the game
        StartCoroutine(FadeOutAndLoadScene());
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

    private IEnumerator FadeOutAndLoadScene()
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

        // load the first playable scene after the fade completes
        SceneManager.LoadScene(firstSceneName);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}