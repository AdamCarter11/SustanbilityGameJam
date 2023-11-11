using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float thrustForce = 5f;
    [SerializeField] float dashForce = 20f; // New variable for dash force
    [SerializeField] float dashCooldown = 2f; // Cooldown for the dash
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float maxGravitationalForce = 15f;
    [SerializeField] float boundaryForce = 2f;
    [SerializeField] float gravitationalForce;
    [SerializeField] Transform planet;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Image healthFill;
    [SerializeField] int startingHealth = 20;
    [SerializeField] int damageFromProj = 5;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private GameObject projectile = null;
    private int health;
    private bool canDash = true; // Flag to check if the player can dash

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

        // Check for dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        // Disable the ability to dash during cooldown
        canDash = false;

        // Store the current velocity for restoration after the dash
        Vector2 originalVelocity = rb.velocity;

        // Apply a burst of force in the current facing direction
        rb.AddForce(transform.right * dashForce, ForceMode2D.Impulse);

        // Wait for a short duration (adjust as needed)
        yield return new WaitForSeconds(2f);

        // Restore the original velocity
        rb.velocity = originalVelocity;

        // Enable the ability to dash after the cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void UpdateHealthUI()
    {
        healthFill.fillAmount = (float)health / (float)startingHealth;
        print(health / (float)startingHealth);
    }

    void HarpoonLogic()
    {
        if (Input.GetMouseButtonDown(0) && projectile == null)
        {
            projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }

    void HandleThrustInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;

        rb.AddForce(movementDirection * thrustForce);
    }

    void HandleRotationInput()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        Vector3 directionToMouse = mousePos - transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            health -= damageFromProj;
            Destroy(collision.gameObject);
        }
    }
}
