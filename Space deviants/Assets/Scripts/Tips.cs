using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tips : MonoBehaviour
{
    private TextMeshProUGUI tipText;
    List<string> stringList = new List<string> { "There have been a 38% increase in satellite pollution from 2022 to 2023", "Satellites last hundreds to thousands of years in space", "Space pollution is flying at over 22,000 mph", "Space pollution contributes to the break down of the ozone layer", "Gabriels favorite color is green, so keep our earth clean for the green plants" };
    void Start()
    {
        tipText = GetComponent<TextMeshProUGUI>();
        int randomIndex = Random.Range(0, stringList.Count);

        string randomString = stringList[randomIndex];
        tipText.text = randomString;
    }

} 
