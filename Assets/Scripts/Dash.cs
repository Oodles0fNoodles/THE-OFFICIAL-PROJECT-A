using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private bool canDash = true;
    public float dashDistance = 3.0f;
    public float dashCooldown = 5.0f;

    // Reference to the Particle System component on the player
    public ParticleSystem dashParticle;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(DashMovement());
        }
    }

    IEnumerator DashMovement()
    {
        canDash = false;

        // Play the particle system loop
        dashParticle.Play();

        float originalAcceleration = GetComponent<GroundController>().acceleration;
        GetComponent<GroundController>().acceleration *= 5.0f; // Increase acceleration during dash

        float dashTimer = 0.0f;

        while (dashTimer < 0.5f) // Dash duration (adjust as needed)
        {
            transform.Translate(Vector2.left * dashDistance * Time.deltaTime);
            dashTimer += Time.deltaTime;
            yield return null;
        }

        GetComponent<GroundController>().acceleration = originalAcceleration; // Reset acceleration

        // Stop the particle system loop after the dash
        dashParticle.Stop();

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
