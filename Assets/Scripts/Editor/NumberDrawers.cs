#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class NumberDrawers
{
    [MenuItem("Acervo Historico/Numerar Gavetas (1 a 5)")]
    public static void AddNumbersToDrawers()
    {
        // Encontra todos os controladores de gaveta na cena
        DrawerController[] drawers = Object.FindObjectsOfType<DrawerController>();
        
        if (drawers.Length == 0)
        {
            Debug.LogError("Nenhuma gaveta com o script DrawerController foi encontrada na cena! Gere a sala primeiro.");
            return;
        }

        // Ordena pelo nome para garantir que a Gaveta 1 receba o número 1, etc.
        // Isso funciona tanto para o SceneBuilder (Drawer_0, Drawer_1) quanto para o RoomBuilder (Gaveta_Interativa_1...)
        System.Array.Sort(drawers, (a, b) => a.name.CompareTo(b.name));

        for (int i = 0; i < drawers.Length; i++)
        {
            GameObject drawer = drawers[i].gameObject;
            
            // Tenta achar e deletar a numeração se ela já foi criada antes para não duplicar
            Transform oldText = drawer.transform.Find("Text_NumeroDaGaveta");
            if (oldText != null)
            {
                Object.DestroyImmediate(oldText.gameObject);
            }

            // Cria um novo texto 3D e coloca como filho da gaveta
            GameObject textObj = new GameObject("Text_NumeroDaGaveta");
            textObj.transform.SetParent(drawer.transform, false);

            // Posiciona na face frontal da gaveta. 
            // O modelo de cubo no Unity tem raio local de 0.5. Como a frente geralmente é Z negativo, usamos -0.51
            // Se a gaveta tiver puxador (handle), o texto ficará flutuando na frente.
            textObj.transform.localPosition = new Vector3(0, 0, -0.52f);
            
            // Inverte a rotação para o texto ficar visível para o jogador que está olhando pra gaveta
            textObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

            var tmpro = textObj.AddComponent<TextMeshPro>();
            tmpro.text = (i + 1).ToString(); // Número da gaveta: 1, 2, 3...
            tmpro.fontSize = 5;
            tmpro.alignment = TextAlignmentOptions.Center;
            tmpro.color = new Color(0.9f, 0.8f, 0.5f); // Cor Dourada/Amarelada para combinar com o museu
            tmpro.fontStyle = FontStyles.Bold;

            RectTransform rt = textObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1, 1);

            // Informa ao Unity que a gaveta foi modificada para salvar na cena
            EditorUtility.SetDirty(drawer);
        }

        Debug.Log($"✅ {drawers.Length} gavetas foram numeradas com sucesso (de 1 a {drawers.Length})!");
    }
}
#endif
