using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image earthFill;
    [SerializeField] Image playerFill;
    private float maxEarthTrash = 100;
    private float maxPlayerTrash = 100;
    public float currEarthTrash = 100;
    public float currPlayerTrash = 1;
    private bool addPlayerTrash = true;
    private bool addEarthTrash;
    private float tempPlayerTrashGoal = 0;

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerFill.fillAmount = currPlayerTrash / maxPlayerTrash;
        earthFill.fillAmount = currEarthTrash / maxEarthTrash;

        
    }

    public void AddTrash(int type)
    {
        if (type == 1) 
        {
            tempPlayerTrashGoal += 10;
            print(tempPlayerTrashGoal);
            //if (addPlayerTrash) 
            {
                StartCoroutine(fillPlayerTrash());
            }
            
        }
    }

    IEnumerator fillPlayerTrash()
    {
        addPlayerTrash = false;
        while (currPlayerTrash < tempPlayerTrashGoal)
        {
            currPlayerTrash += .1f;
            yield return new WaitForSeconds(.01f);

        }
        addPlayerTrash = true;
    }
}



