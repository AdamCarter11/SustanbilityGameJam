using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public Vector2 spawnPoint;
    [SerializeField] int earthRadius = 1;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] float trashForce = 200;
    [SerializeField] float minSpawnSpeed = 100f, maxSpawnSpeed = 150f;
    [SerializeField] float spawnStartDelay = 1f, spawnerDelay = 1f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnTrash", spawnStartDelay, spawnerDelay);
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

    public void SpawnTrash()
    {
        spawnPoint = (Random.insideUnitCircle) * earthRadius;
        GameObject trashInstance;
        trashInstance = Instantiate(trashPrefab,transform.position,Quaternion.identity);
        trashForce = Random.Range(minSpawnSpeed, maxSpawnSpeed);
        trashInstance.GetComponent<Rigidbody2D>().AddForce(spawnPoint * trashForce);
    }
}
