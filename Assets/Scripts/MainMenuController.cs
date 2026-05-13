using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controlador do Menu Principal.
/// Gerencia botões Jogar, Créditos e Sair com efeito de fade.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject menuPanel;
    public GameObject creditsPanel;
    public CanvasGroup canvasGroup;

    [Header("Configurações")]
    public string gameSceneName = "MainScene";
    public float fadeDuration = 0.5f;

    private bool isTransitioning;

    void Start()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void OnPlayButton()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnCreditsButton()
    {
        if (creditsPanel != null)
        {
            bool isActive = creditsPanel.activeSelf;
            creditsPanel.SetActive(!isActive);
        }
    }

    public void OnCloseCreditsButton()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
