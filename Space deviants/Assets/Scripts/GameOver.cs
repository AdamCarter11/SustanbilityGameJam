using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scorePercent;
    [SerializeField] TextMeshProUGUI scorePlayerPercent;
    [SerializeField] TextMeshProUGUI winOutcome;
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
        earthPercent = Mathf.Min(earthPercent, 100);
        playerPercent = Mathf.Min(playerPercent, 100);
        scorePercent.text = "Earth was : " + playerPercent + "% polluted"; 
        scorePlayerPercent.text = "Space was: " + earthPercent + "% polluted";
        if(playerPercent >= 100)
        {
            winOutcome.text = "Player wins!";
        }
        if(earthPercent >= 100)
        {
            winOutcome.text = "Earth polluted space; Player loses!";
        }
    }
} 
 