using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image earthFill;
    [SerializeField] Image playerFill;
    [SerializeField] GameObject earthTrash;

    private float maxEarthTrash = 100;
    private float maxPlayerTrash = 100;
    public float currEarthTrash = 1;
    public float currPlayerTrash = 1;
    private bool addPlayerTrash = true;
    private bool addEarthTrash;
    private float tempPlayerTrashGoal = 0;
    private float tempEarthTrashGoal = 0;

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerFill.fillAmount = currPlayerTrash / maxPlayerTrash;
        earthFill.fillAmount = currEarthTrash / maxEarthTrash;
        //earthFill.fillAmount = .5f;

        //earthTrash.transform.localScale = earthTrash.transform.localScale * (earthFill.fillAmount * 10);
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
        if(type == 2)
        {
            tempEarthTrashGoal += 10;
            StartCoroutine(fillEarthTrash());

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
    IEnumerator fillEarthTrash()
    {
        addEarthTrash = false;
        while (currEarthTrash < tempEarthTrashGoal)
        {
            currEarthTrash += .1f;
            earthTrash.transform.localScale += (Vector3.one * .001f);
            yield return new WaitForSeconds(.01f);

        }
        addPlayerTrash = true;
    }
}



