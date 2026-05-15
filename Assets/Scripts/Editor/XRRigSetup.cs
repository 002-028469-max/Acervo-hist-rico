#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

/// <summary>
/// Editor utility que cria automaticamente um XR Rig completo com
/// mãos (controllers) e o XR Device Simulator na cena ativa.
/// Acesse via menu: Acervo Histórico ▸ Configurar XR Rig + Mãos
/// </summary>
public static class XRRigSetup
{
    private const string SimulatorPrefabPath =
        "Assets/Samples/XR Interaction Toolkit/2.6.5/XR Device Simulator/XR Device Simulator.prefab";

    [MenuItem("Acervo Histórico/Configurar XR Rig + Mãos e Simulador")]
    public static void SetupXRRig()
    {
        // ─── 1. Desativar PlayerController antigo (se existir) ───
        var oldPlayer = Object.FindObjectOfType<PlayerController>();
        if (oldPlayer != null)
        {
            oldPlayer.gameObject.SetActive(false);
            Debug.Log("[XRRigSetup] PlayerController antigo desativado.");
        }

        // ─── 2. Remover XR Rig existente (se houver) ───
        var existingRig = Object.FindObjectOfType<XROrigin>();
        if (existingRig != null)
        {
            Undo.DestroyObjectImmediate(existingRig.gameObject);
            Debug.Log("[XRRigSetup] XR Rig antigo removido.");
        }

        // ─── 3. Criar XR Origin (XR Rig) ───
        GameObject xrOrigin = new GameObject("XR Origin (XR Rig)");
        Undo.RegisterCreatedObjectUndo(xrOrigin, "Criar XR Rig");

        var origin = xrOrigin.AddComponent<XROrigin>();
        xrOrigin.AddComponent<XRInteractionManager>();

        // Character Controller para colisão
        var charCtrl = xrOrigin.AddComponent<CharacterController>();
        charCtrl.center = new Vector3(0, 1f, 0);
        charCtrl.height = 2f;
        charCtrl.radius = 0.3f;

        // ─── 4. Camera Offset ───
        GameObject cameraOffset = new GameObject("Camera Offset");
        cameraOffset.transform.SetParent(xrOrigin.transform);
        cameraOffset.transform.localPosition = Vector3.zero;
        origin.CameraFloorOffsetObject = cameraOffset;

        // ─── 5. Main Camera ───
        // Remover câmera antiga da cena
        Camera oldCam = Camera.main;
        if (oldCam != null && oldCam.gameObject != xrOrigin)
        {
            Undo.DestroyObjectImmediate(oldCam.gameObject);
        }

        GameObject cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.SetParent(cameraOffset.transform);
        cameraGO.transform.localPosition = new Vector3(0, 1.6f, 0);

        Camera cam = cameraGO.AddComponent<Camera>();
        cam.nearClipPlane = 0.1f;
        cam.fieldOfView = 60f;

        cameraGO.AddComponent<AudioListener>();
        cameraGO.AddComponent<TrackedPoseDriver>();
        origin.Camera = cam;

        // ─── 6. Mão Esquerda (Left Hand Controller) ───
        GameObject leftHand = CreateHandController("Left Hand Controller",
            cameraOffset.transform, true);

        // ─── 7. Mão Direita (Right Hand Controller) ───
        GameObject rightHand = CreateHandController("Right Hand Controller",
            cameraOffset.transform, false);

        // ─── 8. Adicionar Ray Interactors para interação à distância ───
        AddRayInteractor(leftHand, true);
        AddRayInteractor(rightHand, false);

        // ─── 9. Criar modelo visual das mãos (cubos simples) ───
        CreateHandVisual(leftHand, true);
        CreateHandVisual(rightHand, false);

        // ─── 10. Posicionar o Rig ───
        xrOrigin.transform.position = new Vector3(0, 0, 0);

        // ─── 11. XR Device Simulator ───
        SetupDeviceSimulator();

        // ─── 12. Locomotion (Teleport + Continuous Move) ───
        SetupLocomotion(xrOrigin, origin);

        // ─── 13. Adaptar objetos interativos existentes para XR ───
        AdaptInteractables();

        EditorUtility.SetDirty(xrOrigin);
        Selection.activeGameObject = xrOrigin;

        Debug.Log("✅ [XRRigSetup] XR Rig configurado com sucesso!\n" +
                  "• Main Camera com TrackedPoseDriver\n" +
                  "• Mão esquerda com Ray Interactor\n" +
                  "• Mão direita com Ray Interactor\n" +
                  "• XR Device Simulator adicionado\n" +
                  "• Locomotion configurada\n" +
                  "• Objetos interativos adaptados para XR");
    }

    private static GameObject CreateHandController(string name, Transform parent, bool isLeft)
    {
        GameObject hand = new GameObject(name);
        hand.transform.SetParent(parent);
        hand.transform.localPosition = isLeft
            ? new Vector3(-0.2f, 1.3f, 0.4f)
            : new Vector3(0.2f, 1.3f, 0.4f);

        // Tracked Pose Driver para rastreamento do controle
        var tpd = hand.AddComponent<TrackedPoseDriver>();

        // XR Controller
        var controller = hand.AddComponent<ActionBasedController>();

        // Tentar carregar o InputActionAsset do projeto
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            "Assets/InputSystem_Actions.inputactions");

        if (inputActions != null)
        {
            // Procurar action maps XRI
            string mapPrefix = isLeft ? "XRI Left" : "XRI Right";

            foreach (var map in inputActions.actionMaps)
            {
                if (map.name.Contains(mapPrefix) || map.name.Contains("XRI"))
                {
                    foreach (var action in map.actions)
                    {
                        var actionRef = InputActionReference.Create(action);

                        if (action.name == "Position")
                            controller.positionAction = new InputActionProperty(actionRef);
                        else if (action.name == "Rotation")
                            controller.rotationAction = new InputActionProperty(actionRef);
                        else if (action.name == "Select" || action.name == "Select Value")
                            controller.selectAction = new InputActionProperty(actionRef);
                        else if (action.name == "Activate" || action.name == "Activate Value")
                            controller.activateAction = new InputActionProperty(actionRef);
                    }
                }
            }
        }

        return hand;
    }

    private static void AddRayInteractor(GameObject hand, bool isLeft)
    {
        // Ray Interactor para apontar e interagir à distância
        var ray = hand.AddComponent<XRRayInteractor>();
        ray.maxRaycastDistance = 10f;

        // Line Renderer para visualizar o raio
        var lineRenderer = hand.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Material do raio
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = isLeft
            ? new Color(0.2f, 0.6f, 1f, 0.8f)
            : new Color(1f, 0.4f, 0.2f, 0.8f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0.3f);

        // XR Interactor Line Visual
        var lineVisual = hand.AddComponent<XRInteractorLineVisual>();
        lineVisual.lineLength = 10f;
    }

    private static void CreateHandVisual(GameObject hand, bool isLeft)
    {
        // Modelo visual simples da mão (cubo representando o controle)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = isLeft ? "Left Hand Model" : "Right Hand Model";
        visual.transform.SetParent(hand.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.08f, 0.04f, 0.15f);

        // Remover collider do visual (não deve interferir com raycasts)
        Object.DestroyImmediate(visual.GetComponent<Collider>());

        // Cor do controle
        var renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = isLeft
                ? new Color(0.2f, 0.5f, 0.9f, 1f)  // Azul para esquerda
                : new Color(0.9f, 0.3f, 0.2f, 1f);  // Vermelho para direita
            renderer.material = mat;
        }

        // Indicador de trigger (parte da frente)
        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        trigger.name = "Trigger Indicator";
        trigger.transform.SetParent(hand.transform);
        trigger.transform.localPosition = new Vector3(0, -0.015f, 0.08f);
        trigger.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        Object.DestroyImmediate(trigger.GetComponent<Collider>());

        var triggerRenderer = trigger.GetComponent<Renderer>();
        if (triggerRenderer != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = Color.white;
            mat.SetFloat("_Smoothness", 0.9f);
            triggerRenderer.material = mat;
        }
    }

    private static void SetupDeviceSimulator()
    {
        // Remover simulador existente
        var existingSim = Object.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator>();
        if (existingSim != null)
        {
            Undo.DestroyObjectImmediate(existingSim.gameObject);
        }

        // Tentar carregar o prefab do simulador
        GameObject simPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SimulatorPrefabPath);

        if (simPrefab != null)
        {
            GameObject simInstance = (GameObject)PrefabUtility.InstantiatePrefab(simPrefab);
            Undo.RegisterCreatedObjectUndo(simInstance, "Adicionar XR Device Simulator");
            Debug.Log("[XRRigSetup] XR Device Simulator adicionado do prefab.");
        }
        else
        {
            Debug.LogWarning("[XRRigSetup] Prefab do XR Device Simulator não encontrado em:\n" +
                SimulatorPrefabPath + "\nVocê pode adicioná-lo manualmente.");
        }
    }

    private static void SetupLocomotion(GameObject xrOrigin, XROrigin origin)
    {
        // Movimento simples com WASD + Mouse (funciona sem Input Actions)
        var movement = xrOrigin.AddComponent<SimpleXRMovement>();
        movement.moveSpeed = 3f;
        movement.mouseSensitivity = 2f;

        // Encontrar a câmera filha
        Camera cam = xrOrigin.GetComponentInChildren<Camera>();
        if (cam != null)
            movement.cameraTransform = cam.transform;

        Debug.Log("[XRRigSetup] Movimento SimpleXR configurado (WASD + Mouse).");
    }

    [MenuItem("Acervo Histórico/Adaptar Objetos Interativos para XR")]
    public static void AdaptInteractables()
    {
        int count = 0;
        // Encontrar todos os MonoBehaviours que implementam IInteractable
        var allBehaviours = Object.FindObjectsOfType<MonoBehaviour>(true);

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour is IInteractable)
            {
                var go = behaviour.gameObject;

                // Adicionar Collider se não existir (necessário para raycasts XR)
                if (go.GetComponent<Collider>() == null)
                {
                    var box = go.AddComponent<BoxCollider>();
                    Debug.Log($"[XRRigSetup] BoxCollider adicionado em '{go.name}'");
                }

                // Adicionar XRInteractableAdapter se não existir
                if (go.GetComponent<XRInteractableAdapter>() == null)
                {
                    Undo.AddComponent<XRInteractableAdapter>(go);
                    count++;
                    Debug.Log($"[XRRigSetup] XRInteractableAdapter adicionado em '{go.name}'");
                }
            }
        }

        Debug.Log($"✅ [XRRigSetup] {count} objetos adaptados para interação XR.");
    }

    [MenuItem("Acervo Histórico/Remover XR Rig")]
    public static void RemoveXRRig()
    {
        var rig = Object.FindObjectOfType<XROrigin>();
        if (rig != null)
        {
            Undo.DestroyObjectImmediate(rig.gameObject);
            Debug.Log("[XRRigSetup] XR Rig removido.");
        }

        var sim = Object.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator>();
        if (sim != null)
        {
            Undo.DestroyObjectImmediate(sim.gameObject);
            Debug.Log("[XRRigSetup] XR Device Simulator removido.");
        }

        // Reativar PlayerController antigo
        var players = Object.FindObjectsOfType<PlayerController>(true);
        foreach (var p in players)
        {
            p.gameObject.SetActive(true);
        }
    }
}
#endif
