#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ForceLoadPhoto
{
    [MenuItem("Acervo Historico/Forçar Carregamento da Foto do Humberto")]
    public static void LoadPhoto()
    {
        // Tenta encontrar qualquer textura com o nome 'humberto' no projeto
        string[] guids = AssetDatabase.FindAssets("humberto t:Texture2D");
        if (guids.Length == 0)
        {
            Debug.LogError("Nenhuma foto com o nome 'humberto' foi encontrada no seu projeto! Certifique-se de que você arrastou a foto para dentro do Unity e que o nome do arquivo contém 'humberto'.");
            return;
        }

        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        
        // Converte a textura para Sprite automaticamente
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }

        // Carrega o Sprite
        Sprite humbertoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

        // Acha a gaveta e o item
        GameObject item = GameObject.Find("Item_Humberto");
        if (item == null)
        {
            GameObject drawer = GameObject.Find("Gaveta_Interativa_1");
            if (drawer == null) drawer = GameObject.Find("Drawer_0");
            if (drawer != null && drawer.transform.childCount > 0)
            {
                foreach (Transform child in drawer.transform)
                {
                    if (child.GetComponent<HistoricalItem>() != null)
                    {
                        item = child.gameObject;
                        break;
                    }
                }
            }
        }

        if (item != null)
        {
            HistoricalItem histItem = item.GetComponent<HistoricalItem>();
            if (histItem != null)
            {
                histItem.itemPhoto = humbertoSprite;
                EditorUtility.SetDirty(histItem);
                Debug.Log($"✅ Foto do Humberto de Maracanã ('{assetPath}') aplicada com sucesso na Gaveta 1!");
            }
        }
    }
}
#endif
