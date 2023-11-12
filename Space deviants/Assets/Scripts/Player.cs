using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileDir;
    [SerializeField] Image healthFill;
    [SerializeField] int startingHealth = 20;
    [SerializeField] int damageFromProj = 5;
    [SerializeField] float damageFromCloud = .1f;
    [SerializeField] float stunDur = 2.5f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private GameObject projectile = null;
    private float health;
    private bool canDash = true; // Flag to check if the player can dash
    private bool stunned = false;
    private float origionalMaxSpeed;

    private Color originalColor;
    private bool isFlashing = false;
    private bool isDashing = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        health = startingHealth;
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        origionalMaxSpeed = maxSpeed;
    }

    void Update()
    {
        if (!stunned)
        {
            HandleThrustInput();
            HandleRotationInput();
            if (!isDashing)
            {
                ApplyGravitationalPull();
            }
        }
        // Check for dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        ClampToScreen();
        HarpoonLogic();
        UpdateHealthUI();

        CapSpeed();
    }

    IEnumerator Dash()
    {
        // Disable the ability to dash during cooldown
        canDash = false;
        // Double the maxSpeed during the dash
        maxSpeed *= 2;

        isDashing = true;

        // Store the input direction
        Vector2 dashDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        // Use Rigidbody2D.AddForce to apply force in the input direction
        rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
        spriteRenderer.flipX = dashDirection.x < 0;
        Invoke("ResetDash", .5f);
        yield return new WaitForSeconds(dashCooldown);

        // Enable dashing after the cooldown
        canDash = true;
    }
    void ResetDash()
    {
        isDashing = false;
        maxSpeed = origionalMaxSpeed;
    }

    void UpdateHealthUI()
    {
        healthFill.fillAmount = (float)health / (float)startingHealth;
        //print(health / (float)startingHealth);
    }

    void HarpoonLogic()
    {
        if (Input.GetMouseButtonDown(0) && projectile == null)
        {
            projectile = Instantiate(projectilePrefab, transform.position, projectileDir.rotation);
        }
    }

    void HandleThrustInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;

        // Apply extra thrust force during dashing
        float finalThrustForce = isDashing ? thrustForce * 2 : thrustForce;
        rb.AddForce(movementDirection * finalThrustForce);
        if (horizontalInput != 0)
            spriteRenderer.flipX = horizontalInput < 0;
    }

    void HandleRotationInput()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        Vector3 directionToMouse = mousePos - transform.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        projectileDir.rotation = Quaternion.Slerp(projectileDir.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        health -= damageFromCloud;
        if(health <= 0)
        {
            StartCoroutine(StunLogic());
        }
    }
    IEnumerator StunLogic()
    {
        stunned = true;
        StartCoroutine(FlashCharacter());
        rb.freezeRotation = false;
        yield return new WaitForSeconds(stunDur);
        rb.freezeRotation = true;
        health = startingHealth;
        transform.rotation = Vector3ToQuaternion(new Vector3(0,0,0));
        stunned = false;
    }
    Quaternion Vector3ToQuaternion(Vector3 vector)
    {
        // Assuming the vector lies in the xy-plane, calculate the angle of rotation around the z-axis
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        // Create a quaternion that represents the rotation around the z-axis
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angle);

        return quaternion;
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
