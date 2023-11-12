using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplodeBehavior : MonoBehaviour
{
    [SerializeField] GameObject toxicCloud;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(SceneManager.GetActiveScene().name == "SamepleScene")
            Instantiate(toxicCloud,transform.position,Quaternion.identity);
    }
}
