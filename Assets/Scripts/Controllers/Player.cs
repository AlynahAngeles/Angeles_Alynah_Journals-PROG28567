using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 bombOffset;
    public GameObject bombPrefab;
    public int numberOfBombs;
    public float bombSpacing = -2f;
    private float inDistance = 2f;

    public Transform enemyTransform;
    private Transform target;
    private float ratio = 0.5f;
    public List<Transform> asteroidTransforms;
    
    // Update is called once per frame
    void Update()
    {
        SpawnBombAtOffset();
        cornerBombs();
        WarpPlayer(target, ratio);
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
}

