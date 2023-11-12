using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image earthFill;
    [SerializeField] Image playerFill;
    [SerializeField] GameObject earthTrash;
    [SerializeField] GameObject playerTrash;

    private float maxEarthTrash = 100;
    private float maxPlayerTrash = 100;
    public float currEarthTrash = 1;
    public float currPlayerTrash = 1;
    private bool addPlayerTrash = true;
    private bool addEarthTrash = true;
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
        GameEndLogic();
        //earthFill.fillAmount = .5f;

        //earthTrash.transform.localScale = earthTrash.transform.localScale * (earthFill.fillAmount * 10);
    }
    private void GameEndLogic()
    {
        if(currEarthTrash + currPlayerTrash >= (maxPlayerTrash + maxEarthTrash) / 2)
        {
            PlayerPrefs.SetFloat("player", currEarthTrash / maxEarthTrash);
            PlayerPrefs.SetFloat("enemy", currPlayerTrash / maxPlayerTrash);
            SceneManager.LoadScene("GameOver");
        }
    }

    public void AddTrash(int type)
    {
        if (type == 1) 
        {
            tempPlayerTrashGoal += 10;
            //if (addPlayerTrash) 
            {
                StartCoroutine(fillPlayerTrash());
            }
            
        }
        if(type == 2)
        {
            tempEarthTrashGoal += 10;
            if(addEarthTrash)
                StartCoroutine(fillEarthTrash());

        }
    }

    IEnumerator fillPlayerTrash()
    {
        addPlayerTrash = false;
        while (currPlayerTrash < tempPlayerTrashGoal)
        {
            currPlayerTrash += .1f;
            playerTrash.transform.localScale *= 0.9984f;
            yield return new WaitForSeconds(.01f);

        }
        addPlayerTrash = true;
    }
    IEnumerator fillEarthTrash()
    {
        //print(tempEarthTrashGoal);
        addEarthTrash = false;

        float baseScale = .9f + (currEarthTrash / 100); // Base scale
        float maxScaleFactor = 0.1f; // Maximum scale factor (10% increase)

        while (currEarthTrash < tempEarthTrashGoal)
        {
            currEarthTrash += 0.01f;

            // Calculate the scale factor based on currEarthTrash
            float scaleFactor = baseScale + (currEarthTrash / tempEarthTrashGoal) * maxScaleFactor;

            // Use the scale factor to modify the scale of earthTrash
            earthTrash.transform.localScale = Vector3.one * scaleFactor;

            yield return new WaitForSeconds(0.01f);
        }

        addEarthTrash = true;
    }
}



