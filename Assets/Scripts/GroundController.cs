using UnityEngine;

public class GroundController : MonoBehaviour
{
    public float acceleration = 5.0f;
    private bool shouldMove = true;

    void Update()
    {
        if (CharacterController2D.stopGroundMovement)
        {
            shouldMove = false;
        }

        if (shouldMove)
        {
            // Accelerate the ground to the left
            transform.Translate(Vector2.left * acceleration * Time.deltaTime);
        }
        else
        {
            // Check for player input to reset ground movement
            if (Input.GetKeyDown(KeyCode.Return))
            {
                shouldMove = true;
                CharacterController2D.stopGroundMovement = false;
            }
        }
    }

    // Check for collisions with the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has the tag "Player" and "end"
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("end"))
        {
            shouldMove = false;
        }
    }
}
