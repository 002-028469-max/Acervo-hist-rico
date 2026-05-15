#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Configura as gavetas 2 a 5 com os dados dos artefatos do Espaço Humberto de Maracanã.
/// Gaveta 2: Boi Enfeitado | Gaveta 3: Boi | Gaveta 4: Caboclo | Gaveta 5: Humberto (foto histórica)
/// </summary>
public class ConfigDrawers2a5
{
    [MenuItem("Acervo Historico/Configurar Gavetas 2 a 5")]
    public static void ConfigureAll()
    {
        ConfigureDrawer2();
        ConfigureDrawer3();
        ConfigureDrawer4();
        ConfigureDrawer5();
        Debug.Log("✅ Gavetas 2, 3, 4 e 5 configuradas com sucesso!");
    }

    [MenuItem("Acervo Historico/Configurar Gaveta 2 (Boi Enfeitado)")]
    public static void ConfigureDrawer2()
    {
        ConfigureDrawer(
            drawerIndex: 2,
            drawerNames: new[] { "Gaveta_Interativa_2", "Drawer_1" },
            itemName: "Boi de Maracanã Enfeitado",
            description: "O Bumba-meu-Boi de Maracanã é uma das mais tradicionais manifestações culturais do Maranhão. " +
                         "Nesta imagem, o Boi aparece ricamente enfeitado com bordados, fitas coloridas, canutilhos e espelhos, " +
                         "pronto para a apresentação no arraial. A decoração do Boi é uma arte transmitida entre gerações, " +
                         "cada detalhe carregando simbolismo e devoção aos santos juninos — São João, São Pedro e São Marçal.",
            photoPath: "Assets/Resources/Fotos/Boi enfeitado.jpg",
            panelName: "InfoPanel_BoiEnfeitado",
            itemObjName: "Item_BoiEnfeitado"
        );
    }

    [MenuItem("Acervo Historico/Configurar Gaveta 3 (Boi)")]
    public static void ConfigureDrawer3()
    {
        ConfigureDrawer(
            drawerIndex: 3,
            drawerNames: new[] { "Gaveta_Interativa_3", "Drawer_2" },
            itemName: "Boi de Maracanã",
            description: "O Boi de Maracanã é um dos mais antigos e respeitados grupos de Bumba-meu-Boi do Maranhão, " +
                         "fundado no bairro de Maracanã, em São Luís. Com sotaque de matraca, seu ritmo inconfundível " +
                         "é marcado pelas batidas das matracas de madeira e pelos pandeirões. O Boi representa a força " +
                         "da cultura popular maranhense e a resistência das tradições afro-indígenas do estado.",
            photoPath: "Assets/Resources/Fotos/Boi.jpg",
            panelName: "InfoPanel_Boi",
            itemObjName: "Item_Boi"
        );
    }

    [MenuItem("Acervo Historico/Configurar Gaveta 4 (Caboclo)")]
    public static void ConfigureDrawer4()
    {
        ConfigureDrawer(
            drawerIndex: 4,
            drawerNames: new[] { "Gaveta_Interativa_4", "Drawer_3" },
            itemName: "O Caboclo de Pena",
            description: "O Caboclo de Pena é um dos personagens mais emblemáticos do Bumba-meu-Boi do Maranhão. " +
                         "Vestido com cocar de penas e trajes que remetem à ancestralidade indígena, o Caboclo " +
                         "representa a herança dos povos originários na formação cultural maranhense. " +
                         "Seus movimentos na brincadeira são cheios de energia e reverência, conectando o sagrado ao festivo " +
                         "nas celebrações juninas do Espaço Humberto de Maracanã.",
            photoPath: "Assets/Resources/Fotos/Caboclo.jpg",
            panelName: "InfoPanel_Caboclo",
            itemObjName: "Item_Caboclo"
        );
    }

    [MenuItem("Acervo Historico/Configurar Gaveta 5 (Tradicao Historica)")]
    public static void ConfigureDrawer5()
    {
        ConfigureDrawer(
            drawerIndex: 5,
            drawerNames: new[] { "Gaveta_Interativa_5", "Drawer_4" },
            itemName: "Tradição Histórica — Boi de Maracanã",
            description: "A tradição do Bumba-meu-Boi de Maracanã remonta a décadas de história e resistência cultural no Maranhão. " +
                         "Reconhecido como Patrimônio Cultural Imaterial do Brasil, o Bumba-meu-Boi reúne elementos das culturas " +
                         "africana, indígena e europeia em uma celebração única. O Boi de Maracanã, com seu sotaque de matraca, " +
                         "é símbolo vivo dessa tradição, mantendo acesa a chama da identidade maranhense através das gerações.",
            photoPath: "Assets/Resources/Fotos/Boidemaracanã.jpg",
            panelName: "InfoPanel_TradicaoHistorica",
            itemObjName: "Item_TradicaoHistorica"
        );
    }

    private static void ConfigureDrawer(int drawerIndex, string[] drawerNames, string itemName,
        string description, string photoPath, string panelName, string itemObjName)
    {
        // Encontrar a gaveta
        GameObject drawer = null;
        foreach (var name in drawerNames)
        {
            drawer = GameObject.Find(name);
            if (drawer != null) break;
        }

        if (drawer == null)
        {
            Debug.LogError($"Gaveta {drawerIndex} não encontrada na cena! Procurei: {string.Join(", ", drawerNames)}");
            return;
        }

        // Encontrar item dentro da gaveta
        Transform itemTrans = null;
        foreach (Transform child in drawer.transform)
        {
            if (child.name.StartsWith("Item") || child.GetComponent<HistoricalItem>() != null)
            {
                itemTrans = child;
                break;
            }
        }

        if (itemTrans == null)
        {
            Debug.LogError($"Item dentro da gaveta {drawerIndex} não encontrado!");
            return;
        }

        GameObject item = itemTrans.gameObject;
        item.name = itemObjName;

        // Configurar HistoricalItem
        HistoricalItem histItem = item.GetComponent<HistoricalItem>();
        if (histItem == null)
            histItem = item.AddComponent<HistoricalItem>();

        histItem.itemName = itemName;
        histItem.description = description;

        // Carregar a foto
        Sprite spr = LoadSprite(photoPath);
        if (spr != null)
        {
            histItem.itemPhoto = spr;
            Debug.Log($"  📷 Foto carregada: {photoPath}");
        }
        else
        {
            Debug.LogWarning($"  ⚠️ Foto não encontrada: {photoPath}");
        }

        // Configurar o DrawerController
        DrawerController dc = drawer.GetComponent<DrawerController>();
        if (dc != null)
        {
            dc.itemName = itemName;
            dc.description = description;
            dc.itemPhoto = spr;
        }

        // Atualizar painel existente ou criar novo
        if (histItem.infoPanel != null)
        {
            histItem.infoPanel.name = panelName;
            if (histItem.nameText != null) histItem.nameText.text = itemName;
            if (histItem.descriptionText != null) histItem.descriptionText.text = description;
            if (histItem.photoImage != null && spr != null)
            {
                histItem.photoImage.sprite = spr;
                histItem.photoImage.color = Color.white;
                histItem.photoImage.preserveAspect = true;
            }
        }
        else
        {
            CreatePanel(histItem, item, panelName);
        }

        EditorUtility.SetDirty(item);
        EditorUtility.SetDirty(drawer);
        if (histItem.infoPanel != null) EditorUtility.SetDirty(histItem.infoPanel);

        Debug.Log($"✅ Gaveta {drawerIndex} configurada: {itemName}");
    }

    private static Sprite LoadSprite(string path)
    {
        // Garantir que a textura está importada como Sprite
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null && ti.textureType != TextureImporterType.Sprite)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.maxTextureSize = 1024;
            ti.SaveAndReimport();
        }

        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s == null)
        {
            Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (t != null)
                s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
        return s;
    }

    private static void CreatePanel(HistoricalItem hi, GameObject item, string panelName)
    {
        GameObject panelCanvas = new GameObject(panelName);
        panelCanvas.transform.position = item.transform.position + Vector3.up * 1.5f;
        var cvs = panelCanvas.AddComponent<Canvas>();
        cvs.renderMode = RenderMode.WorldSpace;
        var cvsRT = panelCanvas.GetComponent<RectTransform>();
        cvsRT.sizeDelta = new Vector2(400, 500);
        panelCanvas.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        panelCanvas.AddComponent<GraphicRaycaster>();

        // Fundo
        var bgGO = new GameObject("PanelBG");
        bgGO.transform.SetParent(panelCanvas.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.06f, 0.03f, 0.95f);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        // Foto
        var photoGO = new GameObject("Photo");
        photoGO.transform.SetParent(panelCanvas.transform, false);
        var photoImg = photoGO.AddComponent<Image>();
        photoImg.preserveAspect = true;
        var photoRT = photoGO.GetComponent<RectTransform>();
        photoRT.anchoredPosition = new Vector2(0, 130);
        photoRT.sizeDelta = new Vector2(350, 200);

        if (hi.itemPhoto != null)
        {
            photoImg.sprite = hi.itemPhoto;
            photoImg.color = Color.white;
        }
        else
        {
            photoImg.color = new Color(0.9f, 0.9f, 0.9f);
        }

        // Nome
        var nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(panelCanvas.transform, false);
        var nameT = nameGO.AddComponent<TextMeshProUGUI>();
        nameT.text = hi.itemName;
        nameT.fontSize = 28;
        nameT.color = new Color(0.96f, 0.87f, 0.6f);
        nameT.alignment = TextAlignmentOptions.Center;
        nameT.enableWordWrapping = true;
        var nameRT = nameGO.GetComponent<RectTransform>();
        nameRT.anchoredPosition = new Vector2(0, 5);
        nameRT.sizeDelta = new Vector2(380, 50);

        // Descrição
        var descGO = new GameObject("DescText");
        descGO.transform.SetParent(panelCanvas.transform, false);
        var descT = descGO.AddComponent<TextMeshProUGUI>();
        descT.text = hi.description;
        descT.fontSize = 16;
        descT.color = new Color(0.85f, 0.8f, 0.7f);
        descT.alignment = TextAlignmentOptions.TopLeft;
        descT.enableWordWrapping = true;
        descT.overflowMode = TextOverflowModes.Ellipsis;
        var descRT = descGO.GetComponent<RectTransform>();
        descRT.anchoredPosition = new Vector2(0, -120);
        descRT.sizeDelta = new Vector2(370, 200);

        // Botão Fechar
        var closeBtnGO = new GameObject("CloseButton");
        closeBtnGO.transform.SetParent(panelCanvas.transform, false);
        var closeBtnImg = closeBtnGO.AddComponent<Image>();
        closeBtnImg.color = new Color(0.6f, 0.15f, 0.1f);
        var closeBtn = closeBtnGO.AddComponent<Button>();
        var closeBtnRT = closeBtnGO.GetComponent<RectTransform>();
        closeBtnRT.anchoredPosition = new Vector2(180, 230);
        closeBtnRT.sizeDelta = new Vector2(40, 40);

        var closeTextGO = new GameObject("X");
        closeTextGO.transform.SetParent(closeBtnGO.transform, false);
        var closeT = closeTextGO.AddComponent<TextMeshProUGUI>();
        closeT.text = "X";
        closeT.fontSize = 24;
        closeT.alignment = TextAlignmentOptions.Center;
        closeT.color = Color.white;
        closeTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

        // Associar referências
        hi.infoPanel = panelCanvas;
        hi.nameText = nameT;
        hi.descriptionText = descT;
        hi.photoImage = photoImg;
        hi.closeButton = closeBtn;

        panelCanvas.SetActive(false);
    }
}
#endif
