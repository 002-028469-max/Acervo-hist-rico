#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class ExitPanelBuilder
{
    [MenuItem("Acervo Historico/Adicionar Painel de Saida")]
    public static void CreateExitPanel()
    {
        // Criação da base do painel
        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "Painel_Sair";
        
        // Posição na parede de trás (perto de onde o player nasce, assumindo Z = -4.9)
        panel.transform.position = new Vector3(0, 1.6f, -14.9f); // A sala do SceneBuilder tem a parede traseira em z=-15
        panel.transform.localScale = new Vector3(1.2f, 0.6f, 0.1f);
        
        // Atribui a Layer Grabbable para que o Raycast do jogador o identifique
        int grabbableLayer = LayerMask.NameToLayer("Grabbable");
        if (grabbableLayer != -1)
            panel.layer = grabbableLayer;

        // Material escuro com tom avermelhado
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.3f, 0.1f, 0.1f);
        panel.GetComponent<Renderer>().material = mat;

        // Adiciona a lógica de interação
        panel.AddComponent<ExitPanel>();

        // Texto 3D flutuante na frente do painel
        GameObject textObj = new GameObject("Text_Sair");
        textObj.transform.SetParent(panel.transform, false);
        
        // Move o texto ligeiramente para frente do cubo para não atravessar (no eixo Z local)
        textObj.transform.localPosition = new Vector3(0, 0, 0.52f);
        
        // Inverte a rotação 180 graus porque a parede traseira olha para Z Positivo
        // E o cubo filho de um pai sem rotação herda o eixo Z do mundo
        textObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

        var tmpro = textObj.AddComponent<TextMeshPro>();
        tmpro.text = "SAIR DA SALA";
        tmpro.fontSize = 3;
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.color = new Color(1f, 0.8f, 0.8f);
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1, 0.5f);

        // Seleciona o painel criado para o desenvolvedor ver
        Selection.activeGameObject = panel;
        
        Debug.Log("✅ Painel de Saída criado com sucesso na parede traseira da sala!");
    }
}
#endif
