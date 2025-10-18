using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 bombOffset;
    public GameObject bombPrefab;
    public GameObject powerupPrefab;
    public int numberOfBombs;
    public float bombSpacing = -2f;
    private float inDistance = 2f;

    public Transform player;
    public Transform enemyTransform;
    private Transform target;
    private float ratio = 0.5f;
    //public List<Transform> asteroidTransforms;

    private LineRenderer radarLine;

    public float movementSpeed = 6f;

    //Vector mechanics
    public bool playerIsHit = false;
    public float originalSize;

    //Rotation Mechanics for proposal
    float rotationSpeed = 100f;

    void Start()
    {
        SpawnPowerups(4f, 5);

    }
    
    // Update is called once per frame
    void Update()
    {
        SpawnBombAtOffset();
        cornerBombs();
        //WarpPlayer(target, ratio);
        EnemyRadar(3, 8);
        PlayerRotation();
        PlayerMovement();
    }

    public void PlayerMovement()
    {

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Player is moving up.");
            transform.position += Vector3.up * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("Player is moving left");
            transform.position += Vector3.left * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Player is moving down.");
            transform.position += Vector3.down * movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("Player is moving right.");
            transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    private void PlayerRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log("Player is turning left.");
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("Player is rotating right.");
            transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }
    }

    private void SpawnBombAtOffset()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for(int i = 0; i < numberOfBombs; i++)
            {
                Vector3 spawnPosition = transform.position + -(transform.up * (bombSpacing * i)) + (transform.right * bombOffset.x);
                Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    private void cornerBombs()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Player is pressing C.");
            Vector3[] corners = new Vector3[]
            {
                new Vector3(1,1,0).normalized,
                new Vector3(-1,1,0).normalized,
                new Vector3(1,-1,0).normalized,
                new Vector3(-1,-1,0).normalized,
            };

            int randomIndex = Random.Range(0, corners.Length);
            Vector3 cornerDirection = corners[randomIndex];

            Vector3 spawnPosition = transform.position + cornerDirection * inDistance;

            Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void WarpPlayer(Transform target, float ratio)
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Vector2 halfWay = Vector2.Lerp(transform.position, target.position, ratio);
            transform.position = new Vector3(halfWay.x, halfWay.y, transform.position.z);
        }
    }

    public void EnemyRadar(float radius, int circlePoints)
    {
        if(radarLine == null)
        {
            radarLine = gameObject.AddComponent<LineRenderer>() as LineRenderer;
            radarLine.loop = true;
            radarLine.useWorldSpace = false;
            radarLine.widthMultiplier = 0.1f;
            radarLine.sortingLayerName = "Default";
            radarLine.sortingOrder = 10;

            radarLine.material = new Material(Shader.Find("Sprites/Default"));
        }

        circlePoints = Mathf.Max(circlePoints, 3);

        bool enemyDetected = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                enemyDetected = true;
                break;
            }
        }

        Color radarColor = enemyDetected ? Color.red : Color.green;
        radarLine.startColor = radarColor;
        radarLine.endColor = radarColor;


        Vector3[] positions = new Vector3[circlePoints];
        for (int i = 0; i < circlePoints; i++)
        {
            float angle = i * Mathf.PI * 2f / circlePoints;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            positions[i] = new Vector3(x, y, 0f);
        }

        radarLine.positionCount = circlePoints;
        radarLine.SetPositions(positions);
    }

    public void SpawnPowerups(float radius, int numberOfPowerups)
    {
        Vector3 center = player.position;

        for (int i = 0; i < numberOfPowerups; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPowerups;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 position = center + new Vector3(x, y, 0f); 

            Instantiate(powerupPrefab, position, Quaternion.identity);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerIsHit(collision.collider);
    }

    public void PlayerIsHit(Collider2D collider2D)
    {
        if(collider2D.CompareTag("Asteroid") && !playerIsHit)
        {
           StartCoroutine(PlayerPulsate(2f, 2f));
        }

        if (collider2D.CompareTag("Enemy") && !playerIsHit)
        {
            StartCoroutine(PlayerPulsate(2f, 2f));
        }
    }

    public IEnumerator PlayerPulsate(float duration, float maxScale)
    {
        Debug.Log("Player is hit!");
        playerIsHit = true;

        float elapsed = 0f;
        Vector3 originalSize = transform.localScale;

        while (elapsed < duration)
        {
           elapsed += Time.deltaTime;
           float scale = Mathf.Lerp(originalSize.x, maxScale, Mathf.PingPong(Time.time * 4f, 1f));
            transform.localScale = new Vector3(scale, scale, originalSize.z);
            yield return null;
        }

        transform.localScale = originalSize;
        playerIsHit = false;
    }
}

