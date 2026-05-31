using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Adaptador que conecta o sistema de interação XR (XR Interaction Toolkit)
/// com os scripts existentes que implementam IInteractable.
/// Adicione este componente junto com um XRSimpleInteractable nos objetos interativos.
/// </summary>
[RequireComponent(typeof(Collider))]
public class XRInteractableAdapter : MonoBehaviour
{
    private IInteractable interactable;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable xrInteractable;

    void Awake()
    {
        interactable = GetComponent<IInteractable>();

        // Adicionar XRSimpleInteractable se não existir
        xrInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (xrInteractable == null)
        {
            xrInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }

        // Registrar eventos
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.AddListener(OnSelectEntered);
            xrInteractable.hoverEntered.AddListener(OnHoverEntered);
            xrInteractable.hoverExited.AddListener(OnHoverExited);
        }
    }

    void OnDestroy()
    {
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.RemoveListener(OnSelectEntered);
            xrInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            xrInteractable.hoverExited.RemoveListener(OnHoverExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactable?.Interact();
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        interactable?.OnHoverEnter();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        interactable?.OnHoverExit();
    }
}
