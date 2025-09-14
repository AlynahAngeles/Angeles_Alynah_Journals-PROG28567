using UnityEngine;

public class SquareSpawner : MonoBehaviour
{
    public float squareSize = 1f; //size of square

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition; //Position of square is where the mouse is located
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

            //Find the corners of the square being spawned
            Vector3 topLeft = worldPos + new Vector3(-squareSize / 2, squareSize / 2, 0);
            Vector3 topRight = worldPos + new Vector3(squareSize / 2, squareSize / 2, 0);
            Vector3 bottomRight = worldPos + new Vector3(squareSize / 2, -squareSize / 2, 0);
            Vector3 bottomLeft = worldPos + new Vector3(-squareSize / 2, -squareSize / 2, 0);

            //Draw the four sides of the square using the coordinates above
            Debug.DrawLine(topLeft, topRight, Color.white, 2f);
            Debug.DrawLine(topRight, bottomRight, Color.white, 2f);
            Debug.DrawLine(bottomRight, bottomLeft, Color.white, 2f);
            Debug.DrawLine(bottomLeft, topLeft, Color.white, 2f);
        }
    }
}