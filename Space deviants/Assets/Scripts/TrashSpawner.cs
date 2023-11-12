using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public Vector2 spawnPoint;
    [SerializeField] int earthRadius = 1;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] GameObject wavePrefab;
    [SerializeField] GameObject explodingPrefab;
    [SerializeField] GameObject compactor;
    [SerializeField] float trashForce = 200;
    [SerializeField] float minSpawnSpeed = 100f, maxSpawnSpeed = 150f;
    [SerializeField] float spawnStartDelay = 1f, spawnerDelay = 1f;
    [SerializeField] float compactorSpawnStartDelay = 3f, compactorSpawnerDelay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnTrash", spawnStartDelay, spawnerDelay);
        InvokeRepeating("SpawnCompactor", compactorSpawnStartDelay, compactorSpawnerDelay);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnTrash();
        }
        */
    }
    public void SpawnCompactor()
    {
        Vector2 offset = new Vector2(0,0);

        int random = Random.Range(0, 4);
        if (random == 0) 
        {
            offset += new Vector2(Random.Range(2,4), 0);
        }
        if (random == 1)
        {
            offset += new Vector2(Random.Range(-4, -2), 0);
        }
        if (random == 2)
        {
            offset += new Vector2(0, Random.Range(2, 4));
        }
        if (random == 3)
        {
            offset += new Vector2(0, Random.Range(-4, -2));
        }

        float angle = Mathf.Atan2(spawnPoint.y, spawnPoint.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Instantiate(compactor, transform.position + (Vector3)offset, targetRotation);
        
    }
    public void SpawnTrash()
    {
        spawnPoint = (Random.insideUnitCircle) * earthRadius;
        GameObject trashInstance;
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            trashInstance = Instantiate(trashPrefab, transform.position, Quaternion.identity);
            trashForce = Random.Range(minSpawnSpeed, maxSpawnSpeed);
            trashInstance.GetComponent<Rigidbody2D>().AddForce(spawnPoint * trashForce);
        }
        if (random == 1)
        {
            float angle = Mathf.Atan2(spawnPoint.y, spawnPoint.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            trashInstance = Instantiate(wavePrefab, transform.position, targetRotation);
            trashForce = Random.Range(minSpawnSpeed, maxSpawnSpeed);
        }
        if (random == 2)
        {
            trashInstance = Instantiate(explodingPrefab, transform.position, Quaternion.identity);
            trashForce = Random.Range(minSpawnSpeed, maxSpawnSpeed);
            trashInstance.GetComponent<Rigidbody2D>().AddForce(spawnPoint * trashForce);
        }
        



    }
}
