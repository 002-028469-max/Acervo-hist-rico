using UnityEngine;
using UnityEditor;
using System.IO;

public class RoteiroWindow : EditorWindow
{
    private string roteiroText = "";
    private Vector2 scrollPos;

    [MenuItem("Arquivo Historico/Abrir Roteiro Completo")]
    public static void ShowWindow()
    {
        var window = GetWindow<RoteiroWindow>("Roteiro Completo");
        window.minSize = new Vector2(600, 500);
        window.Show();
    }

    private void OnEnable()
    {
        LoadText();
    }

    private void LoadText()
    {
        // Pega o caminho do RoteiroMontagem.txt na raiz do projeto
        string path = Path.Combine(Directory.GetCurrentDirectory(), "RoteiroMontagem.txt");
        if (File.Exists(path))
        {
            roteiroText = File.ReadAllText(path);
        }
        else
        {
            roteiroText = "O arquivo RoteiroMontagem.txt não foi encontrado na raiz do projeto:\n" + path;
        }
    }

    private void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) 
        { 
            fontSize = 16, 
            alignment = TextAnchor.MiddleCenter 
        };
        
        GUIStyle textStyle = new GUIStyle(EditorStyles.label) 
        { 
            wordWrap = true, 
            fontSize = 13, 
            richText = true,
            padding = new RectOffset(5, 5, 5, 5)
        };

        EditorGUILayout.Space(10);
        GUILayout.Label("📖 ARQUIVO HISTÓRICO - ROTEIRO DE MONTAGEM", titleStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Recarregar Arquivo", GUILayout.Height(30)))
        {
            LoadText();
        }
        if (GUILayout.Button("Abrir no Bloco de Notas", GUILayout.Height(30)))
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "RoteiroMontagem.txt");
            Application.OpenURL("file://" + path.Replace("\\", "/"));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
        
        // Exibir o texto com suporte a seleção e cópia, mas usando o estilo de label para wordwrap
        EditorGUILayout.SelectableLabel(roteiroText, textStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinHeight(roteiroText.Split('\n').Length * 16));
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space(5);
        
        // Botão bônus para caso o usuário queira executar a montagem automaticamente
        if (GUILayout.Button("⚙️ Executar Auto-Montagem (SceneBuilder)", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Confirmação", "Deseja rodar o SceneBuilderEditor para montar as cenas MainMenu e MainScene automaticamente?\n\nIsso fará o que está descrito no roteiro.", "Sim, Montar", "Cancelar"))
            {
                // Chama o método do script existente SceneBuilderEditor
                SceneBuilderEditor.BuildProjectSilent();
            }
        }
        EditorGUILayout.Space(5);
    }
}
