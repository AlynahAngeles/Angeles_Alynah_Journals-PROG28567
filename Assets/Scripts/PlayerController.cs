using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D player;
    public float playerSpeed = 7f;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2();

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x = -1;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            playerInput.x = 1;
        }


            MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        player.linearVelocity = new Vector2(playerInput.x * playerSpeed, player.linearVelocity.y);
        player.linearDamping = 0f;

    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
