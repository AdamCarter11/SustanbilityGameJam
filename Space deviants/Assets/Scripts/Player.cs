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
    [SerializeField] float stunDur = 2.5f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private GameObject projectile = null;
    private int health;
    private bool canDash = true; // Flag to check if the player can dash
    private bool stunned = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        health = startingHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (!stunned)
        {
            HandleThrustInput();
            HandleRotationInput();
            ApplyGravitationalPull();
        }
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

        // Normalize the direction to get a unit vector
        Vector2 direction = Vector2.right;
        Vector2 normalizedDirection = direction.normalized;

        // Use Rigidbody2D.AddForce to apply force in the calculated direction
        rb.AddForce(normalizedDirection * dashForce, ForceMode2D.Impulse);
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
            if(health <= 0)
            {
                // stun logic
                StartCoroutine(StunLogic());
            }
        }
    }
    IEnumerator StunLogic()
    {
        stunned = true;
        StartCoroutine(FlashCharacter());
        yield return new WaitForSeconds(stunDur);
        health = startingHealth;
        stunned = false;
    }
    IEnumerator FlashCharacter()
    {
        // Flash for 3 seconds
        float duration = stunDur;

        // Repeat every 0.2 seconds
        float flashInterval = 0.2f;

        while (duration > 0)
        {
            yield return new WaitForSeconds(flashInterval);
            isFlashing = !isFlashing;

            // Set the opacity based on whether it's currently flashing
            spriteRenderer.color = isFlashing ? new Color(originalColor.r, originalColor.g, originalColor.b, 0f) : originalColor;

            duration -= flashInterval;
        }

        // Reset the color to the original color after the flashing is done
        spriteRenderer.color = originalColor;
    }
}
