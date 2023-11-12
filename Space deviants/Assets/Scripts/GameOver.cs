using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scorePercent;
    private float earthPercent, playerPercent;
    void Start()
    {
        if (scorePercent == null)
        {
            Debug.LogError("TextMeshPro object not assigned!");
            return;
        }

        // Change the text of the TextMeshPro object
        earthPercent = Mathf.Round(PlayerPrefs.GetFloat("enemy") * 100f * 100f) / 100f;
        playerPercent = Mathf.Round(PlayerPrefs.GetFloat("player") * 100f * 100f) / 100f;
        scorePercent.text = "Earth was : " + playerPercent + "% polluted, Space was: " + earthPercent + "% polluted";
    }
} 
 