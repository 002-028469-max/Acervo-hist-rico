using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.InputSystem.XR;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

/// <summary>
/// Configura automaticamente a cena do Menu Principal para funcionar em VR.
/// Detecta se um headset VR está conectado e converte o Canvas de Screen Space
/// para World Space, criando um XR Rig com Ray Interactors para interação.
/// Compatível com Meta Quest via OpenXR (Unity 6).
/// Se VR não estiver ativo, não faz nada (menu funciona normal com mouse).
/// </summary>
public class VRMenuSetup : MonoBehaviour
{
    /// <summary>
    /// Chamado automaticamente pelo AutoConfigDrawers quando a cena MainMenu carrega.
    /// Verifica se VR está ativo e configura tudo.
    /// </summary>
    public static void Setup()
    {
        if (!IsVRActive())
        {
            Debug.Log("[VRMenuSetup] VR não detectado. Menu funcionará no modo desktop normal.");
            return;
        }

        Debug.Log("🥽 [VRMenuSetup] VR DETECTADO (Meta Quest)! Configurando menu para realidade virtual...");

        // Criar XR Rig para a cena do menu
        GameObject xrRig = SetupXRRig();

        // Converter Canvas do menu para World Space
        ConvertCanvasToWorldSpace(xrRig);

        // Configurar EventSystem para XR
        SetupXREventSystem();

        Debug.Log("✅ [VRMenuSetup] Menu VR configurado! Use os controles Meta Quest para apontar e selecionar.");
    }

    /// <summary>
    /// Detecta se um dispositivo VR está ativo.
    /// Funciona com Meta Quest 2, Quest 3, Quest Pro via OpenXR.
    /// </summary>
    static bool IsVRActive()
    {
        // Método 1: XRSettings (funciona na maioria dos casos)
        if (XRSettings.isDeviceActive)
            return true;

        // Método 2: Verificar dispositivos XR conectados
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(xrDisplaySubsystems);
        foreach (var subsystem in xrDisplaySubsystems)
        {
            if (subsystem.running)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Cria um XR Rig mínimo para a cena do menu.
    /// Inclui câmera e dois Ray Interactors (mãos esquerda e direita).
    /// </summary>
    static GameObject SetupXRRig()
    {
        // Verificar se já existe um XR Rig
        var existingRig = Object.FindAnyObjectByType<XROrigin>();
        if (existingRig != null)
        {
            Debug.Log("[VRMenuSetup] XR Rig já existe na cena.");
            return existingRig.gameObject;
        }

        // Criar XR Origin (XR Rig)
        GameObject rigGO = new GameObject("XR Origin (Menu VR)");
        rigGO.transform.position = Vector3.zero;

        var xrOrigin = rigGO.AddComponent<XROrigin>();

        // Camera Offset (altura dos olhos)
        GameObject cameraOffset = new GameObject("Camera Offset");
        cameraOffset.transform.SetParent(rigGO.transform, false);
        cameraOffset.transform.localPosition = Vector3.zero;
        xrOrigin.CameraFloorOffsetObject = cameraOffset;

        // Câmera VR
        Camera existingCam = Camera.main;
        GameObject cameraGO;

        if (existingCam != null)
        {
            // Reutilizar câmera existente
            cameraGO = existingCam.gameObject;
            cameraGO.transform.SetParent(cameraOffset.transform, false);
            cameraGO.transform.localPosition = new Vector3(0, 1.6f, 0);
            cameraGO.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Criar nova câmera
            cameraGO = new GameObject("Camera VR");
            cameraGO.tag = "MainCamera";
            cameraGO.transform.SetParent(cameraOffset.transform, false);
            cameraGO.transform.localPosition = new Vector3(0, 1.6f, 0);
            var cam = cameraGO.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
        }

        // Adicionar TrackedPoseDriver se não existir
        if (cameraGO.GetComponent<TrackedPoseDriver>() == null)
        {
            var tpd = cameraGO.AddComponent<TrackedPoseDriver>();
            tpd.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        }

        xrOrigin.Camera = cameraGO.GetComponent<Camera>();

        // AudioListener
        if (Object.FindAnyObjectByType<AudioListener>() == null)
            cameraGO.AddComponent<AudioListener>();

        // ══════════════════════════════════════════════
        //  Controles Meta Quest (Ray Interactors para UI)
        // ══════════════════════════════════════════════

        // Mão Direita
        CreateHandController(cameraOffset.transform, "Controle Direito",
            true, new Color(0.2f, 0.6f, 1f));

        // Mão Esquerda
        CreateHandController(cameraOffset.transform, "Controle Esquerdo",
            false, new Color(1f, 0.4f, 0.2f));

        // XR Interaction Manager (necessário para os interactors)
        if (Object.FindAnyObjectByType<XRInteractionManager>() == null)
        {
            new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
        }

        Debug.Log("🎮 [VRMenuSetup] XR Rig criado com Ray Interactors para menu (Meta Quest).");
        return rigGO;
    }

    /// <summary>
    /// Cria um controle de mão com XR Ray Interactor para apontar na UI.
    /// Compatível com XR Interaction Toolkit 3.x e Meta Quest controllers.
    /// </summary>
    static void CreateHandController(Transform parent, string name, bool isRight, Color rayColor)
    {
        GameObject controllerGO = new GameObject(name);
        controllerGO.transform.SetParent(parent, false);
        controllerGO.transform.localPosition = isRight
            ? new Vector3(0.2f, 1.3f, 0.4f)
            : new Vector3(-0.2f, 1.3f, 0.4f);

        // TrackedPoseDriver para rastreamento do controle Meta Quest
        controllerGO.AddComponent<TrackedPoseDriver>();

        // ActionBasedController (compatível com XR Interaction Toolkit 3.x)
        controllerGO.AddComponent<ActionBasedController>();

        // Ray Interactor (para apontar nos botões da UI)
        var rayInteractor = controllerGO.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        rayInteractor.maxRaycastDistance = 20f;

        // Line Renderer (visual do raio)
        var lineRenderer = controllerGO.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Material do raio
        var shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.color = rayColor;
            lineRenderer.material = mat;
        }

        lineRenderer.startColor = new Color(rayColor.r, rayColor.g, rayColor.b, 0.8f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0.3f);

        // XR Interactor Line Visual (visual mais bonito do raio)
        var lineVisual = controllerGO.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>();
        lineVisual.lineLength = 10f;
    }

    /// <summary>
    /// Encontra todos os Canvas na cena e converte de Screen Space para World Space.
    /// Posiciona o Canvas na frente do jogador para que seja visível no VR.
    /// Adiciona TrackedDeviceGraphicRaycaster para interação com controles Meta Quest.
    /// </summary>
    static void ConvertCanvasToWorldSpace(GameObject xrRig)
    {
        Canvas[] allCanvas = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var canvas in allCanvas)
        {
            // Ignorar canvas que já são World Space
            if (canvas.renderMode == RenderMode.WorldSpace)
                continue;

            // Ignorar sub-canvas (canvas filhos de outro canvas)
            if (canvas.transform.parent != null && canvas.transform.parent.GetComponent<Canvas>() != null)
                continue;

            Debug.Log($"[VRMenuSetup] Convertendo Canvas '{canvas.name}' para World Space...");

            // Converter para World Space
            canvas.renderMode = RenderMode.WorldSpace;

            // Posicionar na frente do jogador
            canvas.transform.position = new Vector3(0, 1.6f, 3f); // 3m à frente, altura dos olhos
            canvas.transform.rotation = Quaternion.identity; // Virado para o jogador

            // Escala para ficar legível no VR (cada unidade UI → 1mm)
            canvas.transform.localScale = Vector3.one * 0.002f;

            // Ajustar tamanho se necessário
            var rt = canvas.GetComponent<RectTransform>();
            if (rt != null)
            {
                if (rt.sizeDelta.x < 100)
                    rt.sizeDelta = new Vector2(1920, 1080);
            }

            // Remover GraphicRaycaster padrão (usa mouse) e adicionar o de VR
            var oldRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (oldRaycaster != null)
                Object.Destroy(oldRaycaster);

            // Adicionar TrackedDeviceGraphicRaycaster (funciona com controles Meta Quest)
            if (canvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
                canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();

            // Garantir que o Canvas tem a camera de referência
            Camera vrCam = xrRig.GetComponentInChildren<Camera>();
            if (vrCam != null)
                canvas.worldCamera = vrCam;
        }
    }

    /// <summary>
    /// Substitui o StandaloneInputModule padrão pelo XRUIInputModule
    /// para que o EventSystem processe inputs dos controles Meta Quest.
    /// </summary>
    static void SetupXREventSystem()
    {
        EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();

        if (eventSystem == null)
        {
            // Criar EventSystem se não existir
            GameObject esGO = new GameObject("EventSystem");
            eventSystem = esGO.AddComponent<EventSystem>();
        }

        // Remover StandaloneInputModule (não funciona em VR)
        var standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (standaloneModule != null)
        {
            Object.Destroy(standaloneModule);
            Debug.Log("[VRMenuSetup] StandaloneInputModule removido.");
        }

        // Adicionar XRUIInputModule (processa inputs de controles Meta Quest)
        if (eventSystem.GetComponent<XRUIInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<XRUIInputModule>();
            Debug.Log("[VRMenuSetup] XRUIInputModule adicionado ao EventSystem.");
        }
    }
}
