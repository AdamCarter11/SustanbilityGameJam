using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Harpoon : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] float retractSpeed = 5f;

    GameManager gm;

    private float distanceTraveled = 0f;
    private bool grabbedObj = false;
    private bool launchObj = false;
    private GameObject player, planet;
    private Rigidbody2D rb;
    private bool shotBack = false;

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
            if (!launchObj && !shotBack)
            {
                RetractProj();
            }
            else if(launchObj)
            {
                LaunchBackProj();
                launchObj = false;
                shotBack = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
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
        if (planet == null)
        {
            Debug.LogWarning("Planet object not assigned!");
            return;
        }

        // Calculate the direction from the object to the mouse cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePos - transform.position;

        // Normalize the direction to get a unit vector
        Vector3 normalizedDirection = directionToMouse.normalized;

        // Use Rigidbody.AddForce to apply force in the calculated direction
        rb.AddForce(normalizedDirection * retractSpeed * 500 * Time.deltaTime, ForceMode2D.Impulse);
        print("proj force: " + rb.velocity);
        Destroy(this.gameObject, 2f);
    } 
     
    void RetractProj()
    {
        if (player == null)
        {
            Debug.LogWarning("Player object not assigned!");
            return;
        }

        // Calculate the direction from the current position to the target position
        Vector3 directionToTarget = player.transform.position - transform.position;
        if (Vector2.Distance(player.transform.position, transform.position) >= 1f)
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
            if (!grabbedObj)
            {
                grabbedObj = true;
                Destroy(collision.gameObject);
            }
            if (shotBack)
            {
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Earth"))
        {
            Destroy(this.gameObject);
            if (shotBack)
            {
                // planet gets more trash
                gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
                gm.AddTrash(2);
            }
        }
    }
}
