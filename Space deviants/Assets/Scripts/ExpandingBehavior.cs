using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingBehavior : MonoBehaviour
{
    private YieldInstruction fadeInstruction = new YieldInstruction();
    public float fadeTime = .01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.localScale.x <= 2)
        {
            gameObject.transform.localScale += new Vector3(.001f, .001f, .001f);
        }
        

        if(gameObject.transform.localScale.x >= 2)
        {
            FadeOut(gameObject.GetComponent<SpriteRenderer>().color);
        }

        IEnumerator FadeOut(Color go)
        {
            float elapsedTime = 0.0f;
            Color c = go;
            while (elapsedTime < fadeTime)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                c.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
                gameObject.GetComponent<SpriteRenderer>().color = c;
            }
        }
    }


}
