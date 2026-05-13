#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SceneBuilderEditor : EditorWindow
{
    [InitializeOnLoadMethod]
    static void AutoBuildOnLoad()
    {
        // Auto-build se as cenas ainda não existem
        if (!File.Exists("Assets/Scenes/MainMenu.unity") || !File.Exists("Assets/Scenes/MainScene.unity"))
        {
            EditorApplication.delayCall += () =>
            {
                Debug.Log("🔨 Auto-construindo projeto Arquivo Histórico...");
                BuildProjectSilent();
            };
        }
    }

    [MenuItem("Arquivo Historico/Montar Projeto Completo")]
    public static void BuildProject()
    {
        if (!EditorUtility.DisplayDialog("Montar Projeto",
            "Isso criará as cenas MainMenu e MainScene com todo o conteúdo.\nDeseja continuar?", "Sim", "Cancelar"))
            return;

        BuildProjectSilent();
        EditorUtility.DisplayDialog("Sucesso!", "Projeto montado com sucesso!\n\nClique Play para testar.", "OK");
    }

    public static void BuildProjectSilent()
    {
        SetupLayers();
        SetupTextureImports();
        CreateMainMenuScene();
        CreateMainScene();
        SetupBuildSettings();
        // Abrir a cena MainMenu após construir
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        Debug.Log("✅ Projeto montado com sucesso! MainMenu aberta. Clique Play para testar.");
    }

    static void SetupLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        bool found = false;
        for (int i = 6; i < layers.arraySize; i++)
        {
            if (layers.GetArrayElementAtIndex(i).stringValue == "Grabbable") { found = true; break; }
        }
        if (!found)
        {
            layers.GetArrayElementAtIndex(6).stringValue = "Grabbable";
            tagManager.ApplyModifiedProperties();
        }
    }

    static void SetupTextureImports()
    {
        string[] fotos = { "foto_manuscrito", "foto_mapa", "foto_carta_alforria", "foto_epoca", "foto_diario" };
        foreach (var f in fotos)
        {
            string path = "Assets/Resources/Fotos/" + f + ".png";
            if (File.Exists(path))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null)
                {
                    ti.textureType = TextureImporterType.Sprite;
                    ti.spriteImportMode = SpriteImportMode.Single;
                    ti.maxTextureSize = 1024;
                    ti.SaveAndReimport();
                }
            }
        }
    }

    static Material CreateMat(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.color = color;
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    // ─── MAIN MENU ────────────────────────────────────────────
    static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Camera
        var cam = GameObject.Find("Main Camera");
        if (cam != null) cam.GetComponent<Camera>().backgroundColor = new Color(0.11f, 0.07f, 0.04f);

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();
        var cg = canvasGO.AddComponent<CanvasGroup>();

        // Background
        var bgGO = CreateUIImage(canvasGO.transform, "Background", new Color(0.17f, 0.09f, 0.06f, 1f));
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;

        // Menu Panel
        var menuPanel = new GameObject("MenuPanel");
        menuPanel.transform.SetParent(canvasGO.transform, false);
        var mpRT = menuPanel.AddComponent<RectTransform>();
        mpRT.anchorMin = Vector2.zero; mpRT.anchorMax = Vector2.one;
        mpRT.offsetMin = Vector2.zero; mpRT.offsetMax = Vector2.zero;

        // Título
        var titleGO = CreateTMPText(menuPanel.transform, "Title",
            "ARQUIVO HISTÓRICO\nINTERATIVO", 52, new Color(0.96f, 0.87f, 0.7f),
            TextAlignmentOptions.Center, new Vector2(0, 150), new Vector2(800, 150));

        // Subtítulo
        CreateTMPText(menuPanel.transform, "Subtitle",
            "Espaço Humberto de Maracanã\nVertex Virtualization Lab | UNDB", 22,
            new Color(0.8f, 0.7f, 0.55f), TextAlignmentOptions.Center,
            new Vector2(0, 40), new Vector2(600, 80));

        // Botões
        CreateMenuButton(menuPanel.transform, "BtnJogar", "Explorar o Acervo", new Vector2(0, -60), new Color(0.55f, 0.27f, 0.07f));
        CreateMenuButton(menuPanel.transform, "BtnCreditos", "Créditos", new Vector2(0, -140), new Color(0.4f, 0.2f, 0.05f));
        CreateMenuButton(menuPanel.transform, "BtnSair", "Sair", new Vector2(0, -220), new Color(0.3f, 0.12f, 0.05f));

        // Credits Panel
        var credPanel = new GameObject("CreditsPanel");
        credPanel.transform.SetParent(canvasGO.transform, false);
        var cpRT = credPanel.AddComponent<RectTransform>();
        cpRT.sizeDelta = new Vector2(700, 400);
        var cpImg = credPanel.AddComponent<Image>();
        cpImg.color = new Color(0.12f, 0.07f, 0.04f, 0.95f);

        CreateTMPText(credPanel.transform, "CreditsTitle", "CRÉDITOS", 36,
            new Color(0.96f, 0.87f, 0.7f), TextAlignmentOptions.Center,
            new Vector2(0, 140), new Vector2(600, 60));

        CreateTMPText(credPanel.transform, "CreditsText",
            "Breno Lucas Veras Melo\nTecnologias Emergentes — UNDB 2026\n\nVertex Virtualization Lab\nEspaço Humberto de Maracanã",
            24, new Color(0.8f, 0.7f, 0.55f), TextAlignmentOptions.Center,
            new Vector2(0, 10), new Vector2(600, 200));

        var closeCred = CreateMenuButton(credPanel.transform, "BtnCloseCredits", "Fechar", new Vector2(0, -150), new Color(0.5f, 0.2f, 0.05f));
        credPanel.SetActive(false);

        // GameManager
        var gmGO = new GameObject("GameManager");
        var mmc = gmGO.AddComponent<MainMenuController>();
        mmc.menuPanel = menuPanel;
        mmc.creditsPanel = credPanel;
        mmc.canvasGroup = cg;
        mmc.gameSceneName = "MainScene";

        // Wire buttons
        var btnJogar = menuPanel.transform.Find("BtnJogar").GetComponent<Button>();
        var btnCred = menuPanel.transform.Find("BtnCreditos").GetComponent<Button>();
        var btnSair = menuPanel.transform.Find("BtnSair").GetComponent<Button>();
        var btnCloseCred = credPanel.transform.Find("BtnCloseCredits").GetComponent<Button>();

        btnJogar.onClick.AddListener(() => { }); // placeholder
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnJogar.onClick, mmc.OnPlayButton);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnCred.onClick, mmc.OnCreditsButton);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnSair.onClick, mmc.OnQuitButton);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnCloseCred.onClick, mmc.OnCloseCreditsButton);
        // Remove placeholders
        btnJogar.onClick.RemoveAllListeners();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnJogar.onClick, mmc.OnPlayButton);

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("✅ Cena MainMenu criada: " + scenePath);
    }

    // ─── MAIN SCENE ───────────────────────────────────────────
    static void CreateMainScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Materials
        var matFloor = CreateMat("Floor_Wood", new Color(0.55f, 0.35f, 0.2f));
        var matWall = CreateMat("Wall_Beige", new Color(0.85f, 0.78f, 0.65f));
        var matCeiling = CreateMat("Ceiling_White", new Color(0.92f, 0.9f, 0.85f));
        var matDrawer = CreateMat("Drawer_DarkWood", new Color(0.35f, 0.2f, 0.1f));
        var matItem = CreateMat("Item_Gold", new Color(0.85f, 0.7f, 0.3f));
        var matDegraded = CreateMat("Degraded", new Color(0.6f, 0.55f, 0.4f));
        var matRestored = CreateMat("Restored", new Color(0.9f, 0.8f, 0.5f));

        // Floor
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(3, 1, 3);
        floor.GetComponent<Renderer>().sharedMaterial = matFloor;

        // Walls
        CreateWall("Wall_North", new Vector3(0, 2.5f, 15), new Vector3(30, 5, 0.3f), matWall);
        CreateWall("Wall_South", new Vector3(0, 2.5f, -15), new Vector3(30, 5, 0.3f), matWall);
        CreateWall("Wall_East", new Vector3(15, 2.5f, 0), new Vector3(0.3f, 5, 30), matWall);
        CreateWall("Wall_West", new Vector3(-15, 2.5f, 0), new Vector3(0.3f, 5, 30), matWall);

        // Ceiling
        var ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.position = new Vector3(0, 5, 0);
        ceiling.transform.localScale = new Vector3(30, 0.2f, 30);
        ceiling.GetComponent<Renderer>().sharedMaterial = matCeiling;

        // Lighting
        var lightGO = new GameObject("DirectionalLight");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1f, 0.94f, 0.8f);
        light.intensity = 1.2f;
        lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);

        // Point lights
        CreatePointLight("Light_Warm1", new Vector3(-5, 4, 5), new Color(1f, 0.9f, 0.7f), 12f, 2f);
        CreatePointLight("Light_Warm2", new Vector3(5, 4, -5), new Color(1f, 0.85f, 0.65f), 12f, 2f);
        CreatePointLight("Light_Warm3", new Vector3(0, 4, 0), new Color(1f, 0.95f, 0.8f), 15f, 1.5f);

        // Player
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.transform.position = new Vector3(0, 1.1f, -10);
        var cc = playerGO.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.center = new Vector3(0, 0.9f, 0);
        cc.radius = 0.3f;
        var pc = playerGO.AddComponent<PlayerController>();
        playerGO.AddComponent<CrosshairUI>();

        var camGO = new GameObject("PlayerCamera");
        camGO.transform.SetParent(playerGO.transform, false);
        camGO.transform.localPosition = new Vector3(0, 1.6f, 0);
        var playerCam = camGO.AddComponent<Camera>();
        playerCam.nearClipPlane = 0.1f;
        camGO.AddComponent<AudioListener>();
        pc.cameraTransform = camGO.transform;

        // Furniture/Shelves visual (decorative cabinet base)
        var cabinet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabinet.name = "Cabinet";
        cabinet.transform.position = new Vector3(0, 0.6f, 12);
        cabinet.transform.localScale = new Vector3(12, 1.2f, 1.5f);
        cabinet.GetComponent<Renderer>().sharedMaterial = matDrawer;

        // Drawers + Objects
        string[] names = { "Manuscrito Colonial", "Mapa Cartográfico", "Carta de Alforria", "Fotografia de Época", "Diário de Bordo" };
        string[] photos = { "foto_manuscrito", "foto_mapa", "foto_carta_alforria", "foto_epoca", "foto_diario" };
        string[] periods = { "Século XIX", "1844", "1870", "1920", "1890" };
        string[] descs = {
            "Manuscrito colonial redigido em tinta ferrogálica sobre pergaminho amarelado. Documento oficial da administração provincial do Maranhão, contendo registros de transações comerciais e decretos do governo imperial.",
            "Mapa cartográfico detalhado de São Luís do Maranhão, desenhado à mão com rosa dos ventos, linhas costeiras e anotações em português. Representa a geografia urbana e portuária da capital maranhense no período imperial.",
            "Documento jurídico oficial que concedia liberdade a uma pessoa escravizada. Esta carta de alforria, lavrada em cartório, representa um dos mais importantes documentos do período abolicionista brasileiro.",
            "Fotografia em sépia retratando uma cena urbana colonial de São Luís do Maranhão nos anos 1920. A imagem revela a arquitetura azulejada portuguesa e o cotidiano dos moradores da época.",
            "Diário de bordo de embarcação que operava no Trapiche de São Luís. Contém anotações de navegação, esboços marítimos e registros de carga, documentando as rotas comerciais do final do século XIX."
        };

        float startX = -4.5f;
        float spacing = 2.3f;

        for (int i = 0; i < 5; i++)
        {
            float x = startX + i * spacing;
            CreateDrawerWithItem(i, x, names[i], photos[i], periods[i], descs[i],
                matDrawer, matItem, matDegraded, matRestored);
        }

        // EventSystem
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        string scenePath = "Assets/Scenes/MainScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("✅ Cena MainScene criada: " + scenePath);
    }

    static void CreateDrawerWithItem(int idx, float x, string name, string photoRes,
        string period, string desc, Material matDrawer, Material matItem, Material matDeg, Material matRes)
    {
        float drawerY = 0.65f + (idx % 2) * 0.25f;
        Vector3 closedPos = new Vector3(x, drawerY, 11.2f);
        Vector3 openPos = closedPos + new Vector3(0, 0, -0.8f);

        // Drawer
        var drawer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        drawer.name = "Drawer_" + idx;
        drawer.transform.position = closedPos;
        drawer.transform.localScale = new Vector3(1.8f, 0.18f, 0.7f);
        drawer.GetComponent<Renderer>().sharedMaterial = matDrawer;
        drawer.layer = 0;

        var dc = drawer.AddComponent<DrawerController>();
        dc.closedPosition = closedPos;
        dc.openPosition = openPos;

        // Handle visual
        var handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Handle";
        handle.transform.SetParent(drawer.transform, false);
        handle.transform.localPosition = new Vector3(0, 0, -0.55f);
        handle.transform.localScale = new Vector3(0.2f, 0.3f, 0.05f);
        handle.GetComponent<Renderer>().sharedMaterial = matItem;
        Object.DestroyImmediate(handle.GetComponent<BoxCollider>());

        // Item inside
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = "Item_" + name.Replace(" ", "");
        item.transform.SetParent(drawer.transform, false);
        item.transform.localPosition = new Vector3(0, 0.15f, 0);
        item.transform.localScale = new Vector3(0.5f, 0.5f, 0.3f);
        item.GetComponent<Renderer>().sharedMaterial = matItem;
        item.layer = LayerMask.NameToLayer("Grabbable") >= 0 ? LayerMask.NameToLayer("Grabbable") : 0;

        var hi = item.AddComponent<HistoricalItem>();
        hi.itemName = name;
        hi.description = period + "\n\n" + desc;

        // Load sprite
        Sprite spr = LoadSpriteFromResources(photoRes);
        hi.itemPhoto = spr;

        // Before/After
        var bac = item.AddComponent<BeforeAfterController>();
        bac.degradedMaterial = matDeg;
        bac.restoredMaterial = matRes;

        // Info Panel (World Space Canvas)
        var panelCanvas = new GameObject("InfoPanel_" + idx);
        panelCanvas.transform.position = item.transform.position + drawer.transform.TransformDirection(Vector3.up) * 2.5f;
        var cvs = panelCanvas.AddComponent<Canvas>();
        cvs.renderMode = RenderMode.WorldSpace;
        var cvsRT = panelCanvas.GetComponent<RectTransform>();
        cvsRT.sizeDelta = new Vector2(400, 500);
        panelCanvas.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        panelCanvas.AddComponent<GraphicRaycaster>();

        // Panel Background
        var panelBG = CreateUIImage(panelCanvas.transform, "PanelBG", new Color(0.1f, 0.06f, 0.03f, 0.95f));
        var panelBGRT = panelBG.GetComponent<RectTransform>();
        panelBGRT.anchorMin = Vector2.zero; panelBGRT.anchorMax = Vector2.one;
        panelBGRT.offsetMin = Vector2.zero; panelBGRT.offsetMax = Vector2.zero;

        // Photo
        Image photoImg = null;
        if (spr != null)
        {
            var photoGO = CreateUIImage(panelCanvas.transform, "Photo", Color.white);
            photoImg = photoGO.GetComponent<Image>();
            photoImg.sprite = spr;
            photoImg.preserveAspect = true;
            var photoRT = photoGO.GetComponent<RectTransform>();
            photoRT.anchoredPosition = new Vector2(0, 130);
            photoRT.sizeDelta = new Vector2(350, 200);
        }

        // Name Text
        var nameT = CreateTMPText(panelCanvas.transform, "NameText", name, 32,
            new Color(0.96f, 0.87f, 0.6f), TextAlignmentOptions.Center,
            new Vector2(0, 5), new Vector2(380, 45));

        // Description Text
        var descT = CreateTMPText(panelCanvas.transform, "DescText", period + "\n" + desc, 16,
            new Color(0.85f, 0.8f, 0.7f), TextAlignmentOptions.TopLeft,
            new Vector2(0, -120), new Vector2(370, 200));

        // Close Button
        var closeBtnGO = new GameObject("CloseButton");
        closeBtnGO.transform.SetParent(panelCanvas.transform, false);
        var closeBtnRT = closeBtnGO.AddComponent<RectTransform>();
        closeBtnRT.anchoredPosition = new Vector2(180, 230);
        closeBtnRT.sizeDelta = new Vector2(40, 40);
        var closeBtnImg = closeBtnGO.AddComponent<Image>();
        closeBtnImg.color = new Color(0.6f, 0.15f, 0.1f);
        var closeBtn = closeBtnGO.AddComponent<Button>();

        var closeBtnText = CreateTMPText(closeBtnGO.transform, "X", "X", 24,
            Color.white, TextAlignmentOptions.Center, Vector2.zero, new Vector2(40, 40));

        // Wire up HistoricalItem references
        hi.infoPanel = panelCanvas;
        hi.nameText = nameT.GetComponent<TextMeshProUGUI>();
        hi.descriptionText = descT.GetComponent<TextMeshProUGUI>();
        hi.photoImage = photoImg;
        hi.closeButton = closeBtn;

        dc.itemInDrawer = item;

        panelCanvas.SetActive(false);
        item.SetActive(false);
    }

    static Sprite LoadSpriteFromResources(string name)
    {
        string path = "Assets/Resources/Fotos/" + name + ".png";
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s == null)
        {
            Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (t != null) s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
        return s;
    }

    // ─── HELPERS ──────────────────────────────────────────────
    static GameObject CreateWall(string name, Vector3 pos, Vector3 scale, Material mat)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().sharedMaterial = mat;
        return wall;
    }

    static void CreatePointLight(string name, Vector3 pos, Color color, float range, float intensity)
    {
        var go = new GameObject(name);
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.range = range;
        l.intensity = intensity;
        go.transform.position = pos;
    }

    static GameObject CreateUIImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject CreateTMPText(Transform parent, string name, string text, float fontSize,
        Color color, TextAlignmentOptions align, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return go;
    }

    static GameObject CreateMenuButton(Transform parent, string name, string text, Vector2 pos, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(350, 55);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        go.AddComponent<Button>();

        CreateTMPText(go.transform, "Text", text, 26, new Color(0.96f, 0.87f, 0.7f),
            TextAlignmentOptions.Center, Vector2.zero, new Vector2(340, 50));
        return go;
    }

    static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/MainScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("✅ Build Settings configurado: MainMenu (0), MainScene (1)");
    }
}
#endif
