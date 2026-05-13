using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Painel de saída interativo na parede.
/// O jogador aponta e clica nele para retornar ao menu ou sair do jogo.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExitPanel : MonoBehaviour, IInteractable
{
    private Renderer panelRenderer;
    private Color originalColor;
    
    [Header("Configurações")]
    public Color hoverColor = new Color(0.8f, 0.2f, 0.2f); // Cor vermelha ao mirar
    public string menuSceneName = "MainMenu";

    void Awake()
    {
        panelRenderer = GetComponent<Renderer>();
        if (panelRenderer != null)
        {
            originalColor = panelRenderer.material.color;
        }
    }

    public void OnHoverEnter()
    {
        if (panelRenderer != null)
            panelRenderer.material.color = hoverColor;
    }

    public void OnHoverExit()
    {
        if (panelRenderer != null)
            panelRenderer.material.color = originalColor;
    }

    public void Interact()
    {
        Debug.Log("Painel clicado! Voltando ao menu...");
        
        // Retorna a cor ao normal
        OnHoverExit();

        // Volta pro menu ou fecha o jogo
        if (Application.CanStreamedLevelBeLoaded(menuSceneName))
        {
            // Restaura o cursor para o menu funcionar
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogWarning($"Cena '{menuSceneName}' não encontrada. Fechando aplicação.");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
