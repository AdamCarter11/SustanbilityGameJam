using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Harpoon : MonoBehaviour
{
    [SerializeField] float speed = 5f;  // Adjust the speed as needed
    [SerializeField] float maxDistance = 5f;  // Adjust the distance the projectile should travel
    [SerializeField] float retractSpeed = 5f;

    private float distanceTraveled = 0f;
    private bool grabbedObj = false;
    private bool launchObj = false;
    private GameObject player, planet;
    private Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        planet = GameObject.FindGameObjectWithTag("Earth");
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!grabbedObj)
        {
            MoveProjectile();
        }
        else
        {
            if (!launchObj)
            {
                RetractProj();
            }
            else
            {
                LaunchBackProj();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            launchObj = true;
        }
        

        // Check if the projectile has traveled the desired distance
        if (distanceTraveled >= maxDistance && !grabbedObj)
        {
            Destroy(gameObject);
        }
    }
    void LaunchBackProj()
    {
        // Calculate the direction from the object to the planet
        Vector3 directionToTarget = planet.transform.position - transform.position;

        // Normalize the direction to get a unit vector
        Vector3 normalizedDirection = directionToTarget.normalized;

        // Use Rigidbody.AddForce to apply force in the calculated direction
        rb.AddForce(normalizedDirection * retractSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }
    void RetractProj()
    {
        if (player == null)
        {
            Debug.LogWarning("Target object not assigned!");
            return;
        }

        // Calculate the direction from the current position to the target position
        Vector3 directionToTarget = player.transform.position - transform.position;
        if(Vector2.Distance(player.transform.position, transform.position) >= 1f)
        {
            // Normalize the direction to get a unit vector
            Vector3 normalizedDirection = directionToTarget.normalized;

            // Move the object in the calculated direction
            transform.Translate(normalizedDirection * retractSpeed * Time.deltaTime);
        }
    }

    void MoveProjectile()
    {
        // Move the projectile forward based on its local space
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Update the distance traveled
        distanceTraveled += speed * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            grabbedObj = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            grabbedObj = true;
        }
        if (collision.gameObject.CompareTag("Earth"))
        {
            Destroy(this.gameObject);
            if (launchObj)
            {
                // planet gets more trash
            }
        }
    }
}
