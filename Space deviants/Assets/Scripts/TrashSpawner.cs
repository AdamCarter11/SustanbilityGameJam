using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public Vector2 spawnPoint;
    [SerializeField] int earthRadius = 1;
    [SerializeField] GameObject trashPrefab;
    [SerializeField] float trashForce = 200;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnTrash();
        }
    }

    public void SpawnTrash()
    {
        spawnPoint = (Random.insideUnitCircle) * earthRadius;
        GameObject trashInstance;
        trashInstance = Instantiate(trashPrefab,transform.position,Quaternion.identity);
        trashForce = Random.Range(150, 200);
        trashInstance.GetComponent<Rigidbody2D>().AddForce(spawnPoint * trashForce);
    }
}
