#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class ConfigDrawer1
{
    [MenuItem("Acervo Historico/Configurar Gaveta 1 (Humberto)")]
    public static void Configure()
    {
        // Tenta achar a gaveta 1 gerada pelo RoomBuilder ou SceneBuilder
        GameObject drawer = GameObject.Find("Gaveta_Interativa_1");
        if (drawer == null) drawer = GameObject.Find("Drawer_0");

        if (drawer == null)
        {
            Debug.LogError("Gaveta 1 não encontrada na cena!");
            return;
        }

        // Tenta achar o item dentro da gaveta
        Transform itemTrans = drawer.transform.Find("Item_Historico_1");
        if (itemTrans == null) itemTrans = drawer.transform.Find("Item_ManuscritoColonial"); // Padrão do SceneBuilder
        if (itemTrans == null && drawer.transform.childCount > 0)
        {
            // Pega o primeiro filho que parece ser um item interativo
            foreach (Transform child in drawer.transform)
            {
                if (child.name.StartsWith("Item") || child.GetComponent<Collider>() != null || child.GetComponent<HistoricalItem>() != null)
                {
                    itemTrans = child;
                    break;
                }
            }
        }

        if (itemTrans == null)
        {
            Debug.LogError("Objeto do item dentro da gaveta não encontrado!");
            return;
        }

        GameObject item = itemTrans.gameObject;
        item.name = "Item_Humberto";

        // Adiciona HistoricalItem se nao tiver
        HistoricalItem histItem = item.GetComponent<HistoricalItem>();
        if (histItem == null)
        {
            histItem = item.AddComponent<HistoricalItem>();
        }

        histItem.itemName = "Humberto de Maracanã";
        histItem.description = "Humberto Barbosa Magalhães, eternizado como Humberto de Maracanã, foi uma das maiores vozes da cultura maranhense. Como amo do Boi de Maracanã por décadas, ele elevou o sotaque de matraca ao reconhecimento nacional.";

        // Atualiza a UI se o painel já existir (gerado pelo SceneBuilder)
        if (histItem.infoPanel != null)
        {
            if (histItem.nameText != null) histItem.nameText.text = histItem.itemName;
            if (histItem.descriptionText != null) histItem.descriptionText.text = histItem.description;
            
            // Ajusta o nome do painel
            histItem.infoPanel.name = "InfoPanel_Humberto";
        }
        else
        {
            // Se foi gerado pelo RoomBuilder, precisamos criar o painel do zero
            CreatePanel(histItem, item);
        }

        // Marca o objeto como sujo para salvar a cena
        EditorUtility.SetDirty(item);
        if (histItem.infoPanel != null) EditorUtility.SetDirty(histItem.infoPanel);

        // Foca no item no Unity
        Selection.activeGameObject = item;
        Debug.Log("✅ Gaveta 1 configurada com os dados do Humberto de Maracanã!");
    }

    private static void CreatePanel(HistoricalItem hi, GameObject item)
    {
        GameObject panelCanvas = new GameObject("InfoPanel_Humberto");
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

        // Foto (Espaço para a imagem)
        var photoGO = new GameObject("Photo");
        photoGO.transform.SetParent(panelCanvas.transform, false);
        var photoImg = photoGO.AddComponent<Image>();
        photoImg.color = new Color(0.9f, 0.9f, 0.9f); // Fica cinza claro até ter a foto
        var photoRT = photoGO.GetComponent<RectTransform>();
        photoRT.anchoredPosition = new Vector2(0, 130);
        photoRT.sizeDelta = new Vector2(350, 200);

        // Texto Nome
        var nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(panelCanvas.transform, false);
        var nameT = nameGO.AddComponent<TextMeshProUGUI>();
        nameT.text = hi.itemName;
        nameT.fontSize = 32;
        nameT.color = new Color(0.96f, 0.87f, 0.6f);
        nameT.alignment = TextAlignmentOptions.Center;
        var nameRT = nameGO.GetComponent<RectTransform>();
        nameRT.anchoredPosition = new Vector2(0, 5);
        nameRT.sizeDelta = new Vector2(380, 45);

        // Texto Descricao
        var descGO = new GameObject("DescText");
        descGO.transform.SetParent(panelCanvas.transform, false);
        var descT = descGO.AddComponent<TextMeshProUGUI>();
        descT.text = hi.description;
        descT.fontSize = 18;
        descT.color = new Color(0.85f, 0.8f, 0.7f);
        descT.alignment = TextAlignmentOptions.TopLeft;
        descT.enableWordWrapping = true;
        var descRT = descGO.GetComponent<RectTransform>();
        descRT.anchoredPosition = new Vector2(0, -120);
        descRT.sizeDelta = new Vector2(370, 200);

        // Botao Fechar
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

        // Associa as referencias no script
        hi.infoPanel = panelCanvas;
        hi.nameText = nameT;
        hi.descriptionText = descT;
        hi.photoImage = photoImg;
        hi.closeButton = closeBtn;

        panelCanvas.SetActive(false); // Fica oculto por padrão
    }
}
#endif
