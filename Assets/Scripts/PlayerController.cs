using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class CharacterController2D : MonoBehaviour
{
    // Move player in 2D space
    private BackgroundMusicManager musicManager;
    public static bool stopGroundMovement = false;

    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 3.0f; // Increased gravity
    public Camera mainCamera;
    public float cameraOffset = 2.0f; // Offset to adjust camera position
    public Transform respawnPoint; // Assign the respawn point in the inspector
    public AudioClip fallSoundClip; // Assign the fall sound effect in the inspector

    bool facingRight = true;
    float moveDirection = 1; // Constantly moving to the right
    bool isGrounded = false;
    bool canMove = true; // Flag to determine if the player can move
    Vector3 cameraPos;
    Rigidbody2D r2d;
    BoxCollider2D mainCollider;
    Transform t;
    AudioSource audioSource;

    // Rotation during jump
    float rotationSpeed = 360f;

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<BoxCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fallSoundClip;
        musicManager = FindObjectOfType<BackgroundMusicManager>();

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
{
    if (!canMove)
        return; // If canMove is false, do not process the movement logic

    // Change facing direction
    if (moveDirection > 0 && !facingRight)
    {
        facingRight = true;
        t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
    }
    if (moveDirection < 0 && facingRight)
    {
        facingRight = false;
        t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
    }

    // Jumping
    if (Input.GetKey(KeyCode.W) && isGrounded)
    {
        // Set the velocity only if the player is grounded and "W" is being held down
        r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
    }

    // Camera follow
    if (mainCamera)
    {
        float newX = Mathf.Lerp(mainCamera.transform.position.x, t.position.x + cameraOffset, Time.deltaTime * 5f);
        float newY = Mathf.Lerp(mainCamera.transform.position.y, t.position.y + cameraOffset, Time.deltaTime * 5f);
        mainCamera.transform.position = new Vector3(newX, newY, cameraPos.z);
    }

    // Check if player has fallen off the cliff
    if (t.position.y < -10f) // Change -10f to the appropriate y-coordinate for your respawn point
    {
        Respawn();
    }
}


    void FixedUpdate()
    {
        if (!canMove)
            return; // If canMove is false, do not process the movement logic

        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);

        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;

                    // Check if the object has the "end" tag
                    if (colliders[i].CompareTag("end"))
                    {
                        canMove = false; // Stop the movement
                        stopGroundMovement = true;
                    }
                    break;
                }
            }
        }

        if (isGrounded)
        {
            float angle = Mathf.LerpAngle(t.eulerAngles.z, 0f, Time.fixedDeltaTime * 5f);
            t.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            t.Rotate(Vector3.forward * -rotationSpeed * Time.fixedDeltaTime);
        }

        // Commenting out the line that sets the velocity, making the player not move forward
        // r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
    }

    // Function to respawn the player at the designated point
    void Respawn()
    {
        t.position = respawnPoint.position;
        r2d.velocity = Vector2.zero;

        // Play fall sound
        if (fallSoundClip != null)
        {
            audioSource.Play();
        }
                // Restart background music
        if (musicManager != null)
        {
            musicManager.RestartMusic();
        }
    }
}
