using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Objeto histórico interativo.
/// Ao clicar, exibe um painel de informações flutuante com nome, descrição e foto.
/// </summary>
public class HistoricalItem : MonoBehaviour, IInteractable
{
    [Header("Dados do Artefato")]
    public string itemName = "Artefato";
    [TextArea(3, 10)]
    public string description = "Descrição do artefato histórico.";
    public Sprite itemPhoto;

    [Header("Painel de Info (World Space Canvas)")]
    public GameObject infoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image photoImage;
    public Button closeButton;

    [Header("Visual")]
    public Color hoverColor = new Color(1f, 0.85f, 0.5f, 1f);
    public bool billboardPanel = true;

    private bool isPanelOpen;
    private Renderer itemRenderer;
    private Color originalColor;
    private Transform playerCamera;

    void Start()
    {
        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
            originalColor = itemRenderer.material.color;

        // Preencher dados
        if (nameText != null) nameText.text = itemName;
        if (descriptionText != null) descriptionText.text = description;
        if (photoImage != null && itemPhoto != null) photoImage.sprite = itemPhoto;

        // Fechar painel
        if (infoPanel != null) infoPanel.SetActive(false);

        // Botão fechar
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        // Encontrar camera do jogador
        Camera mainCam = Camera.main;
        if (mainCam != null) playerCamera = mainCam.transform;
    }

    void LateUpdate()
    {
        // Billboard: painel olha para o jogador
        if (billboardPanel && isPanelOpen && infoPanel != null && playerCamera != null)
        {
            infoPanel.transform.LookAt(playerCamera);
            infoPanel.transform.Rotate(0f, 180f, 0f);
        }
    }

    public void Interact()
    {
        if (isPanelOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        if (infoPanel != null)
        {
            // Atualiza os dados toda vez que abre, garantindo que a foto e o texto apareçam
            if (nameText != null) nameText.text = itemName;
            if (descriptionText != null) descriptionText.text = description;
            
            if (photoImage != null)
            {
                if (itemPhoto != null)
                {
                    photoImage.sprite = itemPhoto;
                    photoImage.color = Color.white; // Remove o fundo cinza
                    photoImage.preserveAspect = true; // Mantém a proporção real da foto
                }
            }

            infoPanel.SetActive(true);
            isPanelOpen = true;
        }
    }

    public void ClosePanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
            isPanelOpen = false;
        }
    }

    public void OnHoverEnter()
    {
        if (itemRenderer != null)
            itemRenderer.material.color = hoverColor;
    }

    public void OnHoverExit()
    {
        if (itemRenderer != null)
            itemRenderer.material.color = originalColor;
    }
}
