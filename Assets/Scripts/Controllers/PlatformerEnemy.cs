using UnityEngine;
using TMPro; //added this so that I can use Text Mesh Pro for the HP bar

public class PlatformerEnemy : MonoBehaviour
{
    public float enemyMaxHP = 20f; //enemy's HP is 20
    public float enemyCurrentHP; //variable for the enemy's HP and updates when the enemy is hit

    public float playerDetector = 2f; //the player must be 2 units close to the enemy to be detected
    private Transform player; //to sense the player's transform position
    private TextMeshProUGUI enemyHP; //TMP for the enemy's HP bar

    void Start()
    {
        enemyCurrentHP = enemyMaxHP; //at the start, the enemy's HP will be the max

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); //find the Game Object in the scene with the tag "Player"
        if(playerObject != null) //if the player is in the scene, then...
        {
            player = playerObject.transform; //track the player's movement around the scene
        }

        enemyHP = GetComponentInChildren<TextMeshProUGUI>(true); //assign enemyHP to the TMP placed in the scene

        if (enemyHP != null) //if the UI exists in the scene
        {
            enemyHP.gameObject.SetActive(false); //set it as non-active (not visible) by default
        }
    }

    void Update()
    {
        if (player == null) //if the player is not being detected, then return
            return; 

        float distance = Vector2.Distance(transform.position, player.position); //calculates the distance between the square and the position of the player game object

        if (distance <= playerDetector) //if the distance between the two objects is less than the player range then...
        {
            if(enemyHP != null) //if the UI exists, then...
            {
                enemyHP.gameObject.SetActive(true); //make the UI visible (to indicate that the player is close enough to attack and lunge
                enemyHP.text = "HP: " + Mathf.RoundToInt(enemyCurrentHP); //Displays the current HP of the enemy
            }
        }
        else //if not above then...
        {
            enemyHP.gameObject.SetActive(false); //the UI is automatically hidden (player is NOT close enough to attack)
        }

        if(enemyCurrentHP <= 0) //if the enemy's HP is less than or equal to 0 (or dead)
        {
            Destroy(gameObject); //Destroy the game object to indicate the death of the enemy
        }
    }

    public void Damaged(int damageTaken) //method for the damage the enemy is taking
    {
        enemyCurrentHP -= damageTaken; //subtract the damage taken from the current HP (this is connected to the Player Controller and takes the randomized attack number and subtracts it from the enemy)
    }

    public bool HPVisible() //public boolean for if the HP bar is visible or not
    {
        return enemyHP != null && enemyHP.gameObject.activeSelf; //checks if the enemy's UI exists and is visible. This info is returned to the Player Controller so that the player can attack when this UI is visible
    }
}
