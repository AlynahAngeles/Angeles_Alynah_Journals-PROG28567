using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 bombOffset;
    public GameObject bombPrefab;
    public int numberOfBombs;
    public float bombSpacing = -2f;
    public Transform enemyTransform;
    public List<Transform> asteroidTransforms;
    private float inDistance = 2f;
    
    // Update is called once per frame
    void Update()
    {
        SpawnBombAtOffset();
        cornerBombs();
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
}

