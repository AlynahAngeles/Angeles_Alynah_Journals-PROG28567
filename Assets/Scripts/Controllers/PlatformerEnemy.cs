using UnityEngine;
using TMPro;

public class PlatformerEnemy : MonoBehaviour
{
    public float enemyMaxHP = 20f;
    public float enemyCurrentHP;

    public float playerDetector = 2f;
    private Transform player;
    private TextMeshProUGUI enemyHP;

    void Start()
    {
        enemyCurrentHP = enemyMaxHP;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null)
        {
            player = playerObject.transform;
        }

        enemyHP = GetComponentInChildren<TextMeshProUGUI>(true);

        if (enemyHP != null)
        {
            enemyHP.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null)
            return; 

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= playerDetector)
        {
            if(enemyHP != null)
            {
                enemyHP.gameObject.SetActive(true);
                enemyHP.text = "HP: " + Mathf.RoundToInt(enemyCurrentHP);
            }
        }
        else
        {
            enemyHP.gameObject.SetActive(false);
        }

        if(enemyCurrentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Damaged(int damageTaken)
    {
        enemyCurrentHP -= damageTaken;
    }

    public bool HPVisible()
    {
        return enemyHP != null && enemyHP.gameObject.activeSelf;
    }
}
