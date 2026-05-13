using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sistema Antes/Depois para objetos históricos.
/// Alterna entre material degradado e restaurado.
/// </summary>
public class BeforeAfterController : MonoBehaviour
{
    [Header("Materiais")]
    public Material degradedMaterial;
    public Material restoredMaterial;

    [Header("UI")]
    public Button toggleButton;
    public TMPro.TextMeshProUGUI buttonText;

    private Renderer objectRenderer;
    private bool isRestored;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null && degradedMaterial != null)
            objectRenderer.material = degradedMaterial;

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleState);

        UpdateButtonText();
    }

    public void ToggleState()
    {
        isRestored = !isRestored;

        if (objectRenderer != null)
        {
            objectRenderer.material = isRestored ? restoredMaterial : degradedMaterial;
        }

        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isRestored ? "Ver Degradado" : "Ver Restaurado";
        }
    }
}
