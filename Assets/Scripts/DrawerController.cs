using UnityEngine;

/// <summary>
/// Controlador de gaveta interativa.
/// Abre/fecha com Lerp suave e revela o objeto histórico dentro.
/// </summary>
public class DrawerController : MonoBehaviour, IInteractable
{
    [Header("Posições")]
    public Vector3 closedPosition;
    public Vector3 openPosition;

    [Header("Configurações")]
    public float slideSpeed = 3f;
    public GameObject itemInDrawer;

    [Header("Visual")]
    public Color hoverColor = new Color(1f, 0.9f, 0.7f, 1f);

    private bool isOpen;
    private bool isMoving;
    private Vector3 targetPosition;
    private Renderer drawerRenderer;
    private Color originalColor;

    void Start()
    {
        targetPosition = closedPosition;
        transform.localPosition = closedPosition;
        drawerRenderer = GetComponent<Renderer>();

        if (drawerRenderer != null)
            originalColor = drawerRenderer.material.color;

        if (itemInDrawer != null)
            itemInDrawer.SetActive(false);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * slideSpeed);

            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.005f)
            {
                transform.localPosition = targetPosition;
                isMoving = false;
            }
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? openPosition : closedPosition;
        isMoving = true;

        if (itemInDrawer != null)
        {
            HistoricalItem histItem = itemInDrawer.GetComponent<HistoricalItem>();
            if (isOpen)
            {
                itemInDrawer.SetActive(true);
                if (histItem != null) histItem.OpenPanel();
            }
            else
            {
                if (histItem != null) histItem.ClosePanel();
            }
        }
    }

    public void OnHoverEnter()
    {
        if (drawerRenderer != null)
            drawerRenderer.material.color = hoverColor;
    }

    public void OnHoverExit()
    {
        if (drawerRenderer != null)
            drawerRenderer.material.color = originalColor;
    }
}
