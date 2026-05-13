#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class ExitTotemBuilder
{
    [MenuItem("Acervo Historico/Adicionar Totem de Saida")]
    public static void CreateExitTotem()
    {
        // 1. Objeto Pai (Container do Totem)
        GameObject totem = new GameObject("Totem_Sair");
        
        // Tenta achar a Cômoda/Gabinete para posicionar exatamente ao lado
        GameObject comoda = GameObject.Find("Cabinet");
        if (comoda == null) comoda = GameObject.Find("Comoda");
        
        if (comoda != null)
        {
            // Coloca 2 metros à direita da cômoda e alinhado no eixo Z
            totem.transform.position = comoda.transform.position + new Vector3(8f, -comoda.transform.position.y, -1f);
        }
        else
        {
            totem.transform.position = new Vector3(6f, 0f, 11f); // Posição padrão perto das gavetas
        }

        // 2. Base do Totem (Pedestal)
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.name = "Pedestal";
        baseObj.transform.SetParent(totem.transform, false);
        baseObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        baseObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        Material baseMat = new Material(Shader.Find("Standard"));
        baseMat.color = new Color(0.2f, 0.2f, 0.2f); // Cinza escuro
        baseObj.GetComponent<Renderer>().material = baseMat;

        // 3. O "Cristal" ou Botão no Topo (Parte Interativa)
        GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crystal.name = "Botao_Cristal";
        crystal.transform.SetParent(totem.transform, false);
        crystal.transform.localPosition = new Vector3(0, 1.15f, 0);
        crystal.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        // Rotacionado pra ficar estilo um losango
        crystal.transform.localRotation = Quaternion.Euler(45, 45, 0);

        // Define a Layer Grabbable para a interação com o Raycast
        int grabbableLayer = LayerMask.NameToLayer("Grabbable");
        if (grabbableLayer != -1)
            crystal.layer = grabbableLayer;

        Material crystalMat = new Material(Shader.Find("Standard"));
        crystalMat.color = new Color(0.8f, 0.1f, 0.1f); // Vermelho base
        crystalMat.EnableKeyword("_EMISSION");
        crystalMat.SetColor("_EmissionColor", new Color(0.4f, 0.05f, 0.05f));
        crystal.GetComponent<Renderer>().material = crystalMat;

        // Adiciona o script ExitPanel reaproveitado para atuar no cristal
        ExitPanel exitLogic = crystal.AddComponent<ExitPanel>();
        exitLogic.hoverColor = new Color(1f, 0.3f, 0.3f); // Brilha mais forte ao olhar

        // 4. Texto Flutuante acima do Totem
        GameObject textObj = new GameObject("Text_Sair");
        textObj.transform.SetParent(totem.transform, false);
        textObj.transform.localPosition = new Vector3(0, 1.6f, 0);
        // Virado de frente para o jogador (Z negativo)
        textObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

        var tmpro = textObj.AddComponent<TextMeshPro>();
        tmpro.text = "ENCERRAR JOGO";
        tmpro.fontSize = 2.5f;
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.color = new Color(1f, 0.7f, 0.7f);
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2, 0.5f);

        // Seleciona o totem criado
        Selection.activeGameObject = totem;
        
        Debug.Log("✅ Totem de Saída criado com sucesso!");
    }
}
#endif
