using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controlador do Menu Principal.
/// Gerencia botões Jogar, Créditos e Sair com efeito de fade.
/// Compatível com VR — detecta automaticamente se headset está ativo
/// e pula manipulação de cursor (que não existe em VR).
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
    private bool isVR;

    void Start()
    {
        // Detectar se VR está ativo
        isVR = CheckVRActive();

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }

        // Cursor só existe no modo desktop — em VR não precisa
        if (!isVR)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (isVR)
            Debug.Log("🥽 [MainMenu] Modo VR ativo — cursor desabilitado, use os controles.");
    }

    /// <summary>
    /// Verifica se um dispositivo VR está conectado e ativo.
    /// </summary>
    bool CheckVRActive()
    {
        // Método 1: XRSettings
        if (XRSettings.isDeviceActive)
            return true;

        // Método 2: Subsistemas XR
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(xrDisplaySubsystems);
        foreach (var subsystem in xrDisplaySubsystems)
        {
            if (subsystem.running)
                return true;
        }

        return false;
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
