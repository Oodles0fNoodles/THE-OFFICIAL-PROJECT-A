using System.Collections;
using UnityEngine;

public class RotateOnCollision : MonoBehaviour
{
    public float rotationAmount = 90f; // Amount of rotation in degrees
    public float rotationDuration = 0.1f; // Duration of rotation in seconds
    private bool hasRotated = false; // Flag to track if rotation has occurred

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object is the player and rotation hasn't happened yet
        if (collision.gameObject.CompareTag("Player") && !hasRotated)
        {
            // Start the rotation coroutine
            StartCoroutine(RotateCoroutine());
        }
    }

    private IEnumerator RotateCoroutine()
    {
        hasRotated = true; // Set the flag to indicate rotation has started

        // Calculate the pivot point at the right end of the ground object
        Vector3 pivot = transform.position + transform.right * (GetComponent<BoxCollider2D>().size.x / 2);

        // Get the initial rotation of the ground object
        Quaternion startRotation = transform.rotation;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + Vector3.forward * rotationAmount);

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            // Interpolate the rotation gradually over time
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);

            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the rotation is exactly at the target rotation
        transform.rotation = targetRotation;
    }
}
