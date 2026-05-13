using UnityEngine;
using UnityEditor;

public class RoomBuilder : EditorWindow
{
    [MenuItem("Acervo Historico/Gerar Sala com Cômoda e Gavetas")]
    public static void CreateRoomWithDresser()
    {
        // 1. Criar a Sala (Container)
        GameObject room = new GameObject("Sala_Arquivo");

        // Chão
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Chao";
        floor.transform.parent = room.transform;
        floor.transform.localScale = new Vector3(10f, 0.5f, 10f);
        floor.transform.position = new Vector3(0f, -0.25f, 0f);

        // Teto
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Teto";
        ceiling.transform.parent = room.transform;
        ceiling.transform.localScale = new Vector3(10f, 0.5f, 10f);
        ceiling.transform.position = new Vector3(0f, 4.25f, 0f);

        // Paredes
        CreateWall("Parede_Frente", room.transform, new Vector3(0, 2, 5), new Vector3(10, 4, 0.5f));
        CreateWall("Parede_Tras", room.transform, new Vector3(0, 2, -5), new Vector3(10, 4, 0.5f));
        CreateWall("Parede_Esquerda", room.transform, new Vector3(-5, 2, 0), new Vector3(0.5f, 4, 10));
        CreateWall("Parede_Direita", room.transform, new Vector3(5, 2, 0), new Vector3(0.5f, 4, 10));

        // Luz da Sala
        GameObject lightObj = new GameObject("Luz_Sala");
        lightObj.transform.parent = room.transform;
        lightObj.transform.position = new Vector3(0, 3.5f, 0);
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 15f;
        light.intensity = 2f;

        // 2. Criar a Cômoda
        GameObject dresser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dresser.name = "Comoda";
        dresser.transform.parent = room.transform;
        // Tamanho da cômoda: 2m de largura, 1.5m de altura, 1m de profundidade
        dresser.transform.localScale = new Vector3(2f, 1.5f, 1f);
        // Encostada na Parede da Frente
        dresser.transform.position = new Vector3(0f, 0.75f, 4.4f);
        
        // Material Escuro para a Cômoda
        Renderer dresserRenderer = dresser.GetComponent<Renderer>();
        if (dresserRenderer != null)
        {
            Material dresserMat = new Material(Shader.Find("Standard"));
            dresserMat.color = new Color(0.4f, 0.2f, 0.1f); // Marrom escuro
            dresserRenderer.material = dresserMat;
        }

        // 3. Criar as 5 Gavetas
        // Vamos organizar as gavetas verticalmente
        float startY = 0.3f;
        float spacingY = 0.25f;

        for (int i = 0; i < 5; i++)
        {
            GameObject drawer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drawer.name = $"Gaveta_Interativa_{i + 1}";
            drawer.transform.parent = dresser.transform; // A cômoda é o pai
            
            // As gavetas precisam ser um pouco menores que a largura da cômoda e encaixar nela
            // Como são filhas da cômoda, a escala local é relativa (1 = 100% do pai)
            drawer.transform.localScale = new Vector3(0.9f, 0.15f, 0.9f);
            
            // Posição inicial (fechada) relativa à cômoda
            // y vai de baixo para cima
            float localY = -0.4f + (i * 0.2f); 
            Vector3 closedPos = new Vector3(0f, localY, 0f);
            // Posição aberta (deslizada para trás no eixo Z local, que é o Z global negativo pois a cômoda olha pra frente)
            Vector3 openPos = new Vector3(0f, localY, -0.6f);

            drawer.transform.localPosition = closedPos;

            // Configurar o DrawerController
            DrawerController controller = drawer.AddComponent<DrawerController>();
            controller.closedPosition = closedPos;
            controller.openPosition = openPos;
            controller.slideSpeed = 3f;
            controller.hoverColor = new Color(0.8f, 0.6f, 0.4f); // Cor de destaque ao mirar

            // Colocar um item simbólico dentro (pode ser o HistoricalItem depois)
            GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            item.name = $"Item_Historico_{i + 1}";
            item.transform.parent = drawer.transform;
            item.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            item.transform.localPosition = new Vector3(0, 0, 0);
            
            // Atribuir o item ao controlador para que apareça apenas quando abrir
            controller.itemInDrawer = item;
            item.SetActive(false); // Esconde o item inicialmente
        }

        // Selecionar o quarto criado no Unity
        Selection.activeGameObject = room;
        
        Debug.Log("Sala e Cômoda criadas com sucesso! Elas estão prontas para interação com o PlayerController.");
    }

    private static void CreateWall(string name, Transform parent, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent;
        wall.transform.localScale = scale;
        wall.transform.position = position;
    }
}
