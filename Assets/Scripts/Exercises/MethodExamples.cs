using UnityEngine;

public class MethodExamples : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DrawBoxAtPosition(mousePosition, Vector2.one, new Color(1f, 1f, 1f, 0.5f));
    }

    private void DrawBoxAtPosition(Vector2 position, Vector2 size, Color colour)
    {
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        Vector2 topLeft = position + new Vector2(-halfWidth, halfHeight);
        Vector2 topRight = topLeft + new Vector2(size.x, 0);
        Vector2 bottomRight = topRight + new Vector2(0, -size.y);
        Vector2 bottomLeft = bottomRight + new Vector2(-size.x, 0);

        Debug.DrawLine(topLeft, topRight, colour, 0.5f);
        Debug.DrawLine(topRight, bottomRight, colour, 0.5f);
        Debug.DrawLine(bottomRight, bottomLeft, colour, 0.5f);
        Debug.DrawLine(bottomLeft, topLeft, colour, 0.5f);

    }
}
