using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// SETUP 100% AUTOMÁTICO — Funciona ao abrir qualquer cena com gavetas
/// e também quando o MainMenu transiciona para MainScene.
/// Não precisa de NADA manual.
/// </summary>
public class AutoConfigDrawers : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterSceneCallback()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Esperar um frame para todos os objetos carregarem
        var helper = new GameObject("_AutoConfigHelper").AddComponent<DelayedSetup>();
        helper.StartCoroutine(helper.RunNextFrame());
    }

    public static void RunSetup()
    {
        // Detectar se tem gavetas na cena
        bool hasDrawers = GameObject.Find("Drawer_0") != null
                       || GameObject.Find("Drawer_1") != null
                       || GameObject.Find("Gaveta_Interativa_1") != null;

        if (!hasDrawers)
        {
            Debug.Log("[AutoConfig] Cena sem gavetas, pulando configuração.");
            return;
        }

        Debug.Log("🔧 [AutoConfig] Gavetas detectadas! Configurando TUDO automaticamente...");

        var config = new AutoConfigDrawers();
        config.ConfigureAllDrawers();
        config.CreateExitTotem();
        config.SetupPlayer();

        Debug.Log("✅ [AutoConfig] TUDO PRONTO! Use WASD para andar, Mouse para olhar, Clique para interagir.");
    }

    // ═══════════════════════════════════════════════════════════
    //  GAVETAS
    // ═══════════════════════════════════════════════════════════

    void ConfigureAllDrawers()
    {
        int configured = 0;

        configured += SetupDrawer(0, "Humberto de Maracanã",
            "Humberto Barbosa Magalhães, eternizado como Humberto de Maracanã, " +
            "foi uma das maiores vozes da cultura maranhense. Como amo do Boi de Maracanã " +
            "por décadas, ele elevou o sotaque de matraca ao reconhecimento nacional.",
            new[] { "Fotos/foto_humberto", "Fotos/humberto" }) ? 1 : 0;

        configured += SetupDrawer(1, "Boi de Maracanã Enfeitado",
            "O Bumba-meu-Boi de Maracanã é uma das mais tradicionais manifestações culturais do Maranhão. " +
            "O Boi aparece ricamente enfeitado com bordados, fitas coloridas, canutilhos e espelhos, " +
            "pronto para a apresentação no arraial. A decoração é uma arte transmitida entre gerações, " +
            "cada detalhe carregando simbolismo e devoção aos santos juninos.",
            new[] { "Fotos/boi_enfeitado", "Fotos/Boi enfeitado" }) ? 1 : 0;

        configured += SetupDrawer(2, "Boi de Maracanã",
            "O Boi de Maracanã é um dos mais antigos e respeitados grupos de Bumba-meu-Boi do Maranhão, " +
            "fundado no bairro de Maracanã, em São Luís. Com sotaque de matraca, seu ritmo inconfundível " +
            "é marcado pelas batidas das matracas de madeira e pelos pandeirões.",
            new[] { "Fotos/Boi", "Fotos/boi" }) ? 1 : 0;

        configured += SetupDrawer(3, "O Caboclo de Pena",
            "O Caboclo de Pena é um dos personagens mais emblemáticos do Bumba-meu-Boi do Maranhão. " +
            "Vestido com cocar de penas e trajes que remetem à ancestralidade indígena, o Caboclo " +
            "representa a herança dos povos originários na formação cultural maranhense.",
            new[] { "Fotos/Caboclo", "Fotos/caboclo" }) ? 1 : 0;

        configured += SetupDrawer(4, "Tradição Histórica — Boi de Maracanã",
            "A tradição do Bumba-meu-Boi de Maracanã remonta a décadas de história e resistência cultural. " +
            "Reconhecido como Patrimônio Cultural Imaterial do Brasil, o Bumba-meu-Boi reúne elementos das culturas " +
            "africana, indígena e europeia. O Boi de Maracanã, com seu sotaque de matraca, " +
            "é símbolo vivo dessa tradição.",
            new[] { "Fotos/tradicao_historica", "Fotos/boi_maracana" }) ? 1 : 0;

        Debug.Log($"📦 [AutoConfig] {configured}/5 gavetas configuradas.");
    }

    bool SetupDrawer(int index, string itemName, string description, string[] photoPaths)
    {
        string nameA = "Drawer_" + index;
        string nameB = "Gaveta_Interativa_" + (index + 1);

        GameObject drawer = GameObject.Find(nameA) ?? GameObject.Find(nameB);
        if (drawer == null) return false;

        // Encontrar item
        Transform itemTrans = FindItem(drawer);
        if (itemTrans == null) return false;

        GameObject item = itemTrans.gameObject;

        HistoricalItem hi = item.GetComponent<HistoricalItem>();
        if (hi == null) hi = item.AddComponent<HistoricalItem>();

        hi.itemName = itemName;
        hi.description = description;

        // Foto
        Sprite spr = null;
        foreach (var p in photoPaths)
        {
            spr = LoadSprite(p);
            if (spr != null) break;
        }
        if (spr != null) hi.itemPhoto = spr;

        // DrawerController
        DrawerController dc = drawer.GetComponent<DrawerController>();
        if (dc != null)
        {
            dc.itemName = itemName;
            dc.description = description;
            dc.itemPhoto = spr;
            dc.itemInDrawer = item;
        }

        // Painel
        if (hi.infoPanel == null)
            BuildInfoPanel(hi, item);
        else
            UpdatePanel(hi, spr);

        item.SetActive(false);
        return true;
    }

    Transform FindItem(GameObject drawer)
    {
        foreach (Transform c in drawer.transform)
            if (c.GetComponent<HistoricalItem>() != null) return c;
        foreach (Transform c in drawer.transform)
            if (c.name.StartsWith("Item")) return c;
        foreach (Transform c in drawer.transform)
            if (!c.name.Contains("Handle") && !c.name.Contains("Number") && !c.name.Contains("Text"))
                return c;
        return null;
    }

    // ═══════════════════════════════════════════════════════════
    //  TOTEM DE SAÍDA
    // ═══════════════════════════════════════════════════════════

    void CreateExitTotem()
    {
        if (GameObject.Find("Totem_Sair") != null) return;

        var totem = new GameObject("Totem_Sair");
        var cabinet = GameObject.Find("Cabinet") ?? GameObject.Find("Comoda");
        totem.transform.position = cabinet != null
            ? cabinet.transform.position + new Vector3(8f, -cabinet.transform.position.y, -1f)
            : new Vector3(6f, 0f, 11f);

        // Pedestal
        var ped = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ped.name = "Pedestal";
        ped.transform.SetParent(totem.transform, false);
        ped.transform.localPosition = new Vector3(0, 0.5f, 0);
        ped.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        ApplyColor(ped, new Color(0.2f, 0.2f, 0.2f));

        // Cristal
        var crystal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crystal.name = "Botao_Cristal";
        crystal.transform.SetParent(totem.transform, false);
        crystal.transform.localPosition = new Vector3(0, 1.15f, 0);
        crystal.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        crystal.transform.localRotation = Quaternion.Euler(45, 45, 0);
        ApplyColor(crystal, new Color(0.8f, 0.1f, 0.1f));

        crystal.AddComponent<ExitPanel>().hoverColor = new Color(1f, 0.3f, 0.3f);

        // Texto
        var txtGO = new GameObject("Text_Sair");
        txtGO.transform.SetParent(totem.transform, false);
        txtGO.transform.localPosition = new Vector3(0, 1.6f, 0);
        txtGO.transform.localRotation = Quaternion.Euler(0, 180, 0);
        var tmp = txtGO.AddComponent<TextMeshPro>();
        tmp.text = "SAIR"; tmp.fontSize = 3f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.7f, 0.7f);
        txtGO.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 0.5f);

        Debug.Log("🚪 [AutoConfig] Totem de saída criado.");
    }

    // ═══════════════════════════════════════════════════════════
    //  JOGADOR
    // ═══════════════════════════════════════════════════════════

    void SetupPlayer()
    {
        // 1. Procurar jogador existente
        GameObject player = null;

        // Por nome
        string[] names = { "Player", "XR Origin (XR Rig)", "FPSController" };
        foreach (var n in names)
        {
            player = GameObject.Find(n);
            if (player != null) break;
        }

        // Por tag
        if (player == null)
        {
            try { player = GameObject.FindWithTag("Player"); } catch { }
        }

        // Por componente
        if (player == null)
        {
            var pc = Object.FindObjectOfType<PlayerController>();
            if (pc != null) player = pc.gameObject;
        }
        if (player == null)
        {
            var sm = Object.FindObjectOfType<SimpleXRMovement>();
            if (sm != null) player = sm.gameObject;
        }
        if (player == null)
        {
            var cc = Object.FindObjectOfType<CharacterController>();
            if (cc != null) player = cc.gameObject;
        }

        // 2. Se não encontrou, CRIAR do zero
        if (player == null)
        {
            player = new GameObject("Player");
            player.transform.position = new Vector3(0, 1.1f, -10);
            Debug.Log("[AutoConfig] Jogador criado do zero.");
        }

        // 3. CharacterController
        var charCtrl = player.GetComponent<CharacterController>();
        if (charCtrl == null)
        {
            charCtrl = player.AddComponent<CharacterController>();
            charCtrl.height = 1.8f;
            charCtrl.center = new Vector3(0, 0.9f, 0);
            charCtrl.radius = 0.3f;
        }

        // 4. Câmera
        Camera cam = player.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
            if (cam != null && cam.transform.parent == null)
            {
                cam.transform.SetParent(player.transform, false);
                cam.transform.localPosition = new Vector3(0, 1.6f, 0);
            }
            else if (cam == null)
            {
                var camGO = new GameObject("PlayerCamera");
                camGO.tag = "MainCamera";
                camGO.transform.SetParent(player.transform, false);
                camGO.transform.localPosition = new Vector3(0, 1.6f, 0);
                cam = camGO.AddComponent<Camera>();
                cam.nearClipPlane = 0.1f;
                if (Object.FindObjectOfType<AudioListener>() == null)
                    camGO.AddComponent<AudioListener>();
            }
        }

        // 5. PlayerController
        if (player.GetComponent<PlayerController>() == null && player.GetComponent<SimpleXRMovement>() == null)
        {
            var pc = player.AddComponent<PlayerController>();
            pc.cameraTransform = cam.transform;
        }
        else
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null && pc.cameraTransform == null)
                pc.cameraTransform = cam.transform;
        }

        // 6. Crosshair
        if (player.GetComponent<CrosshairUI>() == null)
            player.AddComponent<CrosshairUI>();

        Debug.Log($"🎮 [AutoConfig] Player: '{player.name}' pos={player.transform.position}");
    }

    // ═══════════════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════════════

    Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        Texture2D tex = Resources.Load<Texture2D>(path);
        if (tex != null)
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return Resources.Load<Sprite>(path);
    }

    void ApplyColor(GameObject go, Color color)
    {
        var rend = go.GetComponent<Renderer>();
        if (rend == null) return;
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        rend.material = new Material(shader) { color = color };
    }

    void UpdatePanel(HistoricalItem hi, Sprite spr)
    {
        if (hi.nameText != null) hi.nameText.text = hi.itemName;
        if (hi.descriptionText != null) hi.descriptionText.text = hi.description;
        if (hi.photoImage != null && spr != null)
        {
            hi.photoImage.sprite = spr;
            hi.photoImage.color = Color.white;
            hi.photoImage.preserveAspect = true;
        }
    }

    void BuildInfoPanel(HistoricalItem hi, GameObject item)
    {
        var panel = new GameObject("InfoPanel");
        panel.transform.position = item.transform.position + Vector3.up * 2.5f;
        var cvs = panel.AddComponent<Canvas>();
        cvs.renderMode = RenderMode.WorldSpace;
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
        panel.transform.localScale = Vector3.one * 0.005f;
        panel.AddComponent<GraphicRaycaster>();

        // BG
        var bg = AddUI<Image>(panel, "BG");
        bg.color = new Color(0.1f, 0.06f, 0.03f, 0.95f);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.sizeDelta = Vector2.zero;

        // Foto
        var photo = AddUI<Image>(panel, "Photo");
        photo.preserveAspect = true;
        Pos(photo.gameObject, new Vector2(0, 130), new Vector2(350, 200));
        if (hi.itemPhoto != null) { photo.sprite = hi.itemPhoto; photo.color = Color.white; }
        else { photo.color = new Color(0.3f, 0.25f, 0.2f); }

        // Nome
        var nameT = AddUI<TextMeshProUGUI>(panel, "Name");
        nameT.text = hi.itemName; nameT.fontSize = 28;
        nameT.color = new Color(0.96f, 0.87f, 0.6f);
        nameT.alignment = TextAlignmentOptions.Center; nameT.enableWordWrapping = true;
        Pos(nameT.gameObject, new Vector2(0, 5), new Vector2(380, 50));

        // Descrição
        var descT = AddUI<TextMeshProUGUI>(panel, "Desc");
        descT.text = hi.description; descT.fontSize = 16;
        descT.color = new Color(0.85f, 0.8f, 0.7f);
        descT.alignment = TextAlignmentOptions.TopLeft; descT.enableWordWrapping = true;
        Pos(descT.gameObject, new Vector2(0, -120), new Vector2(370, 200));

        // Fechar
        var cbImg = AddUI<Image>(panel, "CloseBtn");
        cbImg.color = new Color(0.6f, 0.15f, 0.1f);
        var closeBtn = cbImg.gameObject.AddComponent<Button>();
        Pos(cbImg.gameObject, new Vector2(180, 230), new Vector2(40, 40));
        var xT = AddUI<TextMeshProUGUI>(cbImg.gameObject, "X");
        xT.text = "X"; xT.fontSize = 24;
        xT.alignment = TextAlignmentOptions.Center; xT.color = Color.white;
        xT.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

        hi.infoPanel = panel;
        hi.nameText = nameT;
        hi.descriptionText = descT;
        hi.photoImage = photo;
        hi.closeButton = closeBtn;
        panel.SetActive(false);
    }

    T AddUI<T>(GameObject parent, string name) where T : Component
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        return go.AddComponent<T>();
    }

    void Pos(GameObject go, Vector2 pos, Vector2 size)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt != null) { rt.anchoredPosition = pos; rt.sizeDelta = size; }
    }
}

/// <summary>
/// Helper para esperar 1 frame antes de rodar o AutoConfig.
/// Garante que todos os objetos da cena estejam carregados.
/// </summary>
public class DelayedSetup : MonoBehaviour
{
    public System.Collections.IEnumerator RunNextFrame()
    {
        yield return null; // espera 1 frame
        AutoConfigDrawers.RunSetup();
        Destroy(gameObject);
    }
}
