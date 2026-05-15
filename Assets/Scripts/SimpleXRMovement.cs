using UnityEngine;

/// <summary>
/// Movimento simples para o XR Rig que funciona com teclado/mouse
/// sem depender de Input Actions configuradas.
/// Funciona tanto no modo simulador quanto com óculos VR.
/// </summary>
public class SimpleXRMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float gravity = -9.81f;

    [Header("Camera (Mouse Look)")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    [Header("Interação")]
    public float interactionRange = 5f;
    public LayerMask interactionMask = ~0;

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation;
    private IInteractable currentHover;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.center = new Vector3(0, 1f, 0);
            controller.height = 2f;
            controller.radius = 0.3f;
        }

        // Encontrar câmera automaticamente
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleInteraction();

        // ESC para liberar/prender cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleInteraction()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentHover != interactable)
                {
                    currentHover?.OnHoverExit();
                    currentHover = interactable;
                    currentHover.OnHoverEnter();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    interactable.Interact();
                }
            }
            else
            {
                if (currentHover != null)
                {
                    currentHover.OnHoverExit();
                    currentHover = null;
                }
            }
        }
        else
        {
            if (currentHover != null)
            {
                currentHover.OnHoverExit();
                currentHover = null;
            }
        }
    }
}
