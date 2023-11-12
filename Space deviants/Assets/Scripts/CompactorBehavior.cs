using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactorBehavior : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gm = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
            gm.emptyTrash();
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Trash")
        {
            Destroy(gameObject);
        }
    }
}
