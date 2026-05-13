using UnityEngine;

/// <summary>
/// Crosshair simples no centro da tela.
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    public float size = 20f;
    public float thickness = 2f;
    public Color color = Color.white;

    void OnGUI()
    {
        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;

        GUI.color = color;

        // Horizontal
        GUI.DrawTexture(new Rect(cx - size / 2f, cy - thickness / 2f, size, thickness), Texture2D.whiteTexture);
        // Vertical
        GUI.DrawTexture(new Rect(cx - thickness / 2f, cy - size / 2f, thickness, size), Texture2D.whiteTexture);

        // Ponto central
        GUI.DrawTexture(new Rect(cx - 2f, cy - 2f, 4f, 4f), Texture2D.whiteTexture);
    }
}
