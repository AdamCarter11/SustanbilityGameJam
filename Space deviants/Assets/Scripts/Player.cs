using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float thrustForce = 5f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float maxGravitationalForce = 15f; // New variable for max gravitational force
    [SerializeField] float boundaryForce = 2f;
    [SerializeField] float gravitationalForce;
    [SerializeField] Transform planet;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Image healthFill;
    [SerializeField] int startingHealth = 20;
    [SerializeField] int damageFromProj = 5;

    private Rigidbody2D rb;
    private Camera mainCamera;
    GameObject projectile = null;
    private int health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        health = startingHealth;
    }

    void Update()
    {
        HandleThrustInput();
        HandleRotationInput();
        ApplyGravitationalPull();
        ClampToScreen();
        CapSpeed();
        HarpoonLogic();
        UpdateHealthUI();
    }
    void UpdateHealthUI()
    {
        healthFill.fillAmount = (float)health / (float)startingHealth;
        print(health / startingHealth);
    }
    void HarpoonLogic()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && projectile == null)
        {
            projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }

    void HandleThrustInput()
    {
        // Get the WASD movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction based on the input
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Apply thrust force in the movement direction
        rb.AddForce(movementDirection * thrustForce);
    }

    void HandleRotationInput()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // Maintain the same Z-coordinate as the player

        // Calculate the direction from the player to the mouse cursor
        Vector3 directionToMouse = mousePos - transform.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // Create a Quaternion from the angle
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Rotate towards the target rotation using Slerp for smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ApplyGravitationalPull()
    {
        if (planet != null)
        {
            Vector2 directionToTarget = planet.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > 0)
            {
                // Use Mathf.Min to ensure the gravitational force doesn't exceed maxGravitationalForce
                float actualGravitationalForce = Mathf.Min(gravitationalForce, maxGravitationalForce);

                Vector2 gravitationalForceVector = directionToTarget.normalized * (1 / distanceToTarget) * actualGravitationalForce;
                rb.AddForce(gravitationalForceVector);
            }
        }
    }

    void ClampToScreen()
    {
        Vector3 clampedPosition = mainCamera.WorldToScreenPoint(transform.position);

        float minX = 0f;
        float maxX = Screen.width;
        float minY = 0f;
        float maxY = Screen.height;

        if (clampedPosition.x <= minX || clampedPosition.x >= maxX)
        {
            rb.AddForce(Vector2.left * boundaryForce * Mathf.Sign(clampedPosition.x - Screen.width / 2));
        }

        if (clampedPosition.y <= minY || clampedPosition.y >= maxY)
        {
            rb.AddForce(Vector2.down * boundaryForce * Mathf.Sign(clampedPosition.y - Screen.height / 2));
        }

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        transform.position = mainCamera.ScreenToWorldPoint(clampedPosition);
    }

    void CapSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    #region Collision Logic
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            health -= damageFromProj;
            Destroy(collision.gameObject);
        }
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            health -= damageFromProj;
            Destroy(collision.gameObject);
        }
    }
    #endregion
}
