using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] float retractSpeed = 5f;
    [SerializeField] float shakeMagnitude = 0.1f;
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float pauseDuration = 0.5f;
    [SerializeField] ParticleSystem particleSystemPrefab;
    [SerializeField] GameObject trashVisualization;

    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    private AudioSource audioSource;

    GameManager gm;

    private float distanceTraveled = 0f;
    private bool grabbedObj = false;
    private bool launchObj = false;
    private bool shotBack = false;
    private bool canMove = true;
    private GameObject player, planet;
    private Rigidbody2D rb;
    private Camera mainCamera;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isOrbiting = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        planet = GameObject.FindGameObjectWithTag("Earth");
        audioSource = GameObject.FindGameObjectWithTag("AudioPlayer").GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Store initial position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!grabbedObj && !isOrbiting)
        {
            MoveProjectile();
        }
        else
        {
            if (!launchObj && !shotBack)
            {
                if (canMove)  // Only retract if allowed to move
                    RetractProj();
            }
            else if (launchObj)
            {
                LaunchBackProj();
                launchObj = false;
                shotBack = true;
                canMove = false;  // Disable movement after firing back
            }
            else if (Input.GetMouseButtonDown(0) && isOrbiting)
            {
                StopOrbiting();
                launchObj = true;
                canMove = true;  // Enable movement when launching again
            }
            else if (isOrbiting)
            {
                OrbitPlayer();
            }
        }

        if (Input.GetMouseButtonDown(0) && !isOrbiting && canMove && !launchObj)
        {
            launchObj = true;
        }

        CheckMaxDistance();
    }

    private void CheckMaxDistance()
    {
        if (distanceTraveled >= maxDistance && !grabbedObj)
        {
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
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
                trashVisualization.SetActive(true);
                transform.localScale *= 2f;
                Destroy(collision.gameObject);
                Time.timeScale = 1f;
            }
            if (shotBack)
            {
                Destroy(collision.gameObject);
                Time.timeScale = 1f;
                Destroy(this.gameObject);
                ParticleSystem newParticleSystem = Instantiate(particleSystemPrefab, transform.position, transform.rotation);
                PlayRandomAudioClip();
            }
        }
        if (collision.gameObject.CompareTag("Earth"))
        {
            StartCoroutine(DestroyWithDelay());
            if (shotBack)
            {
                gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
                gm.AddTrash(2);
            }
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        distanceTraveled += speed * Time.deltaTime;
    }

    private void LaunchBackProj()
    {
        if (planet == null)
        {
            Debug.LogWarning("Planet object not assigned!");
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePos - transform.position;
        Vector3 normalizedDirection = directionToMouse.normalized;

        rb.AddForce(normalizedDirection * retractSpeed * 500 * Time.deltaTime, ForceMode2D.Impulse);
        Time.timeScale = 1f;
        Destroy(this.gameObject, 2f);
    }

    private void RetractProj()
    {
        if (player == null)
        {
            Debug.LogWarning("Player object not assigned!");
            return;
        }

        Vector3 directionToTarget = player.transform.position - transform.position;
        if (Vector2.Distance(player.transform.position, transform.position) >= 1f)
        {
            Vector3 normalizedDirection = directionToTarget.normalized;
            transform.Translate(normalizedDirection * retractSpeed * Time.deltaTime);
        }
        else
        {
            isOrbiting = true;
            shotBack = true;
            print("retract?");
        }
    }

    private void OrbitPlayer()
    {
        
        float orbitSpeed = 2f;
        float orbitRadius = 1.5f;

        // Calculate the angle based on time and speed
        float angle = Time.time * orbitSpeed;

        // Calculate the position in a circular motion
        float x = Mathf.Cos(angle) * orbitRadius;
        float y = Mathf.Sin(angle) * orbitRadius;

        // Set the new position relative to the player
        transform.position = player.transform.position + new Vector3(x, y, 0f);
        
        //transform.position = player.transform.position + new Vector3(1f, 0, 0);
    }

    private void StopOrbiting()
    {
        isOrbiting = false;
        //transform.position = initialPosition;
        //transform.rotation = initialRotation;
    }

    private IEnumerator DestroyWithDelay()
    {
        if (shotBack)
        {
            StartCoroutine(ScreenShake());
            Time.timeScale = 0.2f;
            PlayRandomAudioClip();
        }
        yield return new WaitForSeconds(pauseDuration);

        Time.timeScale = 1f;
        
        //PlayRandomAudioClip();

        if (shotBack)
        {
            ParticleSystem newParticleSystem = Instantiate(particleSystemPrefab, transform.position, transform.rotation);
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            gm.AddTrash(2);
        }
        Destroy(gameObject);
    }

    private IEnumerator ScreenShake()
    {
        Vector3 originalPosition = mainCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;
    }

    void PlayRandomAudioClip()
    {
        // Use Random.Range to generate a random index within the list
        int randomIndex = Random.Range(0, audioClips.Count);

        // Access the randomly chosen AudioClip
        AudioClip randomAudioClip = audioClips[randomIndex];

        // Play the randomly chosen audio clip
        audioSource.clip = randomAudioClip;
        audioSource.Play();
    }
}
