using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingBehavior : MonoBehaviour
{
    [SerializeField] GameObject fadeObj;
    public float fadeTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.localScale.x <= 1.5)
        {
            gameObject.transform.localScale += new Vector3(.001f, .001f, .001f);
        }
        

        if(gameObject.transform.localScale.x >= 1.5)
        {
            StartCoroutine(Fade());
            //print("fade");
        }
        
        //print(gameObject.GetComponent<SpriteRenderer>().color);

        if(fadeObj.gameObject.GetComponent<SpriteRenderer>().color.a == 0)
        {
            Destroy(gameObject);
        }
       
    }
    IEnumerator Fade()
    {
        SpriteRenderer rend = fadeObj.gameObject.GetComponent<SpriteRenderer>();
        Color initialColor = rend.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        float elapsedTime = 0f;
        while (elapsedTime < 2.5f)
        {
            elapsedTime += Time.deltaTime;
            rend.color = Color.Lerp(initialColor, targetColor, elapsedTime / 2.5f);
            yield return null;
        }
    }

}
